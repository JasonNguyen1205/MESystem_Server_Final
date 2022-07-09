using System.ComponentModel.DataAnnotations.Schema;

namespace MESystem.Data.TRACE;

[Table("IFS_CUSTOMER_ORDER", Schema = "TRACE")]
public class CustomerOrder
{
    //public CustomerOrder(string? customerPoNo, string? orderNo, string? partNo, string? partDescription, int revisedQtyDue, int qtyInvoiced, int qtyShipped, DateTime pLannedDeliveryDate, DateTime pLannedShipDate, string? rev)
    //{
    //    CustomerPoNo = customerPoNo;
    //    OrderNo = orderNo;
    //    PartNos = partNo;
    //    PartDescription = partDescription;
    //    RevisedQtyDue = revisedQtyDue;
    //    QtyInvoiced = qtyInvoiced;
    //    QtyShipped = qtyShipped;
    //    PLannedDeliveryDate = pLannedDeliveryDate;
    //    PLannedShipDate = pLannedShipDate;
    //    Rev = rev;
    //}

    [Column("CUSTOMER_PO_NO")]
    public string? CustomerPoNo { get; set; }
    [Column("ORDER_NO")]
    public string? OrderNo { get; set; }
    [Column("PART_NO")]
    public string? PartNo { get; set; }
    [Column("CATALOG_DESC")]
    public string? PartDescription { get; set; }
    [Column("REVISED_QTY_DUE")]
    public int RevisedQtyDue { get; set; }
    [Column("QTY_INVOICED")]
    public int QtyInvoiced { get; set; }
    [Column("QTY_SHIPPED")]
    public int QtyShipped { get; set; }
    [Column("PLANNED_DELIVERY_DATE")]
    public DateTime PLannedDeliveryDate { get; set; }
    [Column("PLANNED_SHIP_DATE")]
    public DateTime PLannedShipDate { get; set; }

    [NotMapped]
    public string? Rev { get; set; }

}