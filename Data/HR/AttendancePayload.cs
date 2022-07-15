using Newtonsoft.Json;

namespace MESystem.Data.HR;

public class AttendancePayload
{
    [JsonProperty("date")]
    public string date { get; set; }


    [JsonProperty("times")]
    public List<string> times { get; set; }


    [JsonProperty("shiftCount")]
    public int shiftCount { get; set; }  
    

    [JsonProperty("type")]
    public string? type { get; set; }
   
}

public enum type
{
    Unknown,
    RecordMissing, 
    Standard, 
    HalfTime, 
    PaidAbsent,
    UnpaidAbsent
}