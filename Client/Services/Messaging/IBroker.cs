using System.Threading;

namespace Client.Services.Messaging;

public interface IBroker
{
    void Listen(CancellationToken cancellationToken);
    void WaitReady();
}
