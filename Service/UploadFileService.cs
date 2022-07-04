﻿using DevExpress.Blazor.Internal;
using DevExpress.DataAccess.Native.Web;
using MESystem.Data.TRACE;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.IO;
using Aspose;
using Aspose.Cells.Drawing;
using Aspose.Cells;

using Excel = Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using DevExpress.Spreadsheet;
using Microsoft.JSInterop;
//using Microsoft.VisualStudio.Tools.Applications.Runtime;

namespace MESystem.Data
{
    public class UploadFileService
    {

        ExcelWorksheet? worksheet { get; set; } = null;
        //TraceService? TraceDataService { get; set; }
        private readonly TraceService? TraceService;
        private readonly IWebHostEnvironment Environment;
        private readonly IJSRuntime? JSRuntime;

        public UploadFileService(TraceService? _traceService, IWebHostEnvironment environment, IJSRuntime? jSRuntime)
        {
            TraceService=_traceService;
            Environment=environment;
            JSRuntime=jSRuntime;
        }

        public async Task<List<Shipment>> GetShipments(string path)
        {
            List<Shipment> shipmentList = new List<Shipment>();
            FileInfo fileInfo = new FileInfo(path);
            ExcelPackage.LicenseContext=LicenseContext.NonCommercial;
            using(ExcelPackage excelPackage = new(fileInfo))
            {
                int totalColumn = 0;
                int totalRow = 0;
                worksheet=excelPackage.Workbook.Worksheets.FirstOrDefault();
                if(worksheet!=null)
                {
                    totalColumn=worksheet.Dimension.End.Column;
                    totalRow=worksheet.Dimension.End.Row;


                    for(int row = 2; row<=totalRow; row++)
                    {
                        Shipment shipment = new Shipment();
                        if(worksheet.Cells[row, 1].Value!=null)
                        {
                            for(int col = 1; col<=totalColumn; col++)
                            {
                                if(worksheet.Cells[row, col].Value!=null)
                                {
                                    if(col==1) shipment.PoNo=worksheet.Cells[row, col].Value.ToString();
                                    if(col==2) shipment.PartNo=worksheet.Cells[row, col].Value.ToString();
                                    if(col==3) shipment.CustomerPo=worksheet.Cells[row, col].Value.ToString();
                                    if(col==4) shipment.CustomerPartNo=worksheet.Cells[row, col].Value.ToString();
                                    if(col==5) shipment.PartDesc=worksheet.Cells[row, col].Value.ToString();
                                    if(col==6) shipment.ShipQty=Convert.ToInt32(worksheet.Cells[row, col].Value.ToString());
                                    if(col==7) shipment.ShippingAddress=worksheet.Cells[row, col].Value.ToString();
                                    if(col==8) shipment.ShipMode=worksheet.Cells[row, col].Value.ToString();
                                }

                            }
                            shipmentList.Add(shipment);
                        }



                    }
                }
                return shipmentList;
            }
        }

