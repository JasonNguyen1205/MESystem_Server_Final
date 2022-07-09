using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Components;

#nullable enable

namespace MESystem.Data.TRACE;

[Table("SI_MANUFACTURING_TOOL", Schema = "TRACE")]

public partial class SiManufacturingTool : ComponentBase
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }
    [Column("INVENTORY_NO")]
    public int? InventoryNo { get; set; }
    [Column("DEVICE_NO")]
    public string? DeviceNo { get; set; }
    [Column("DESCRIPTION")]
    public string? Description { get; set; }
    [Column("TOOL_TYP_ID")]
    public int? TypeId { get; set; }
    [Column("MANUFACTURER")]
    public string? Manufacturer { get; set; }
    [Column("PM_DATE")]
    public DateTime? PmDate { get; set; }
    [Column("NEXT_PM_DATE")]
    public DateTime? NextPmDate { get; set; }
    [Column("RELEASE_DATE")]
    public DateTime? ReleaseDate { get; set; }
    [Column("LOCATION")]
    public string? Location { get; set; }
    [Column("TEST_COUNTER")]
    public int Testcounter { get; set; }
}