namespace MESystem.Data.TRACE
{
    public class Scrap
    {
        public string? ReworkID { get; set; }
        public int? NG_Code { get; set; }
        public string? NGDescriptionEng { get; set; }
        public string? NGDescriptionVN { get; set; }
        public string? Barcode { get; set; }
        public string? CustomerBarcode { get; set; }
        public DateTime? InputDate { get; set; }
        public string? Contract { get; set; }
        public string? PartNo { get; set; }
        public string? OrderNo { get; set; }
        public string? UserId { get; set; }
        public string? Remark { get; set; }

        public Scrap(string? barcode, string? customer_Barcode, int? nG_Code, string? remark, string? part_No, string? order_No, string? user_Id)
        {
            Barcode = barcode;
            CustomerBarcode = customer_Barcode;
            NG_Code = nG_Code;
            Remark = remark;
            PartNo = part_No;
            OrderNo = order_No;
            UserId = user_Id;
        }

        public Scrap(int? nG_Code, string? nG_Description_Eng, string? nG_Description_VN)
        {
            NG_Code = nG_Code;
            NGDescriptionEng = nG_Description_Eng;
            NGDescriptionVN = nG_Description_VN;
        }

        public Scrap()
        {

        }

        public Scrap(string? nG_Description_VN)
        {
            NGDescriptionVN = nG_Description_VN;
        }

        public Scrap(int? nG_Code)
        {
            NG_Code = nG_Code;
        }
    }
}
