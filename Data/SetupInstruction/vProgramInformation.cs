using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Components;

#nullable disable

namespace MESystem.Data.SetupInstruction;

[Table("vProgramInformation")]

public partial class vProgramInformation : ComponentBase
{
    [Column("id")]
    public int Id { get; set; }
    [Column("partNo")]
    public string PartNo { get; set; }
    [Column("side")]
    public string Side { get; set; }
    [Column("pcbPartNo")]
    public string PcbPartNo { get; set; }
    [Column("pcbDescription")]
    public string PcbDescription { get; set; }
    [Column("pcbGroup")]
    public string PcbGroup { get; set; }
    [Column("L1")]
    public bool? Line1 { get; set; }
    [Column("L2")]
    public bool? Line2 { get; set; }
    [Column("L2M")]
    public bool? Line2m { get; set; }
    [Column("L3")]
    public bool? Line3 { get; set; }
    [Column("L4")]
    public bool? Line4 { get; set; }
    [Column("AOI")]
    public bool? Aoi { get; set; }
    [Column("JET")]
    public bool? Jet { get; set; }
    [Column("THT")]
    public bool? Tht { get; set; }
    [Column("MY700")]
    public bool? My700 { get; set; }
    [Column("epr")]
    public bool? Epr { get; set; }
    [Column("laserArea")]
    public bool? LaserArea { get; set; }
    [Column("pcbLaser")]
    public bool? PcbLaser { get; set; }
    [Column("spi")]
    public bool? Spi { get; set; }
    [Column("stencil")]
    public int Stencil { get; set; }
}