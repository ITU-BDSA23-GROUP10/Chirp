namespace ClientToServer;

public interface IServerInteraction<T>
{
    public string? GetToEndpointWithJsonResponce(string endpoint);

    public string? PostToEndpointWithJsonResponce(string endpoint, T postData);
}
