using System.ComponentModel.DataAnnotations.Schema;

namespace MESystem.Data.TRACE;

[Table("DATA_EFFICIENCY", Schema = "TRACE")]
public class EffPlan
{
    [Column("IDX")]
    public int? Idx { get; set; }
    [Column("PLAN_DATE")]
    public DateTime? PlanDate { get; set; }
    [Column("FROM_TIME")]
    public string? FromTime { get; set; }
    [Column("TO_TIME")]
    public string? ToTime { get; set; }
    [Column("MI_LINE")]
    public string? MILine { get; set; }
    [Column("REAL_LINE")]
    public string? RealLine { get; set; }
    [Column("FAMILY")]
    public string? Family { get; set; }
    [Column("CO_NO")]
    public string? CoNo { get; set; }
    [Column("SO_BB")]
    public string? SoBB { get; set; }
    [Column("SO_MI")]
    public string? SoMI { get; set; }
    [Column("PART_NO")]
    public string? PartNo { get; set; }
    [Column("DRAWING_NO")]
    public string? DrawingNo { get; set; }
    [Column("SO_QTY")]
    public int? SoQty { get; set; }
    [Column("UPH")]
    public int? UPH { get; set; }
    [Column("OUTPUT_MI")]
    public int? OutputMI { get; set; }
    [Column("REMAINING")]
    public int? Remaining { get; set; }
    [Column("WORKING_HOUR")]
    public int? WorkingHour { get; set; }
    [Column("WEEK")]
    public string? Week { get; set; }
    [Column("CAL_HOURS")]
    public double? CalHours { get; set; }
    [Column("PLAN_QTY")]
    public int? PlanQty { get; set; }
    [Column("ACTUAL_HOURS")]
    public double? ActualHours { get; set; }
    [Column("REAL_OUTPUT")]
    public int? RealOutput { get; set; }
    [Column("PERCENT")]
    public double? Percent { get; set; }
    [Column("NOTE")]
    public string? Note { get; set; }
    [Column("AREA")]
    public string? Area { get; set; }

    [NotMapped]
    public int Id { get; set; }
    [NotMapped]
    public int? GroupId { get; set; }
    [NotMapped]
    public string Name { get; set; }
    [NotMapped]
    public bool IsGroup { get; set; }
    [NotMapped]
    public string TextCss { get; set; }
    [NotMapped]
    public string BackgroundCss { get; set; }

    public override bool Equals(object obj)
    {
        EffPlan resource = obj as EffPlan;
        return resource != null && resource.Id == Id;
    }
    public override int GetHashCode()
    {
        return Id;
    }
}