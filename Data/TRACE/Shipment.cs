using System.ComponentModel.DataAnnotations.Schema;

namespace MESystem.Data.TRACE
{
    [Table("PACKING_MASTER_LIST", Schema = "TRACE")]
    public class Shipment
    {
        [Column("PART_NO")]
        public string? PartNo { get; set; }

        public Shipment(
                        string? poNo = "",
                        string? partNo = "",
                        string? customerPo = "",
                        string? customerPartNo = "",
                        string? partDesc = "",
                        string? barcodePallet = "",
                        int cartonQty = 0,
                        int realPalletQty = 0,
                        int shipQty = 0,
                        int totalQty = 0,

                        double net = 0,
                        double gross = 0,
                        string? dimension = "",
                        double cbm = 0,
                        string? shippingAddress = "",
                        string? shipMode = "",
                        int palletQtyStandard = 0)
        {

            PoNo = poNo;
            PartNo = partNo;
            CustomerPo = customerPo;
            CustomerPartNo = customerPartNo;
            PartDesc = partDesc;
            BarcodePallet = barcodePallet;
            CartonQty = cartonQty;
            RealPalletQty = realPalletQty;
            ShipQty = shipQty;
            PoTotalQty = totalQty;
            Net = net;
            Gross = gross;
            Dimension = dimension;
            Cbm = cbm;
            ShippingAddress = shippingAddress;
            ShipMode = shipMode;
            PalletQtyStandard = palletQtyStandard;
        }

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
        private int Idx { get; set; }

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

    }
}
