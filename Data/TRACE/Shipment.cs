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

        public Shipment()
        {
        }

        public Shipment(
                        string? poNo = null,
                        string? partNo = null,
                        string? customerPo = null,
                        string? customerPartNo = null,
                        string? partDesc = null,
                        string? bARCODE_PALLET = null,
                        int shipQty = 0,
                        int cartonQty = 0,
                        int realPalletQty = 0,
                        double net = 0,
                        double gross = 0,
                        string? dimension = null,
                        double cbm = 0,
                        string? shippingAddress = null,
                        string? shipMode = null)
        {

            PoNo = poNo;
            PartNo = partNo;
            CustomerPo = customerPo;
            CustomerPartNo = customerPartNo;
            PartDesc = partDesc;
            ShipQty = shipQty;
            CartonQty = cartonQty;
            RealPalletQty = realPalletQty;
            Net = net;
            Gross = gross;
            Dimension = dimension;
            Cbm = cbm;
            ShippingAddress = shippingAddress;
            ShipMode = shipMode;
            BARCODE_PALLET = bARCODE_PALLET;
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
        public int PALLET_QTY_STANDARD { get; set; }

        [Column("BARCODE_PALLET")]
        public string? BARCODE_PALLET { get; set; }

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
