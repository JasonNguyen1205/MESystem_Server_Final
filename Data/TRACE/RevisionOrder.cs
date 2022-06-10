using System.ComponentModel.DataAnnotations.Schema;

namespace MESystem.Data.TRACE
{

    public class RevisionOrder
    {
        public string OrderNo { get; set; }
        public string PartNo { get; set; }
        public string CustomerVersionSo { get; set; }
        public string CustomerVersionLastest { get; set; }
        public string ActiveDate { get; set; }
        public int Status { get; set; }
        public string UserConfirm { get; set; }

        public RevisionOrder(string orderNo, string partNo, string customerVersionSo, string customerVersionLastest, string activeDate, int status, string userConfirm)
        {
            OrderNo = orderNo;
            PartNo = partNo;
            CustomerVersionSo = customerVersionSo;
            CustomerVersionLastest = customerVersionLastest;
            ActiveDate = activeDate;
            Status = status;
            UserConfirm = userConfirm;
        }
    }
}
