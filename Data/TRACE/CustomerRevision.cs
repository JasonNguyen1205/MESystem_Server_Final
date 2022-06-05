namespace MESystem.Data.TRACE;

public class CustomerRevision
{
    public CustomerRevision(string? po, string? partNo, string? orderNo, string? rev, string? productFamily)
    {
        PO = po;
        OrderNo = orderNo;
        PartNo = partNo;
        Rev = rev;
        ProductFamily = productFamily;
    }

    public string PO { get; set; }
    public string OrderNo { get; set; }
    public string PartNo { get; set; }
    public string Rev { get; set; }
    public string ProductFamily { get; set; } 
}