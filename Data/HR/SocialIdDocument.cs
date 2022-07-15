using Newtonsoft.Json;

namespace MESystem.Data.HR;

public class SocialIdDocument
{

    [JsonProperty("number")]

    public string? number { get; set; }
    [JsonProperty("issuedDate")]

    public string? issuedDate { get; set; }
    [JsonProperty("issuedPlace")]

    public string issuedPlace { get; set; }

}
