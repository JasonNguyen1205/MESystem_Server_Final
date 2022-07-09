using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Components;

#nullable enable

namespace MESystem.Data.TRACE;

[Table("V_SI_PRODUCTS_FAMILYS", Schema = "TRACE")]

public partial class vSiProductFamily : ComponentBase
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }
    [Column("PART_NO")]
    public string? PartNo { get; set; }
    [Column("FAMILY_ID")]
    public int FamilyId { get; set; }
    [Column("FAMILY")]
    public string? Family { get; set; }
}