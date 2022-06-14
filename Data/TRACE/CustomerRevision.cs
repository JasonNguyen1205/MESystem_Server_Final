namespace MESystem.Data.TRACE;

public class CustomerRevision
{
    public CustomerRevision(string? po,
        string? partNo,
        string? orderNo,
        string? rev,
        string? productFamily = null,
        string? lastestRev = null,
        string? activeDate = null,
        int status = 0,
        string? userConfirm = null,

        int quantity = 0,
        string description = null,
        DateTime confirmDateTime = default,
        string? remark = "")
    {
        PO = po;
        OrderNo = orderNo;
        PartNo = partNo;
        Rev = rev;
        LastestRev = lastestRev;
        ProductFamily = productFamily;

        ActiveDate = activeDate;
        Status = status;
        UserConfirm = userConfirm;
        Description = description;
        Quantity = quantity;
        ConfirmDateTime = confirmDateTime;
        Remark = remark;
    }

    public string PO { get; set; }
    public string OrderNo { get; set; }
    public string PartNo { get; set; }
    public string Rev { get; set; }
    public string LastestRev { get; set; }
    public string ProductFamily { get; set; }
    public string ActiveDate { get; set; }
    public int Status { get; set; }
    public string UserConfirm { get; set; }
    public DateTime ConfirmDateTime { get; set; }
    public int Quantity { get; set; }
    public string Description { get; set; }
    public string Remark { get; set; }
}