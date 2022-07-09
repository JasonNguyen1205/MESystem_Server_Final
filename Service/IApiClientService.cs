using MESystem.Data.Location;
namespace MESystem.Service;

public interface IApiClientService
{
    Task<IPAddress> GetUserIPAsync();
    Task<UserGeoLocation> GetLocationAsync(string userIp);
}
