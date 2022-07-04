using System;
using System.IO;
using System.Threading;
using Client.Services.Messaging;
using Client.Task;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Client;

internal static class Program
{
    private static void Main()
    {
        BuildConfiguration();
        RegisterServices()
            .Run(new CancellationToken());
    }

    private static Runner RegisterServices()
    {
        var host = Host
            .CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddSingleton<IRunner, Runner>();
                services.AddSingleton<IBroker, Broker>();
            }).Build();

        return ActivatorUtilities.CreateInstance<Runner>(host.Services);
    }

    private static void BuildConfiguration()
    {
        ConfigurationBuilder builder = new();
        builder
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("RUNTIME_ENVIRONMENT")}.json", true, true)
            .AddEnvironmentVariables();
    }
}
