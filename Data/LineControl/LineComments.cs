using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Components;

#nullable disable

namespace MESystem.Data.LineControl;

[Table("LINE_COMMENTS", Schema = "TRACE")]

public partial class LineComments : ComponentBase
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }
    [Column("LINE_ID")]
    public int LineID { get; set; }
    [Column("DESCRIPTION")]
    public string? Comment { get; set; }
    [Column("DESCRIPTION_EN")]
    public string? CommentEN { get; set; }
}