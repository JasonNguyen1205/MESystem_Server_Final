using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Components;

namespace MESystem.Data.ASM;

[Table("LineOverview")]

public partial class ProdData : ComponentBase
{
    [Key]
    [Column("ORDER_NO")]
    public string? OrderNo { get; set; }
    [Column("partNo")]
    public string? PartNo { get; set; }
    [Column("product")]
    public string? PartDescription { get; set; }
    [Column("Line")]
    public string? ProductionLine { get; set; }
    [Column("strsetup")]
    public string? SetupName { get; set; }
    [Column("board")]
    public string? Board { get; set; }
    [Column("side")]
    public string? Side { get; set; }
    [Column("lotSize")]
    public int LotSize { get; set; }
    [Column("producedQTY")]
    public int ProducedQTY { get; set; }
    [Column("prodStartTime")]
    public DateTime StartTime { get; set; }
}
