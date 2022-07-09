using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Components;

#nullable enable

namespace MESystem.Data.SetupInstruction;

//[Table("mes_productionPlanSMT")]
[Table("vProductionPlanSMT")]
public partial class TProductionPlanSMT : ComponentBase
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
    [Column("QTY_DONE")]
    public int? QtyDone { get; set; }
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
    [Column("SO_ORDER_TYPE")]
    public string? OrderType { get; set; }
    [Column("PERCENT_DONE")]
    public double PercentDone { get; set; }
    [Column("MATERIAL_STATE")]
    public string? MaterialState { get; set; }
    [Column("pcbGroup")]
    public string? PcbGroup { get; set; }
    [Column("L1")]
    public bool Line1 { get; set; }
    [Column("L2")]
    public bool Line2 { get; set; }
    [Column("L3")]
    public bool Line3 { get; set; }
    [Column("L4")]
    public bool Line4 { get; set; }
    [Column("AOI")]
    public bool Aoi { get; set; }
    [Column("JET")]
    public bool Jet { get; set; }
    [Column("THT")]
    public bool Tht { get; set; }
    [Column("MY700")]
    public bool My700 { get; set; }
    [Column("epr")]
    public bool Epr { get; set; }
    [Column("laserArea")]
    public bool LaserArea { get; set; }
    [Column("pcbLaser")]
    public bool PcbLaser { get; set; }
    [Column("spi")]
    public bool Spi { get; set; }
    [Column("stencil")]
    public bool Stencil { get; set; }
}
