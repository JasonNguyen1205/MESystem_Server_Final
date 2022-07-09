using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MESystem.Data.TRACE;

[Table("PACKING_MASTER_LIST", Schema = "TRACE")]
public class Shipment
{
    [Column("PART_NO")]
    public string? PartNo { get; set; }

    [Column("NET")]
    public double Net { get; set; }

    [Column("GROSS")]
    public double Gross { get; set; }

    [Column("DIMENSION")]
    public string? Dimension { get; set; }

    [Column("CBM")]
    public double Cbm { get; set; }

    [Column("CUSTOMER_PO")]
    public string? CustomerPo { get; set; }

    [Column("CUSTOMER_PART_NO")]
    public string? CustomerPartNo { get; set; }

    [Column("PART_DESC")]
    public string? PartDesc { get; set; }

    [Column("SHIP_QTY")]
    public int ShipQty { get; set; }
    [Column("SHIPPING_ADDRESS")]
    public string? ShippingAddress { get; set; }

    [Column("SHIPMODE")]
    public string? ShipMode { get; set; }

    [Column("PALLET_QTY_STANDARD")]
    public int PalletQtyStandard { get; set; }

    [Column("BARCODE_PALLET")]
    public string? BarcodePallet { get; set; }

    [Column("PALLET_ORDER")]
    public int PalletOrder { get; set; }

    [Column("PO_NO")]
    public string? PoNo { get; set; }

    [Column("RAW_DATA")]
    public int RawData { get; set; }

    [Column("IDX")]
    public int Idx { get; set; }

    [Column("WEEK_")]
    public string? Week_ { get; set; }

    [Column("YEAR_")]
    public string? Year_ { get; set; }

    [Column("PO_TOTAL_QTY")]
    public int PoTotalQty { get; set; }

    [Column("TRACE_PALLET_BARCODE")]
    public string? TracePalletBarcode { get; set; }

    [Column("REAL_PALLET_QTY")]
    public int RealPalletQty { get; set; }

    [Column("CARTON_QTY")]
    public int CartonQty { get; set; }

    [Column("SHIPMENT_ID")]
    public string? ShipmentId { get; set; }

    [Column("PACKING_LIST_ID")]
    public string? PackingListId { get; set; }

    [Column("CONTAINER_NO")]
    public string? ContainerNo { get; set; }

    public string? ETD { get; set; }

    public string? ETA { get; set; }
    [DateTimeEditMask("dd-MMM-yyyy")]
    public DateTime ShippingDate { get; set; }

    public CustomerOrder Implicit => new()
    {
        CustomerPoNo=PoNo,
        PartNo=PartNo
    };
}
