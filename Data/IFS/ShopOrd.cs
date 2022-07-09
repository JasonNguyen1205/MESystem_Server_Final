using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Components;

namespace MESystem.Data.IFS;

[Table("SHOP_ORD", Schema = "IFSAPP")]

public partial class ShopOrd : ComponentBase
{
    [Column("ORDER_NO")]
    public string? OrderNo { get; set; }
    [Column("OBJID")]
    public string? Objid { get; set; }
    [Column("OBJVERSION")]
    public string? Objversion { get; set; }

}
