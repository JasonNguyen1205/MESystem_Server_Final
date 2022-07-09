using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Components;

#nullable enable

namespace MESystem.Data.TRACE;

[Table("PRODUCTION_PLAN_LINES", Schema = "TRACE")]

public partial class ProductionPlanLine : ComponentBase
{
    [Key]
    [Column("ID")]
    public string? Id { get; set; }
    [Column("ORDER_NO")]
    public string? OrderNo { get; set; }
    [Column("DEPARTMENT_NO")]
    public string? DepartmentNo { get; set; }
    [Column("OPERATION_NO")]
    public int OperationNo { get; set; }
    [Column("LINE_ID")]
    public int LineId { get; set; }
    [Column("PLAN_START_TIME")]
    public DateTime PlannedStartTime { get; set; }
    [Column("PROD_ORDER")]
    public int ProductionOrder { get; set; }
    [Column("MODIFICATION_TIME")]
    public DateTime ModificationTime { get; set; }
    [Column("TARGET_PPH")]
    public int? TargetPPH { get; set; }
}