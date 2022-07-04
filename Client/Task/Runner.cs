using System.Threading;
using Client.Services.Messaging;

namespace Client.Task;

internal class Runner : IRunner
{
    private readonly IBroker _service;

    public Runner(IBroker service)
    {
        _service = service;
    }

    public void Run(CancellationToken cancellationToken)
    {
        _service.WaitReady();

        _service.Listen(cancellationToken);

        cancellationToken.WaitHandle.WaitOne();
    }
}
