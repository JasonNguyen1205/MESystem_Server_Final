using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Components;

#nullable enable

namespace MESystem.Data.LineControl;

[Table("V_LAST_STATE", Schema = "TRACE")]

public partial class LastLineState : ComponentBase
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }
    [Column("ORDER_NO")]
    public string? OrderNo { get; set; }
    [Column("PART_NO")]
    public string? PartNo { get; set; }
    [Column("PART_DESCRIPTION")]
    public string? PartDescription { get; set; }
    [Column("REVISED_QTY_DUE")]
    public int QtyDue { get; set; }
    [Column("LINE")]
    public string? ProductionLine { get; set; }
    [Column("STATE")]
    public string? LineState { get; set; }
    [Column("QTY_STAFF")]
    public int QtyStaff { get; set; }
    [Column("QTY_FP")]
    public int QtyFirstPass { get; set; }
    [Column("FPY")]
    public int FPY { get; set; }
    [Column("QTY_PASS")]
    public int QtyPass { get; set; }
    [Column("QTY_FAIL")]
    public int QtyFail { get; set; }
    [Column("START_TIME")]
    public DateTime StartTime { get; set; }
    [Column("PROD_BEGIN")]
    public DateTime ProductionBegin { get; set; }
    [Column("COLOR")]
    public string? StatusColor { get; set; }
    [Column("POTTING_PRODUCT")]
    public int? pottingProduct { get; set; }
}