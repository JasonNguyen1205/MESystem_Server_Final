using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable enable

namespace MESystem.Data.TRACE
{
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
    }
}