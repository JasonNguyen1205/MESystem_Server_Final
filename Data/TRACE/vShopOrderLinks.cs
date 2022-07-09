using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Components;

#nullable enable

namespace MESystem.Data.TRACE;

[Table("V_SHOP_ORDER_LINK", Schema = "TRACE")]

public partial class vShopOrderLinks : ComponentBase
{
    [Column("ORDER_NO_PCB_C")]
    public string? OrderNoMI { get; set; }
    [Column("ORDER_NO_FA")]
    public string? OrderNo { get; set; }
    [Column("PART_NO")]
    public string? PartNo { get; set; }
    [Column("PART_DESCRIPTION")]
    public string? PartDescription { get; set; }
    [Column("NEED_DATE")]
    public DateTime NeedDate { get; set; }
}