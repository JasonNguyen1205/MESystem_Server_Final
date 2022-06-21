using System.ComponentModel.DataAnnotations.Schema;

namespace MESystem.Data.TRACE
{
    [Table("PACKING_MASTER_LIST", Schema = "TRACE")]
    public class Shipment
    {
        [Column("ORDER_NO")]
        public string OrderNo { get; set; }
        [Column("PART_NO")]
        public string PartNo { get; set; }
        [Column("CUSTOMER_PO")]
        public string CustomerPo { get; set; }
        [Column("CUSTOMER_PART_NO")]
        public string CustomerPartNo { get; set; }
        [Column("PART_DESC")]
        public string PartDesc { get; set; }
        [Column("SHIP_QTY")]
        public int ShipQty { get; set; }
        [Column("SHIPPING_ADDRESS")]
        public string ShippingAddress { get; set; }
        [Column("SHIPMODE")]
        public string ShipMode { get; set; }
    }
}
