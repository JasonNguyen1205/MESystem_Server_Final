using System.ComponentModel.DataAnnotations.Schema;

namespace MESystem.Data.HR;

[Table(nameof(RelationDept),Schema ="dbo")]
public class RelationDept
{
    [Column(nameof(ID),TypeName = "int")]
    public int ID { get; set; }
    [Column(nameof(Description), TypeName = "nvarchar(50)")]

    public string? Description { get; set; }
    [Column(nameof(RelationID), TypeName = "int")]

    public int RelationID { get; set; }
    [Column(nameof(LevelID), TypeName = "tinyint")]
    
    public int LevelID { get; set; }
    [Column(nameof(DeptCode), TypeName = "nvarchar(50)")]

    public int DeptCode { get; set; }
    [Column(nameof(TempID), TypeName = "int")]

    public int TempID { get; set; }
    [Column(nameof(TempRelationID), TypeName = "int")]

    public int TempRelationID { get; set; }
    [Column(nameof(IsUsedReport), TypeName = "bit")]

    public bool IsUsedReport { get; set; }
    
}
