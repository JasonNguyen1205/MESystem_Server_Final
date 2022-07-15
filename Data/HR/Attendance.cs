using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;
namespace MESystem.Data.HR;

[NotMapped]
public class Attendance
{
    [Column(nameof(UserID), TypeName = "bigint")]
    public long UserID { get; set; }
    [Column(nameof(TimeStr), TypeName = "Date")]
    public DateTime? TimeStr { get; set; } 
    [Column(nameof(TimeDate), TypeName = "Date")]
    public DateTime? TimeDate { get; set; }
    [Column(nameof(TimeIn), TypeName = "Date")]
    public DateTime? TimeIn { get; set; }
    [Column(nameof(TimeOut), TypeName = "Date")]
    public DateTime? TimeOut { get; set; }
    [Column(nameof(FingerTime), TypeName = "int")]
    public int? FingerTime { get; set; }
    [Column(nameof(Checked), TypeName = "bit")]
    public bool Checked => (FingerTime>1&&TimeOffset()>new TimeSpan(0,7,0,0));
    [NotMapped]
    public TimeSpan? Offset => TimeOffset();
  
    public TimeSpan? TimeOffset()
    {
        var rs = TimeOut-TimeIn;
        long temp = 0;
        if(TimeOut!=null&&rs!=null)
            return rs;
        else
            return new TimeSpan(0, 0, 0, 0);
    }
    [Column(nameof(UserFullName), TypeName = "NVARCHAR(100)")]
    public string? UserFullName { get; set; }
    [Column(nameof(UserIDTitle), TypeName = "int")]
    public int? UserIDTitle { get; set; }
    [NotMapped]
    public int? UserIDD { get; set; }
    [NotMapped]
    public string? Desc { get; set; }
}
