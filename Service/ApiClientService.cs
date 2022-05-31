using System.Drawing.Printing;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MESystem.Data.Location;
using MESystem.Service;
namespace MESystem.Service
{
    public class ApiClientService : IApiClientService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ApiClientService(IHttpClientFactory httpClientfactory)
        {
            _httpClientFactory = httpClientfactory;
        }

        public async Task<IPAddress?> GetUserIPAsync()
        {
            var client = _httpClientFactory.CreateClient("IP");
            return await client.GetFromJsonAsync<IPAddress>("/");
        }
        public async Task<UserGeoLocation?> GetLocationAsync(string userIp)
        {
            string path = $"{userIp}?access_key=d654307d664f10538be30ef67b32bcbe";
            var client = _httpClientFactory.CreateClient("Location");
            return await client.GetFromJsonAsync<UserGeoLocation>(path);
        }

        //private string barcodeImage = GetBarcodeAsSvgBase64Image();

     
    }
}