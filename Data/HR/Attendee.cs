using Newtonsoft.Json;

namespace MESystem.Data.HR;

public class Attendee
{
    [JsonProperty("attendeeCode")]
    public string? attendeeCode
    { get; set; }


    [JsonProperty("code")]
    public string? code { get; set; }


    [JsonProperty("payload")]
    public AttendancePayload? payload { get; set; }  
    

   
}
