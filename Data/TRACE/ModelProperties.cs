using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Components;

#nullable enable

namespace MESystem.Data.TRACE;

[Table("MODEL_PROPERTIES", Schema = "TRACE")]

public partial class ModelProperties : ComponentBase
{
    [Key]
    [Column("IDX")]
    public int Id { get; set; }
    [Column("PART_NO")]
    public string? PartNo { get; set; }
    [Column("DESCRIPTION")]
    public string? Description { get; set; }
    [Column("FAMILY")]
    public string? Family { get; set; }
    [Column("ROUTING_STATION")]
    public string? Routing_station { get; set; }
    [NotMapped]
    public string? PartNoDescription => $"{PartNo} - {Description}";
    [Column("QUANTITY_PER_BOX")]
    public int QtyPerBox { get; set; }
}