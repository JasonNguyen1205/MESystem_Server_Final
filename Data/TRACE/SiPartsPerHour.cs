using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Components;

#nullable enable

namespace MESystem.Data.TRACE;

[Table("SI_PPH", Schema = "TRACE")]

public partial class SiPartsPerHour : ComponentBase
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }
    [Column("PART_NO")]
    public string? partNo { get; set; }
    [Column("DEPARTMENT")]
    public string? Department { get; set; }
    [Column("DEPARTMENT_NO")]
    public string? DepartmentNo { get; set; }
    [Column("WORK_CENTER_NO")]
    public string? workCenterNo { get; set; }
    [Column("TARGET_PPH")]
    public int? targetPPH { get; set; }
}