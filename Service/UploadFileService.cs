using DevExpress.Blazor.Internal;
using DevExpress.DataAccess.Native.Web;
using MESystem.Data.TRACE;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.IO;

namespace MESystem.Data
{
    public class UploadFileService
    {
        ExcelWorksheet? worksheet { get; set; } = null;
        public async Task<List<Shipment>> GetShipments(string path)
        {
            List<Shipment> shipmentList = new List<Shipment>();
            FileInfo fileInfo = new FileInfo(path);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new ExcelPackage(fileInfo))
            {
                worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                int totalColumn = worksheet.Dimension.End.Column;
                int totalRow = worksheet.Dimension.End.Row;
                for (int row = 2; row <= totalRow; row++)
                {
                        Shipment shipment = new Shipment();
                        for (int col = 1; col <= totalColumn; col++)
                        {
                            if (worksheet.Cells[row, col].Value != null)
                            {
                                if (col == 1) shipment.PoNo = worksheet.Cells[row, col].Value.ToString();
                                if (col == 2) shipment.PartNo = worksheet.Cells[row, col].Value.ToString();
                                if (col == 3) shipment.CustomerPo = worksheet.Cells[row, col].Value.ToString();
                                if (col == 4) shipment.CustomerPartNo = worksheet.Cells[row, col].Value.ToString();
                                if (col == 5) shipment.PartDesc = worksheet.Cells[row, col].Value.ToString();
                                if (col == 6) shipment.ShipQty = Convert.ToInt32(worksheet.Cells[row, col].Value.ToString());
                                if (col == 7) shipment.ShippingAddress = worksheet.Cells[row, col].Value.ToString();
                                if (col == 8) shipment.ShipMode = worksheet.Cells[row, col].Value.ToString();
                            }
                        shipmentList.Add(shipment);
                    }

                }
                return shipmentList;
            }
        }
    }
}
