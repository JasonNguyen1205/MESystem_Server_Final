using System.Net.Http;

using MESystem.Data.Location;
namespace MESystem.Service;

public class ApiClientService : IApiClientService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ApiClientService(IHttpClientFactory httpClientfactory)
    {
        _httpClientFactory=httpClientfactory;
    }

    public async Task<IPAddress?> GetUserIPAsync()
    {
        HttpClient? client = _httpClientFactory.CreateClient("IP");
        return await client.GetFromJsonAsync<IPAddress>("/");
    }
    public async Task<UserGeoLocation?> GetLocationAsync(string userIp)
    {
        var path = $"{userIp}?access_key=d654307d664f10538be30ef67b32bcbe";
        HttpClient? client = _httpClientFactory.CreateClient("Location");
        return await client.GetFromJsonAsync<UserGeoLocation>(path);
    }

    //private string barcodeImage = GetBarcodeAsSvgBase64Image();


}