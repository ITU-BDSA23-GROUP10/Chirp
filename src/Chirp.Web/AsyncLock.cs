namespace Chirp.Web;

public class AsyncPadlock : IDisposable
{
    private readonly static SemaphoreSlim _semaphore = new(1, 1);

    public async Task<AsyncPadlock> Lock()
    {
        await _semaphore.WaitAsync();
        return this;
    }

    public void Dispose()
    {
        _semaphore.Release();
        GC.SuppressFinalize(this);
    }
}