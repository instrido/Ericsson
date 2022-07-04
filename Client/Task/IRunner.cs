using System.Threading;

namespace Client.Task;

internal interface IRunner
{
    void Run(CancellationToken cancellationToken);
}