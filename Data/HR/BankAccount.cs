using Newtonsoft.Json;

namespace MESystem.Data.HR;

public class BankAccount
{
    [JsonProperty("bankCode")]

    public string? bankCode { get; set; }
    [JsonProperty("account")]

    public string? account { get; set; }
}
