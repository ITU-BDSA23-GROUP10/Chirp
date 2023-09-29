namespace ClientToSercer;

public class ClientServerInteraction<T> : IServerInteraction<T>
{
    private static readonly HttpClient client;

    public ClientServerInteraction(string baseAddress)
    {
        client = new HttpClient
        {
            BaseAddress = new Uri(baseAddress)
        };
    }

    public string? GetToEndpointWithJsonResponce(string endpoint)
    {
        //Our requests for data should expect JSON (the method works without, but its another layer of specificity)
        //https://learn.microsoft.com/en-us/uwp/api/windows.web.http.httpclient.defaultrequestheaders?view=winrt-22621
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
        //sends an asynchronous GET request to the endpoint "cheeps"
        var response = await client.GetAsync("cheeps");
        response.EnsureSuccessStatusCode();
        //reads the content of the response as a string asynchronously in JSON format
        var json = await response.Content.ReadAsStringAsync();
        
        return json;
    }

    public string? PostToEndpointWithJsonResponce(string endpoint, T postData)
    {

    }
}
