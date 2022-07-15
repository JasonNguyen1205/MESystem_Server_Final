using Newtonsoft.Json;

namespace MESystem.Data.HR;

public class Employee
{
 
    [JsonProperty("companyCode")]
    public string? companyCode { get; set; }
    [JsonProperty("code")]
    public string? code { get; set; }
    [JsonProperty("payload")]
    public Payload? payload { get; set; }
}
