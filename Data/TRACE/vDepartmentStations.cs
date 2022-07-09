using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Components;

#nullable enable

namespace MESystem.Data.TRACE;

[Table("V_SI_DEPARTMENTS_STATIONS", Schema = "TRACE")]

public partial class vDepartmentStation : ComponentBase
{
    [Column("DEPARTMENT_ID")]
    public int DepartmentId { get; set; }
    [Column("DEPARTMENT")]
    public string? Department { get; set; }
    [Column("STATION")]
    public string? Station { get; set; }
    [Key]
    [Column("STATION_ID")]
    public int StationId { get; set; }
}