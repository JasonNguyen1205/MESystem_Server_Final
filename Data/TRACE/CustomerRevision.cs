namespace MESystem.Data.TRACE;

public class CustomerRevision
{
    //For choosing the CV from shipping form
    public CustomerRevision(string rev, int quantity)
    {
        Rev=rev;
        Quantity=quantity;
    }

    public CustomerRevision(string? po,
        string? partNo,
        string? orderNo,
        string? rev,
        string? productFamily = null,
        string? latestRev = null,
        string? activeDate = null,
        int status = 0,
        string? userConfirm = null,

        int quantity = 0,
        string description = null,
        DateTime confirmDateTime = default,
        string? remark = "")
    {
        PO=po;
        OrderNo=orderNo;
        PartNo=partNo;
        Rev=rev;
        LatestRev=latestRev;
        ProductFamily=productFamily;
        ActiveDate=activeDate;
        Status=status;
        UserConfirm=userConfirm;
        Description=description;
        Quantity=quantity;
        ConfirmDateTime=confirmDateTime;
        Remark=remark;
    }

    public string? PO { get; set; }

    public CustomerRevision(string? pO, string? partNo = null, int quantity = 0)
    {
        PO=pO;
        PartNo=partNo;
        Quantity=quantity;
    }

    public string? OrderNo { get; set; }
    public string? PartNo { get; set; }
    public string? Rev { get; set; }
    public string? LatestRev { get; set; }
    public string? ProductFamily { get; set; }
    public string? ActiveDate { get; set; }
    public CustomerRevision(int status, DateTime confirmDateTime)
    {
        this.Status=status;
        this.ConfirmDateTime=confirmDateTime;

    }
    public int Status { get; set; }
    public string? UserConfirm { get; set; }
    public DateTime ConfirmDateTime { get; set; }
    public int Quantity { get; set; }
    public string? Description { get; set; }
    public string? Remark { get; set; }
}