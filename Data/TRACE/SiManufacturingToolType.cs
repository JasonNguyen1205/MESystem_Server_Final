using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Components;

#nullable enable

namespace MESystem.Data.TRACE;

[Table("SI_MANUF_TOOL_TYPE", Schema = "TRACE")]

public partial class SiManufacturingToolType : ComponentBase
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }
    [Column("TOOL_TYPE_DESCRIPTION_EN")]
    public string? DescriptionEN { get; set; }
    [Column("TOOL_TYPE_DESCRIPTION_GER")]
    public string? DescriptionGER { get; set; }
    [Column("TOOL_TYPE_DESCRIPTION_VN")]
    public string? DescriptionVN { get; set; }
}