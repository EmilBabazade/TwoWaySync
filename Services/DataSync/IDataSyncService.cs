
namespace Services.DataSync;

public interface IDataSyncService
{
    Task SynchronizeLocalToRemoteAsync(CancellationToken cancellationToken = default);
    Task SynchronizeRemoteToLocalAsync(CancellationToken cancellationToken = default);
}