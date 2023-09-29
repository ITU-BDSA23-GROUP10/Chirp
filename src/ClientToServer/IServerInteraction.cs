namespace ClientToServer;

public interface IServerInteraction<T>
{
    public Task<string?> GetToEndpointWithJsonResponceAsync(string endpoint);

    public Task<string?> PostToEndpointWithJsonResponceAsync(string endpoint, T postData);
}
