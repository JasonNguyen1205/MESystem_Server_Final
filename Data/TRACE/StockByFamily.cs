namespace MESystem.Data.TRACE;

public class StockByFamily
{
    public string OrderNo { get; set; }
    public string PartNo { get; set; }
    public string CustomerPartNo { get; set; }
    public string Revision { get; set; }
    public int Stock { get; set; }
    public string Family { get; set; }
    public string Invoice { get; set; }
    public string ProductionDate { get; set; }

    public StockByFamily(string productionDate, string family, string partNo, string customerPartNo, string revision, int stock)
    {
        PartNo=partNo;
        Revision=revision;
        Stock=stock;
        Family=family;
        ProductionDate=productionDate;
        CustomerPartNo=customerPartNo;
    }
}