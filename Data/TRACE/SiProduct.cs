using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Components;

#nullable enable

namespace MESystem.Data.TRACE;

[Table("SI_PRODUCT", Schema = "TRACE")]

public partial class SiProduct : ComponentBase
{
    [Key]
    //[Column("ID")]
    //public int Id { get; set; }
    [Column("PART_NO")]
    public string? PartNo { get; set; }
    //[Column("FAMILY_ID")]
    //public int FamilyId { get; set; }
}