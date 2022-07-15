using Newtonsoft.Json;

namespace MESystem.Data.HR;

public class Payload
{
    [JsonProperty("fullName")]
    public string? fullName { get; set; }
    [JsonProperty("workplaceId")]
    public string? workplaceId { get; set; }
    [JsonProperty("status")]
    public string? status { get; set; }
    [JsonProperty("salary")]
    public Salary? salary { get; set; }
    [JsonProperty("phoneNumber")]
    public string? phoneNumber { get; set; }
    [JsonProperty("email")]
    public string? email { get; set; }
    [JsonProperty("bankAccount")]
    public BankAccount? bankAccount { get; set; }
    [JsonProperty("socialIdDocument")]
    public SocialIdDocument? socialIdDocument { get; set; }
    [JsonProperty("gender")]
    public string? gender { get; set; }
    [JsonProperty("startDate")]
    public string? startDate { get; set; }
    [JsonProperty("dob")]
    public string? dob { get; set; }
    [JsonProperty("attendeeCodes")]
    public string? attendeeCode { get; set; }
    [JsonProperty("workingDayPerMonth")]

    public int? workingDayPerMonth { get; set; }
    [JsonProperty("departmentName")]

    public string? departmentName { get; set; }
    [JsonProperty("workplaceName")]


    public string? workplaceName { get; set; }
    [JsonProperty("contractType")]


    public string? contractType { get; set; }
}
