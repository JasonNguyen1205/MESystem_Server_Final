using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Components;

//#nullable enable

namespace MESystem.Data.TRACE;

[Table("V_PRODUCTION_PLAN_SMT", Schema = "TRACE")]

public partial class vProductionPlanSMTdef : ComponentBase
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
    [Column("ORDER_NO")]
    public string? OrderNo { get; set; }
    [Column("OPERATION_NO")]
    public int OperationNo { get; set; }
    [Column("IFS_VERSION")]
    public string? IfsVersion { get; set; }
    [Column("PART_NO")]
    public string? PartNo { get; set; }
    [Column("PART_DESCRIPTION")]
    public string? PartDescription { get; set; }
    [Column("SIDE")]
    public string? Side { get; set; }
    [Column("PCB_PART_NO")]
    public string? PcbPartNo { get; set; }
    [Column("PCB_DESCRIPTION")]
    public string? PcbDescription { get; set; }
    [Column("REVISED_QTY_DUE")]
    public int QtyPlanned { get; set; }
    [Column("NEED_DATE")]
    public DateTime NeedDate { get; set; }
    [Column("TARGET_RUNTIME")]
    public string? TargetRuntime { get; set; }
    [Column("TARGET_PPH_IFS")]
    public int TargetPartsPerHourIFS { get; set; }
    [Column("PLAN_START_TIME")]
    public DateTime? PlannedStartTime { get; set; }
    [Column("PROD_ORDER")]
    public int ProductionOrder { get; set; }
    [Column("NOTE")]
    public string? ProductionNote { get; set; }
}

public partial class TempProductionPlanSMT : ComponentBase
{
    public string? Id { get; set; }
    public int? LineId { get; set; }
    public string? LineDescription { get; set; }
    public string? DepartmentNo { get; set; }
    public string? Department { get; set; }
    public string? OrderNo { get; set; }
    public int OperationNo { get; set; }
    public string? IfsVersion { get; set; }
    public string? PartNo { get; set; }
    public string? PartDescription { get; set; }
    public string? PcbGroup { get; set; }
    public string? Side { get; set; }
    public string? PcbPartNo { get; set; }
    public string? PcbDescription { get; set; }
    public int QtyPlanned { get; set; }
    public DateTime NeedDate { get; set; }
    public string? TargetRuntime { get; set; }
    public int TargetPartsPerHourIFS { get; set; }
    public DateTime? PlannedStartTime { get; set; }
    public int ProductionOrder { get; set; }
    public string? ProductionNote { get; set; }
    public bool? Line1 { get; set; }
    public bool? Line2 { get; set; }
    public bool? Line3 { get; set; }
    public bool? Line4 { get; set; }
    public bool? Aoi { get; set; }
    public bool? Spi { get; set; }
    public int? Stencil { get; set; }
}