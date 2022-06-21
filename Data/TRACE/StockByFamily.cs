namespace MESystem.Data.TRACE;

public class StockByFamily
{
    public string OrderNo { get; set; }
    public string PartNo { get; set; }

    public string Revision { get; set; }
    public int Stock { get; set; }
    public string Family { get; set; }
    public string Invoice { get; set; }

    public StockByFamily(string orderNo, string partNo, string revision, int stock, string family, string invoice)
    {
        OrderNo = orderNo;
        PartNo = partNo;
        Revision = revision;
        Stock = stock;
        Family = family;
        Invoice = invoice;
    }
}