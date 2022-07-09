using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Components;

#nullable enable

namespace MESystem.Data.IFS;

[Table("FPS_PRODUCTION_PLAN_TAB", Schema = "IFSAPP")]

public partial class ProductionPlanFIS : ComponentBase
{
    [Column("DEPARTMENT_NO")]
    public string? DepartmentNo { get; set; }
    [Column("DEPARTMENT")]
    public string? Department { get; set; }
    [Column("WORK_CENTER_NO")]
    public string? WorkCenterNo { get; set; }
    [Column("STATION")]
    public string? Station { get; set; }
    [Column("CONTRACT")]
    public string? Contract { get; set; }
    [Column("ORDER_NO")]
    public string? OrderNo { get; set; }
    [Column("RELEASE_NO")]
    public string? ReleaseNo { get; set; }
    [Column("SEQUENCE_NO")]
    public string? SequenceNo { get; set; }
    [Column("OPERATION_NO")]
    public int OperationNo { get; set; }
    [Column("PART_NO")]
    public string? PartNo { get; set; }
    [Column("PART_DESCRIPTION")]
    public string? PartDescription { get; set; }
    [Column("REVISED_QTY_DUE")]
    public int QtyPlanned { get; set; }
    [Column("QTY_TILL_FINISH")]
    public int? QtyTillFinish { get; set; }
    [Column("NEED_DATE")]
    public string? NeedDate { get; set; }
    [Column("REMAINING_HOURS")]
    public string? RemainingHours { get; set; }
    [Column("CUMULATED")]
    public string? Cumulated { get; set; }
    [Column("NOTE")]
    public string? Note { get; set; }
    [Column("PREVIOUS_STATION")]
    public string? PreviousStation { get; set; }
    [Column("PREVIOUS_STATE")]
    public string? PreviousState { get; set; }
    [Column("NEXT_STATION")]
    public string? NextStaion { get; set; }
    [Column("OPER_STATUS_CODE")]
    public string? OperStatuscode { get; set; }
    [Column("COMMISSIONING")]
    public string? Commissioning { get; set; }
    [Column("SCHEDULING_NOTE")]
    public string? SchedulingNote { get; set; }
    [Column("PERCENT_DONE")]
    public string? PercentDone { get; set; }
    [Column("OPERATION_PRIORITY")]
    public int? OperationPriority { get; set; }
}