        public async Task<byte[]> ExportExcelWarehouse(List<Shipment> masterList)
        {
            byte[] bytes = { };
            if(masterList.Count()>0)
            {
                ExcelPackage.LicenseContext=LicenseContext.NonCommercial;
                using(var package = new ExcelPackage())
                {
                    var sheet = package.Workbook.Worksheets.Add("Warehouse");
                    string[] headers = {
                        "PO NO",
                        "PART NO",
                        "CUSTOMER PO",
                        "CUSTOMER PART NO",
                        "PART DESCRIPTION",
                        "SHIP QTY",
                        "SHIP MODE",
                        "SCANNED QTY",
                        "CARTON QTY",
                        "PALLET",
                        "PALLET CAPACITY",
                    };
                    // byte[] bytes = { };
                    // Write headers
                    for(int col = 1; col<=headers.Length; col++)
                    {
                        sheet.Cells[1, col].Value=headers[col-1];
                    }

                    for(int row = 1; row<masterList.Count(); row++)
                    {
                        // Write rows data
                        if(row>1)
                        {
                            for(int col = 1; col<=headers.Length; col++)
                            {
                                if(col==1) sheet.Cells[row, col].Value=masterList[row-1].PoNo;
                                if(col==2) sheet.Cells[row, col].Value=masterList[row-1].PartNo;
                                if(col==3) sheet.Cells[row, col].Value=masterList[row-1].CustomerPo;
                                if(col==4) sheet.Cells[row, col].Value=masterList[row-1].CustomerPartNo;
                                if(col==5) sheet.Cells[row, col].Value=masterList[row-1].PartDesc;
                                if(col==6) sheet.Cells[row, col].Value=masterList[row-1].ShipQty;
                                if(col==7) sheet.Cells[row, col].Value=masterList[row-1].ShipMode;
                                if(col==8) sheet.Cells[row, col].Value=masterList[row-1].RealPalletQty;
                                if(col==9) sheet.Cells[row, col].Value=masterList[row-1].CartonQty;
                                if(col==10) sheet.Cells[row, col].Value=masterList[row-1].TracePalletBarcode;
                                if(col==11) sheet.Cells[row, col].Value=masterList[row-1].PalletQtyStandard;
                            }
                        }
                    }

                    bytes=await package.GetAsByteArrayAsync();

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
            if(masterList.Count()>0)
            {
                ExcelPackage.LicenseContext=LicenseContext.NonCommercial;
                using(var package = new ExcelPackage())
                {
                    var sheet = package.Workbook.Worksheets.Add("Warehouse");
                    string[] headers = {
                    "PO NO",
                    "PART NO",
                    "CUSTOMER PO",
                    "CUSTOMER PART NO",
                    "PART DESCRIPTION",
                    "SHIP QTY",
                    "SHIP MODE",
                    "PALLET CAPACITY",
                    "SCANNED QTY",
                    "PALLET",
                    "NET",
                    "GROSS",
                    "DIMENTION",
                    "CBM" };

                    // Write headers
                    for(int col = 1; col<=headers.Length; col++)
                    {
                        sheet.Cells[1, col].Value=headers[col-1];
                    }

                    for(int row = 1; row<masterList.Count(); row++)
                    {
                        // Write rows data
                        if(row>1)
                        {
                            for(int col = 1; col<=headers.Length; col++)
                            {
                                if(col==1) sheet.Cells[row, col].Value=masterList[row-1].PoNo;
                                if(col==2) sheet.Cells[row, col].Value=masterList[row-1].PartNo;
                                if(col==3) sheet.Cells[row, col].Value=masterList[row-1].CustomerPo;
                                if(col==4) sheet.Cells[row, col].Value=masterList[row-1].CustomerPartNo;
                                if(col==5) sheet.Cells[row, col].Value=masterList[row-1].PartDesc;
                                if(col==6) sheet.Cells[row, col].Value=masterList[row-1].ShipQty;
                                if(col==7) sheet.Cells[row, col].Value=masterList[row-1].ShipMode;
                                if(col==8) sheet.Cells[row, col].Value=masterList[row-1].PalletQtyStandard;
                                if(col==9) sheet.Cells[row, col].Value=masterList[row-1].RealPalletQty;
                                if(col==10) sheet.Cells[row, col].Value=masterList[row-1].BarcodePallet;
                                if(col==11) sheet.Cells[row, col].Value=masterList[row-1].Net;
                                if(col==12) sheet.Cells[row, col].Value=masterList[row-1].Gross;
                                if(col==13) sheet.Cells[row, col].Value=masterList[row-1].Dimension;
                                if(col==14) sheet.Cells[row, col].Value=masterList[row-1].Cbm;
                            }
                        }
                    }

                    bytes=await package.GetAsByteArrayAsync();

                }
            }
            else
            {
                return null;
            }
            return bytes;
        }

        public async Task<byte[]> ExportExcelStock(List<StockByFamily> masterList)
        {
            byte[] bytes = { };
            if(masterList.Count()>0)
            {
                ExcelPackage.LicenseContext=LicenseContext.NonCommercial;
                using(var package = new ExcelPackage())
                {
                    var sheet = package.Workbook.Worksheets.Add("Warehouse");
                    string[] headers = {
                    "PART NO",
                    "CUSTOMER PART NO",
                    "STOCK",
                    "CUSTOMER REVISION",
                    "INVOICE"
                    };

                    // Write headers
                    for(int col = 1; col<=headers.Length; col++)
                    {
                        sheet.Cells[1, col].Value=headers[col-1];
                    }

                    for(int row = 1; row<masterList.Count(); row++)
                    {
                        // Write rows data
                        if(row>1)
                        {
                            for(int col = 1; col<=headers.Length; col++)
                            {
                                if(col==1) sheet.Cells[row, col].Value=masterList[row-1].OrderNo;
                                if(col==2) sheet.Cells[row, col].Value=masterList[row-1].PartNo;
                                if(col==3) sheet.Cells[row, col].Value=masterList[row-1].Stock;
                                if(col==4) sheet.Cells[row, col].Value=masterList[row-1].Revision;
                                if(col==5) sheet.Cells[row, col].Value=masterList[row-1].Invoice;

                            }
                        }
                    }

                    bytes=await package.GetAsByteArrayAsync();

                }
            }
            else
            {
                return null;
            }
            return bytes;
        }

        public async Task<bool> ExportTempShipmentData(List<Shipment> masterList)
        {


            if(masterList.Count()>0)
            {
                ExcelPackage.LicenseContext=LicenseContext.NonCommercial;
                using(var package = new ExcelPackage())
                {
                    var sheet = package.Workbook.Worksheets.Add("SCM");
                    string[] headers = {
                    "NO",
                    "PO",
                    "PART NO",
                    "DESCRIPTION",
                    "PALLET CAPACITY",
                    "NET",
                    "GROSS",
                    "DIMENSION",
                    "CARTONS"
                    };
                    int sumOfPallet = 0;
                    int sumOfPcb = 0;
                    float sumOfNet = 0;
                    float sumOfGross = 0;
                    int sumOfCartons = 0;
                    int numberOfCarton = 0;
                    string tempPartNo = "";

                    // Write headers
                    for(int col = 1; col<=headers.Length; col++)
                    {
                        sheet.Cells[1, col].Value=headers[col-1];
                    }

                    for(int row = 1; row<=masterList.Count(); row++)
                    {
                        // Write rows data
                        if(row>1)
                        {
                            string rowPartNo = masterList[row-1].PartNo.ToString();
                            if(!tempPartNo.Equals(rowPartNo))
                            {
                                IEnumerable<FinishedGood> finishedGood = await TraceService.GetPalletContentInfoByPartNo(masterList[row-1].PartNo.ToString());
                                if(finishedGood.Count()==1&&numberOfCarton!=int.Parse(finishedGood.FirstOrDefault().QtyPallet.ToString()))
                                {
                                    int QtyPallet = finishedGood.FirstOrDefault().QtyPallet;
                                    int QtyBox = masterList[row-1].PalletQtyStandard;
                                    numberOfCarton=QtyPallet/QtyBox;
                                }
                                tempPartNo=masterList[row-1].PartNo.ToString();
                            }


                            for(int col = 1; col<=headers.Length; col++)
                            {
                                if(col==1) sheet.Cells[row, col].Value=row-1;
                                if(col==2) sheet.Cells[row, col].Value=masterList[row-1].PoNo;
                                if(col==3) sheet.Cells[row, col].Value=masterList[row-1].PartNo;
                                if(col==4) sheet.Cells[row, col].Value=masterList[row-1].PartDesc;
                                if(col==5)
                                {
                                    sheet.Cells[row, col].Value=masterList[row-1].PalletQtyStandard;
                                    sumOfPcb+=int.Parse(masterList[row-1].PalletQtyStandard.ToString());
                                }
                                if(col==6)
                                {
                                    sheet.Cells[row, col].Value=masterList[row-1].Net;
                                    sumOfNet+=float.Parse(masterList[row-1].Net.ToString());
                                }

                                if(col==7)
                                {
                                    sheet.Cells[row, col].Value=masterList[row-1].Gross;
                                    sumOfGross+=float.Parse(masterList[row-1].Gross.ToString());
                                }

                                if(col==8) sheet.Cells[row, col].Value=masterList[row-1].Dimension;
                                if(col==9)
                                {
                                    sheet.Cells[row, col].Value=numberOfCarton;
                                    sumOfCartons+=numberOfCarton;
                                }

                            }

                        }
                        sumOfPallet++;
                    }

                    // The last row sum
                    if(sumOfPallet+1==masterList.Count()+1)
                    {
                        for(int col = 1; col<=headers.Length; col++)
                        {
                            if(col==1) sheet.Cells[sumOfPallet+1, col].Value="Total";
                            if(col==2) sheet.Cells[sumOfPallet+1, col].Value="";
                            if(col==3) sheet.Cells[sumOfPallet+1, col].Value="";
                            if(col==4) sheet.Cells[sumOfPallet+1, col].Value=sumOfPallet+" pallets";
                            if(col==5) sheet.Cells[sumOfPallet+1, col].Value=sumOfPcb;
                            if(col==6) sheet.Cells[sumOfPallet+1, col].Value=sumOfNet;
                            if(col==7) sheet.Cells[sumOfPallet+1, col].Value=sumOfGross;
                            if(col==8) sheet.Cells[sumOfPallet+1, col].Value="";
                            if(col==9) sheet.Cells[sumOfPallet+1, col].Value=sumOfCartons;
                        }
                    }
                    //bytes = await package.GetAsByteArrayAsync();
                    var path = Path.Combine(Environment.ContentRootPath, "wwwroot", "uploads", $"TempShipment-{DateTime.Now.ToString("dd-MM-yy-HH-mm-ss")}.xlsx");
                    await package.SaveAsAsync(new FileInfo(path));
                    package.Dispose();

                    await PrintWaterMark(path);
                 
                }
            }
            return true;
        }

        public async Task PrintWaterMark(string path)
        {
            // Instantiate a new Workbook
            Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(path);

            // Get the first default sheet
            Aspose.Cells.Worksheet sheet = workbook.Worksheets[0];

            // Add watermark
            Aspose.Cells.Drawing.Shape wordart = sheet.Shapes.AddTextEffect(MsoPresetTextEffect.TextEffect1,
<<<<<<< Updated upstream
            "FRIWO", "Arial Black", 50, false, true
            , 18, 8, 10, 1, 130, 500);
=======
            "FRIWO VIET NAM Co. Ltd\nTemporary Copy", "Arial Black", 50, false, true
            , 18, 8, 1, 1, 130, 800);
>>>>>>> Stashed changes

            // Lock shape aspects
            wordart.IsLocked=true;
            wordart.SetLockedProperty(ShapeLockType.Selection, true);
            wordart.SetLockedProperty(ShapeLockType.ShapeType, true);
            wordart.SetLockedProperty(ShapeLockType.Move, true);
            wordart.SetLockedProperty(ShapeLockType.Resize, true);
            wordart.SetLockedProperty(ShapeLockType.Text, true);

            // Get the fill format of the word art
            FillFormat wordArtFormat = wordart.Fill;

            // Set the transparency
<<<<<<< Updated upstream
            wordArtFormat.Transparency = 0.9;
            var nameFile = $"Watermarked-{DateTime.Now.ToString("dd-MM-yy-HH-mm-ss")}";
            var _path = Path.Combine(Environment.ContentRootPath, "wwwroot", "uploads", nameFile);
            workbook.Save(_path + ".xlsx");
=======
            wordArtFormat.Transparency=0.9;

            var nameFile = $"PKL-{DateTime.Now.ToString("dd-MM-yy-HH-mm-ss")}";
            var _path = Path.Combine(Environment.ContentRootPath, "wwwroot", "uploads", nameFile);
            workbook.Save(_path+".xlsx");
            workbook.Dispose();
>>>>>>> Stashed changes
            File.Delete(path);

            DevExpress.Spreadsheet.Workbook workbooks = new DevExpress.Spreadsheet.Workbook();
            workbooks.LoadDocument(_path+".xlsx", DocumentFormat.Xlsx);
            workbooks.Worksheets.RemoveAt(1);
<<<<<<< Updated upstream
            workbooks.SaveDocument(_path + "-final.xlsx", DocumentFormat.Xlsx);
            File.Delete(_path + ".xlsx");

            string rootFolderPath = Path.Combine(Environment.ContentRootPath, "wwwroot", "uploads");
            string filesToDelete = @"*Watermarked*.xlsx";   // Only delete DOC files containing "DeleteMe" in their filenames
            string[] fileList = System.IO.Directory.GetFiles(rootFolderPath, filesToDelete);

            foreach (string file in fileList)
            {
                if (!file.Contains(nameFile))
                {
                    System.IO.File.Delete(file);
                }
                //System.Diagnostics.Debug.WriteLine(file + "will be deleted");
                //  System.IO.File.Delete(file);
            }


            var proc = new Process();
            proc.StartInfo = new ProcessStartInfo(_path + "-final.xlsx")
=======
            workbooks.SaveDocument(_path+"-final.xlsx", DocumentFormat.Xlsx);
            workbooks.Dispose();
            File.Delete(_path+".xlsx");

            string rootFolderPath = Path.Combine(Environment.ContentRootPath, "wwwroot", "uploads");
            string filesToDelete = @"*PKL*.xlsx";   // Only delete DOC files containing "DeleteMe" in their filenames
            string[] fileList = System.IO.Directory.GetFiles(rootFolderPath, filesToDelete);

            foreach(string file in fileList)
            {
                if(!file.Contains(nameFile))
                {
                    System.IO.File.Delete(file);
                }
                //System.Diagnostics.Debug.WriteLine(file + "will be deleted");
                //  System.IO.File.Delete(file);
            }
            //myWorkbook.Save(_path + "-final.xlsx");
            FileInfo fileInfo = new FileInfo(_path+"-final.xlsx");
            ExcelPackage.LicenseContext=LicenseContext.NonCommercial;
            using(ExcelPackage excelPackage = new(fileInfo))
>>>>>>> Stashed changes
            {
                
                await JSRuntime.InvokeVoidAsync("saveAsFile", $"PKL_{DateTime.Now}.xlsx", Convert.ToBase64String(excelPackage.GetAsByteArray()));
            }
            //FileInfo fi = new FileInfo(_path+"-final.xlsx");
            //if(fi.Exists)
            //{
            //    System.Diagnostics.Process.Start(fi.ToString());
            //}
            //else
            //{
            //    //file doesn't exist


            //}

        }
    }
}

