using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Components;



namespace MESystem.Data.LineControl;

[Table("V_LINE_EVENTS_DETAIL", Schema = "TRACE")]

public partial class vLineEvent : ComponentBase
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }
    [Column("ORDER_NO")]
    public string OrderNo { get; set; }
    [Column("PART_NO")]
    public string PartNo { get; set; }
    [Column("PART_DESCRIPTION")]
    public string PartDescription { get; set; }
    [Column("LINE_ID")]
    public int LineId { get; set; }
    [Column("LINE")]
    public string ProductionLine { get; set; }
    [Column("STATE")]
    public string LineState { get; set; }
    [Column("QTY_STAFF")]
    public int QtyStaff { get; set; }
    [Column("QTY_FP")]
    public int QtyFirstPass { get; set; }
    [Column("QTY_PASS")]
    public int QtyPass { get; set; }
    [Column("QTY_FAIL")]
    public int QtyFail { get; set; }
    [Column("SHIFT")]
    public string Shift { get; set; }
    [Column("START_TIME")]
    public DateTime StartTime { get; set; }
    [Column("END_TIME")]
    public DateTime? EndTime { get; set; }
    [Column("COMMENT_ID")]
    public int? CommentId { get; set; }
    [Column("DESCRIPTION")]
    public string? Comment { get; set; }
    [Column("DESCRIPTION_EN")]
    public string? CommentEN { get; set; }
    public vLineEvent(string productionLine, string lineState, DateTime startTime, DateTime endTime)
    {
        (ProductionLine, LineState, StartTime, EndTime)=(productionLine, lineState, startTime, endTime);
    }
}

[Table("LINE_EVENTS", Schema = "TRACE")]

public partial class LineEvent : ComponentBase
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }
    [Column("COMMENT_ID")]
    public int? CommentId { get; set; }
}