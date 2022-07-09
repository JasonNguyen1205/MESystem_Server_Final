using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Components;

#nullable enable

namespace MESystem.Data.TRACE;

[Table("V_SHOP_ORDER_STATUS_MI", Schema = "TRACE")]
public partial class vShopOrderStateMI : ComponentBase
{
    [Column("DEPARTMENT")]
    public string? Department { get; set; }
    [Column("ORDER_NO")]
    public string? OrderNo { get; set; }
    [Column("PART_NO")]
    public string? PartNo { get; set; }
    [Column("PART_DESCRIPTION")]
    public string? PartDescription { get; set; }
    [Column("REVISED_QTY_DUE")]
    public int QtyPlanned { get; set; }
    [Column("QTY_FINISHED")]
    public int QtyFinished { get; set; }
    [Column("REMAINING_QTY")]
    public int QtyRemaining { get; set; }
    [Column("NEED_DATE")]
    public DateTime NeedDate { get; set; }
    [Column("START_DATE")]
    public DateTime? StartDate { get; set; }
}