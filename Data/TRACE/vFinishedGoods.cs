using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using IndexAttribute = Microsoft.EntityFrameworkCore.IndexAttribute;

#nullable enable

namespace MESystem.Data.TRACE;

[Table("V_FINISHED_GOODS", Schema = "TRACE")]
[Index(nameof(BarcodeBox))]
public class vFinishedGoods
{
    [Column("BARCODE_ID")]
    public int BarcodeId { get; set; }
    [Column("BARCODE", TypeName = "varchar(80)")]
    public string? Barcode { get; set; }
    [Column("INTERNAL_BARCODE", TypeName = "varchar(50)")]
    public string? InternalBarcode { get; set; }
    [Key]
    [Column("BARCODE_BOX", TypeName = "varchar(50)")]
    public string? BarcodeBox { get; set; }
    [Column("BOX_NUMBER")]
    public int BoxNumber { get; set; }
    [Column("QUANTITY")]
    public int PanelQty { get; set; }
    [Column("DATE_OF_PACKING")]
    public DateTime? DateOfPackingBox { get; set; }
    [Column("ORDER_NO", TypeName = "varchar(12)")]
    public string? OrderNo { get; set; }
    [Column("PART_NO", TypeName = "varchar(20)")]
    public string? PartNo { get; set; }
    [Column("DAYY", TypeName = "nvarchar2(100)")]
    public string? Day { get; set; }
    [Column("WEEK", TypeName = "nvarchar2(100)")]
    public string? Week { get; set; }
    [Column("MONTHH", TypeName = "nvarchar2(100)")]
    public string? Month { get; set; }
    [Column("YEARR", TypeName = "nvarchar2(100)")]
    public string? Year { get; set; }
    [NotMapped]
    public int QtyCounted { get; set; }
}
