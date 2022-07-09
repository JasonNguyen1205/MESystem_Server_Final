using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Components;

#nullable enable

namespace MESystem.Data.TRACE;

[Table("V_SI_PRODUCTION_LAYOUTS", Schema = "TRACE")]

public partial class vProductionLayout : ComponentBase
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }
    [Column("PART_NO")]
    public string? PartNo { get; set; }
    [Column("FAMILY")]
    public string? Family { get; set; }
    [Column("DEPARTMENT")]
    public string? Department { get; set; }
    [Column("STATION_ORDER")]
    public int StationOrder { get; set; }
    [Column("STATION")]
    public string? Station { get; set; }
    [Column("CYCLE_TIME_SEC")]
    public float CycleTimeSec { get; set; }
    [Column("ALLOWANCE")]
    public float Allowance { get; set; }
    [Column("STANDARD_TIME")]
    public float StandardTime { get; set; }
    [Column("PANEL_SIZE_MI")]
    public int PanelSizeMi { get; set; }
    [Column("STATION_QTY")]
    public int StationQty { get; set; }
    [Column("STAFF_QTY")]
    public int StaffQty { get; set; }
    [Column("TAKT_TIME_SEC")]
    public float TaktTimeSec { get; set; }
    [Column("BALANCE_TIME")]
    public float BalanceTime { get; set; }
    [Column("UNITS_PER_HOUR")]
    public int UnitsPerHour { get; set; }
    [Column("STATION_STATUS")]
    public int StationStatus { get; set; }
    [Column("USER_ID")]
    public string? UserId { get; set; }
    [Column("LAST_ACTION")]
    public DateTime LastAction { get; set; }
}