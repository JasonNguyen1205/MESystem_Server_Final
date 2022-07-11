using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESystem.Data.HR;


[Table("CheckInOut", Schema = "dbo")]
public class CheckInOut
{
    [Column(nameof(CardNo), Order = 11, TypeName = "nvarchar(30)")]
    public string? CardNo { get; set; }
    [Column(nameof(EventAddr), Order = 10, TypeName = "tinyint")]
    public int? EventAddr { get; set; }
    [Column(nameof(EventType), Order = 9, TypeName = "tinyint")]
    public int? EventType { get; set; }
    [Column(nameof(InOutState), Order = 12, TypeName = "tinyint")]
    public int? InOutState { get; set; }
    [Column(nameof(MachineNo), Order = 6, TypeName = "tinyint")]
    public int? MachineNo { get; set; }
    [Column(nameof(MaskFlag), Order = 13, TypeName = "tinyint")]
    public int? MaskFlag { get; set; }
    [Column(nameof(NewType), Order = 4, TypeName = "nvarchar(5)")]
    public string? NewType { get; set; }
    [Column(nameof(OriginType), Order = 3, TypeName = "nvarchar(15)")]
    public string? OriginType { get; set; }
    [Column(nameof(SerialNumber), Order = 8, TypeName = "nvarchar(20)")]
    public string? SerialNumber { get; set; }
    [Column(nameof(Source), Order = 5, TypeName = "nvarchar(15)")]
    public string? Source { get; set; }
    [Column(nameof(Temperature), Order = 14, TypeName = "real")]
    public double? Temperature { get; set; }
    [Column(nameof(TimeDate), Order = 2, TypeName = "Date")]
    public DateTime? TimeDate { get; set; }

    [Key]
    [Column(nameof(TimeStr), Order = 1, TypeName = "datetime2(0)")]
    public DateTime TimeStr { get; set; }
    [Key]
    [Column(nameof(UserEnrollNumber), Order = 0, TypeName = "bigint")]
    public long UserEnrollNumber { get; set; }
    [Column(nameof(WorkCode), Order = 7, TypeName = "tinyint")]
    public int? WorkCode { get; set; }
    [NotMapped]
    public double WorkingOffset { get; set; }
    [NotMapped]
    public int FingerTime { get; set; }
    [NotMapped]
    public bool Attendance { get; set; }
    [NotMapped]
    public string? UserFullName { get; set; }
    [NotMapped]
    public int? UserIDTitle { get; set; }
    [NotMapped]
    public int? UserIDD { get; set; }
    [NotMapped]
    public string? Desc { get; set; }

}

