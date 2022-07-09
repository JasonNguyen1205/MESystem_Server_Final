using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Components;

#nullable enable

namespace MESystem.Data.TRACE;

[Table("V_PRODUCTION_PLAN_IFS", Schema = "TRACE")]

public partial class vProductionPlanIFS : ComponentBase
{
    [Column("DEPARTMENT_NO")]
    public string? DepartmentNo { get; set; }
    [Column("DEPARTMENT")]
    public string? Department { get; set; }
    [Key]
    [Column("ORDER_NO")]
    public string? OrderNo { get; set; }
    [Column("PART_NO")]
    public string? PartNo { get; set; }
    [Column("PART_DESCRIPTION")]
    public string? PartDescription { get; set; }
    [Column("REVISED_QTY_DUE")]
    public int QtyPlanned { get; set; }
    [Column("QTY_BOOKED_IFS")]
    public int QtyBookedIfs { get; set; }
    [Column("QTY_PRODUCED")]
    public int QtyProduced { get; set; }
    [Column("QTY_NOT_BOOKED")]
    public int QtyNotBooked { get; set; }
    [Column("QTY_REMAINING")]
    public int QtyRemaining { get; set; }
    [Column("PERCENT_DONE")]
    public float PercentDone { get; set; }

}