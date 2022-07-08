namespace MESystem.Data.HR;

public class HRCheckInOut
{
    public int UserEnrollNumber { get; set; }

    public string? TimeStr { get; set; }

    public DateTime TimeDate { get; set; }

    public string? OriginType { get; set; }

    public string? NewType { get; set; }

    public string? Source { get; set; }

    public int MachineNo { get; set; }

    public int WorkCode { get; set; }

    public string? SerialNumber { get; set; }

    public int EventType{ get; set; }

    public string? CardNo { get; set; }

    public int InOutState { get; set; }

    public int MaskFlag { get; set; }

    public double Temperature { get; set; }

}
