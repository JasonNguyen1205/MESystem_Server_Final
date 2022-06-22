using System.ComponentModel.DataAnnotations.Schema;

namespace MESystem.Data.TRACE
{
    [Table("PACKING_MASTER_LIST", Schema = "TRACE")]
    public class Shipment
    {
        [Column("ORDER_NO")]
        public string? OrderNo { get; set; }
        [Column("PART_NO")]
        public string? PartNo { get; set; }
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
        public string? PALLET_QTY_STANDARD { get; set; }

        [Column("BARCODE_PALLET")]
        public string? BARCODE_PALLET { get; set; }

        [Column("PALLET_ORDER")]
        public string? PalletOrder { get; set; }

        [Column("PO_NO")]
        public string? PoNo { get; set; }

        [Column("RAW_DATA")]
        public string? RawData { get; set; }

        [Column("IDX")]
        private int Idx { get; set; }

        [Column("WEEK_")]
        public string? Week_ { get; set; }

        [Column("YEAR_")]
        public string? Year_ { get; set; }

        [Column("PO_TOTAL_QTY")]
        public string? PoTotalQty { get; set; }

        [Column("TRACE_PALLET_BARCODE")]
        public string? TracePalletBarcode { get; set; }

        [Column("REAL_PALLET_QTY")]
        public string? RealPalletQty { get; set; }

        [Column("CARTON_QTY")]
        public string? CartonQty { get; set; }

    }
}
