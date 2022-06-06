using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESystem.Data.TRACE
{
    [Table("SI_MANUF_TOOL_LINK", Schema = "TRACE")]

    public partial class SiManufacturingToolLink : ComponentBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int Id { get; set; }
        [Column("PART_NO")]
        public string PartNo { get; set; }
        [Column("MANUFACTURING_TOOL_ID")]
        public int ManufToolId { get; set; }
    }
}