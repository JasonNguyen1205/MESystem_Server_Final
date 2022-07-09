using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Components;

#nullable enable

namespace MESystem.Data.TRACE;

[Table("V_PRODUCTION_PLAN", Schema = "TRACE")]

public partial class vProductionPlan : ComponentBase
{
    [Key]
    [Column("ID")]
    public string? Id { get; set; }
    [Column("LINE_ID")]
    public int? LineId { get; set; }
    [Column("LINE_DESCRIPTION")]
    public string? LineDescription { get; set; }
    [Column("DEPARTMENT_NO")]
    public string? DepartmentNo { get; set; }
    [Column("DEPARTMENT")]
    public string? Department { get; set; }
    [Column("WORK_CENTER_NO")]
    public string? WorkCenterNo { get; set; }
    [Column("WORK_CENTER_DESCRIPTION")]
    public string? WorkCenterDescription { get; set; }
    [Column("ORDER_NO")]
    public string? OrderNo { get; set; }
    [Column("OPERATION_NO")]
    public int OperationNo { get; set; }
    [Column("PART_NO")]
    public string? PartNo { get; set; }
    [Column("PART_DESCRIPTION")]
    public string? PartDescription { get; set; }
    [Column("FAMILY")]
    public string? Family { get; set; }
    [Column("REVISED_QTY_DUE")]
    public int QtyPlanned { get; set; }
    [Column("NEED_DATE")]
    public DateTime NeedDate { get; set; }
    [Column("TARGET_RUNTIME")]
    public string? TargetRuntime { get; set; }
    [Column("TARGET_PPH_IFS")]
    public int TargetPartsPerHourIFS { get; set; }
    [Column("TARGET_PPH")]
    public int? TargetPartsPerHour { get; set; }
    [Column("SUGGESTED_PPH")]
    public int suggestedPartsPerHour { get; set; }
    [Column("PLAN_START_TIME")]
    public DateTime? PlannedStartTime { get; set; }
    [Column("PROD_ORDER")]
    public int ProductionOrder { get; set; }
    [Column("NOTE")]
    public string? ProductionNote { get; set; }

    [Column("SO_ORDER_TYPE")]
    public string? OrderType { get; set; }
    [Column("PERCENT_DONE")]
    public float PercentDone { get; set; }
    [Column("MATERIAL_STATE")]
    public string? MaterialState { get; set; }
}