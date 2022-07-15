using Newtonsoft.Json;

namespace MESystem.Data.HR;

public class Salary
{
    [JsonProperty("value")]

    public double value { get; set; }
    [JsonProperty("currency")]

    public string? currency { get; set; }
}
