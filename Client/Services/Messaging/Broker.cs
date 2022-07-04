using System;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Client.Services.Messaging;

public class BrokerSettings
{
    public string HostName { get; set; }
    public string QueueName { get; set; }
}

public class Broker : IBroker
{
    private readonly BrokerSettings _settings;
    private (IConnection connection, IModel channel, EventingBasicConsumer consumer) _brokerState;

    public Broker(IConfiguration config)
    {
        _settings = new BrokerSettings
        {
            HostName = config.GetValue<string>("BROKER_HOST"),
            QueueName = config.GetValue<string>("BROKER_QUEUE")
        };
    }

    public Broker(IConfiguration config,
        (IConnection connection, IModel channel, EventingBasicConsumer consumer) brokerState) : this(config)
    {
        _brokerState = brokerState;
    }

    public void WaitReady()
    {
        var connected = false;
        var timeToRetry = TimeSpan.FromSeconds(1);

        Console.WriteLine($"Connecting to broker: {_settings.HostName}, queue: {_settings.QueueName}");
        Console.Write("Waiting to connect to broker..");

        while (!connected)
            try
            {
                Connect();
                if (!_brokerState.connection.IsOpen) continue;
                Console.WriteLine("Connected!");
                connected = true;
            }
            catch (BrokerUnreachableException)
            {
                Console.Write(".");
                Thread.Sleep(timeToRetry);
            }
    }

    public void Listen(CancellationToken cancellationToken)
    {
        _brokerState.consumer.Received += (_, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.Write(message);
        };

        _brokerState.connection.ConnectionShutdown += (_, ea) =>
        {
            Console.WriteLine(ea.Initiator switch
            {
                ShutdownInitiator.Peer => "Connection closed by peer.",
                _ => "Connection closed locally."
            });
            WaitReady();
        };

        _brokerState.channel.BasicConsume(_settings.QueueName, true, _brokerState.consumer);
        cancellationToken.WaitHandle.WaitOne();
    }

    private void Connect()
    {
        if (_brokerState.connection is { IsOpen: true }) return;

        ConnectionFactory factory = new() { HostName = _settings.HostName };
        _brokerState.connection = factory.CreateConnection();
        _brokerState.channel = _brokerState.connection.CreateModel();
        _brokerState.channel.QueueDeclare(_settings.QueueName, false, false, false, null);
        _brokerState.consumer = new EventingBasicConsumer(_brokerState.channel);
    }
}
