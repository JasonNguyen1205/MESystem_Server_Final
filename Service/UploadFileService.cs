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
                for (int row = 1; row <= totalRow; row++)
                {
                    if (row > 2)
                    {
                        Shipment shipment = new Shipment();
                        for (int col = 1; col <= totalColumn; col++)
                        {
                            if (worksheet.Cells[row, col].Value != null)
                            {
                                if (col == 1) shipment.OrderNo = worksheet.Cells[row, col].Value.ToString();
                                if (col == 2) shipment.PartNo = worksheet.Cells[row, col].Value.ToString();
                                if (col == 3) shipment.CustomerPo = worksheet.Cells[row, col].Value.ToString();
                                if (col == 4) shipment.CustomerPartNo = worksheet.Cells[row, col].Value.ToString();
                                if (col == 5) shipment.PartDesc = worksheet.Cells[row, col].Value.ToString();
                                if (col == 6) shipment.ShipQty = Convert.ToInt32(worksheet.Cells[row, col].Value.ToString());
                                if (col == 7) shipment.ShippingAddress = worksheet.Cells[row, col].Value.ToString();
                                if (col == 8) shipment.ShipMode = worksheet.Cells[row, col].Value.ToString();
                            }

                        }
                        shipmentList.Add(shipment);
                    }

                }
                return shipmentList;
            }
        }

        public async Task<byte[]> ExportExcelWarehouse(List<Shipment> masterList)
        {
            byte[] bytes = { };
            if (masterList.Count() > 0)
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    var sheet = package.Workbook.Worksheets.Add("Warehouse");
                    string[] headers = { "PO_NO", "PART_NO", "CUSTOMER_PO", "CUSTOMER_PART_NO", "SHIP_QTY", "SHIP_MODE", "SCANNED_QTY", "PALLET" };
                    // byte[] bytes = { };
                    // Write headers
                    for (int col = 1; col <= headers.Length; col++)
                    {
                        sheet.Cells[1, col].Value = headers[col - 1];
                    }

                    for (int row = 1; row < masterList.Count(); row++)
                    {
                        // Write rows data
                        if (row > 1)
                        {
                            for (int col = 1; col <= headers.Length; col++)
                            {
                                if (col == 1) sheet.Cells[row, col].Value = masterList[row - 1].PoNo;
                                if (col == 2) sheet.Cells[row, col].Value = masterList[row - 1].PartNo;
                                if (col == 3) sheet.Cells[row, col].Value = masterList[row - 1].CustomerPo;
                                if (col == 4) sheet.Cells[row, col].Value = masterList[row - 1].CustomerPartNo;
                                if (col == 5) sheet.Cells[row, col].Value = masterList[row - 1].ShipQty;
                                if (col == 6) sheet.Cells[row, col].Value = masterList[row - 1].ShipMode;
                                if (col == 7) sheet.Cells[row, col].Value = masterList[row - 1].RealPalletQty;
                                if (col == 8) sheet.Cells[row, col].Value = masterList[row - 1].BARCODE_PALLET;
                            }
                        }
                    }

                    bytes = await package.GetAsByteArrayAsync();

                }
            }
            else
            {
                return null;
            }

            return bytes;
        }

        public async Task<byte[]> ExportExcelSCM(List<Shipment> masterList)
        {
            byte[] bytes = { };
            if (masterList.Count() > 0)
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    var sheet = package.Workbook.Worksheets.Add("Warehouse");
                    string[] headers = { "PO_NO", "PART_NO", "CUSTOMER_PO", "CUSTOMER_PART_NO", "SHIP_QTY", "SHIP_MODE", "SCANNED_QTY", "PALLET", "NET", "GROSS", "DIMENTION","CBM"};

                    // Write headers
                    for (int col = 1; col <= headers.Length; col++)
                    {
                        sheet.Cells[1, col].Value = headers[col - 1];
                    }

                    for (int row = 1; row < masterList.Count(); row++)
                    {
                        // Write rows data
                        if (row > 1)
                        {
                            for (int col = 1; col <= headers.Length; col++)
                            {
                                if (col == 1) sheet.Cells[row, col].Value = masterList[row - 1].PoNo;
                                if (col == 2) sheet.Cells[row, col].Value = masterList[row - 1].PartNo;
                                if (col == 3) sheet.Cells[row, col].Value = masterList[row - 1].CustomerPo;
                                if (col == 4) sheet.Cells[row, col].Value = masterList[row - 1].CustomerPartNo;
                                if (col == 5) sheet.Cells[row, col].Value = masterList[row - 1].ShipQty;
                                if (col == 6) sheet.Cells[row, col].Value = masterList[row - 1].ShipMode;
                                if (col == 7) sheet.Cells[row, col].Value = masterList[row - 1].RealPalletQty;
                                if (col == 8) sheet.Cells[row, col].Value = masterList[row - 1].BARCODE_PALLET;
                                if (col == 9) sheet.Cells[row, col].Value = masterList[row - 1].Net;
                                if (col == 10) sheet.Cells[row, col].Value = masterList[row - 1].Gross;
                                if (col == 11) sheet.Cells[row, col].Value = masterList[row - 1].Dimension;
                                if (col == 12) sheet.Cells[row, col].Value = masterList[row - 1].Cbm;
                            }
                        }
                    }

                    bytes = await package.GetAsByteArrayAsync();

                }
            }
            else
            {
                return null;
            }
            return bytes; 
        }
    }
}
