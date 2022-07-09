using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Components;

namespace MESystem.Data.SetupInstruction;

[Table("vstencil_overview")]

public partial class vStencilOverview : ComponentBase
{
    [Key]
    [Column("storage_ID")]
    public int StorageID { get; set; }
    [Column("Job_ID")]
    public int JobID { get; set; }
    [Column("group_ID")]
    public int GroupID { get; set; }
    [Column("pcbGroup")]
    public string? PcbGroup { get; set; }
    [Column("side")]
    public string? Side { get; set; }
    [Column("partNo")]
    public string? PartNo { get; set; }
    [Column("description")]
    public string? Description { get; set; }
    [Column("stencil_ID")]
    public int StencilID { get; set; }
    [Column("location")]
    public string? Location { get; set; }
    [Column("cycletime")]
    public int Cycletime { get; set; }
    [Column("panelSize")]
    public int PanelSize { get; set; }
}