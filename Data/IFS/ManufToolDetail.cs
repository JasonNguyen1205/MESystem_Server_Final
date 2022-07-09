using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Components;

namespace MESystem.Data.IFS;

[Table("MANUF_TOOL_DETAIL_CFV", Schema = "IFSAPP")]

public partial class ManufToolDetail : ComponentBase
{
    [Column("CONTRACT")]
    public string? Contract { get; set; }
    [Column("TOOL_ID")]
    public string? ToolId { get; set; }
    [Column("TOOL_INSTANCE")]
    public string? ToolInstance { get; set; }
    [Column("DESCRIPTION")]
    public string? ToolDescription { get; set; }
    [Column("NOTE_TEXT")]
    public string? NoteText { get; set; }
    [Column("NORMAL_PRODUCTION_LINE")]
    public string? StandardProdLine { get; set; }
    [Column("CF$_BARCODE")]
    public string? Barcode { get; set; }
    [Column("CF$_FOLD")]
    public string? Fold { get; set; }
    [Column("CF$_OUTPUT_DATE")]
    public string? OutputDate { get; set; }
    [Column("CF$_GUELTIG_AB")]
    public string? ReleasedDate { get; set; }
    [Column("CF$_OBERGRENZE")]
    public string? UpperLimit { get; set; }
    [Column("CF$_GERFERTIGTE_MENGE")]
    public string? UsedQuantity { get; set; }
    [Column("OBJID")]
    public string? Objid { get; set; }
    [Column("OBJVERSION")]
    public string? Objversion { get; set; }
}
