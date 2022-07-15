using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Components;

#nullable enable

namespace MESystem.Data.TRACE;

[Table("V_SI_MANUF_TOOL_PART_NO", Schema = "TRACE")]

public partial class vSiManufacturingToolPart : ComponentBase
{
    [Column("ID")]
    public int? Id { get; set; }
    [Column("MANUFACTURING_TOOL_ID")]
    public int ManufToolId { get; set; }
    [Column("DEVICE_NO")]
    public string DeviceNo { get; set; }
    [Column("JIG_DESCRIPTION")]
    public string? JigDescription { get; set; }
    [Column("FAMILY")]
    public string? Family { get; set; }
    [Column("PART_NO")]
    public string? PartNo { get; set; }
    [Column("DESCRIPTION")]
    public string? Description { get; set; }
}