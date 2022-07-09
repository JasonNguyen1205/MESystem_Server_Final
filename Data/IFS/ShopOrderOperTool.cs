using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Components;

namespace MESystem.Data.IFS;

[Table("SHOP_ORDER_OPER_TOOL_CFV", Schema = "IFSAPP")]
public partial class ShopOrderOperTool : ComponentBase
{
    [Column("CONTRACT")]
    public string? Contract { get; set; }
    [Column("ORDER_NO")]
    public string? OrderNo { get; set; }
    [Column("RELEASE_NO")]
    public string? ReleaseNo { get; set; }
    [Column("SEQUENCE_NO")]
    public string? SequenceNo { get; set; }
    [Column("TOOL_ID")]
    public string? ToolId { get; set; }
    [Column("DESCRIPTION")]
    public string? ToolDescription { get; set; }
    [Column("TOOL_QUANTITY")]
    public int ToolsNeeded { get; set; }
    [Column("FOLD")]
    public string? Fold { get; set; }
    [Column("AVAILABLE")]
    public string? ToolsAvailable { get; set; }
    [Column("CF$_TOOL_INSTANCE")]
    public string? ToolsInstance { get; set; }
    [Column("OBJID")]
    public string? Objid { get; set; }
    [Column("OBJVERSION")]
    public string? Objversion { get; set; }
}

public partial class ShopOrderOperToolQty : ComponentBase
{
    public int ToolsTotal { get; set; }
}
