using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ClientToServer;

public class ClientServerInteraction<T> : IServerInteraction<T>
{
    private readonly HttpClient client;

    public ClientServerInteraction(string baseAddress)
    {
        client = new HttpClient
        {
            BaseAddress = new Uri(baseAddress)
        };
    }

    public async Task<string?> GetToEndpointWithJsonResponceAsync(string endpoint)
    {
        //Our requests for data should expect JSON (the method works without, but its another layer of specificity)
        //https://learn.microsoft.com/en-us/uwp/api/windows.web.http.httpclient.defaultrequestheaders?view=winrt-22621
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
        //sends an asynchronous GET request to the endpoint "cheeps"
        var response = await client.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();
        //reads the content of the response as a string asynchronously in JSON format
        var json = await response.Content.ReadAsStringAsync();
        
        return json;
    }

    public async Task<string?> PostToEndpointWithJsonResponceAsync(string endpoint, T postData)
    {
        // Set up header to expect JSON
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
        // Send POST request and wait for responce
        var response = await client.PostAsJsonAsync(endpoint, postData);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        
        return result;
    }
}
