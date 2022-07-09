using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Components;

#nullable enable

namespace MESystem.Data.TRACE;

[Table("FINAL_RESULT_OF_FG", Schema = "TRACE")]

public partial class FinalResultFg : ComponentBase
{
    [Key]
    [Column("FINAL_RESULT_ID")]
    public int Id { get; set; }
    [Column("STATUS")]
    public string? Status { get; set; }
    [Column("REMARK")]
    public string? Remark { get; set; }
    [Column("BARCODE")]
    public string? Barcode { get; set; }
    [Column("FINAL_RESULT_THROUGH_STATIONS")]
    public string? Result { get; set; }
}