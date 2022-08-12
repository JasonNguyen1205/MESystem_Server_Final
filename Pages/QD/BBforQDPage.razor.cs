using MESystem.Service;
using MESystem.Data.TRACE;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using DevExpress.Blazor;
using Microsoft.JSInterop;
using Blazored.Toast.Services;
using System.Text.RegularExpressions;
using DevExpress.BarCodes;
using System.Text;
using System.IO;
using DevExpress.Pdf;
using System.Drawing.Printing;
using System.Printing;

namespace MESystem.Pages.QD;

    public partial class BBforQDPage : ComponentBase
{
    private string selectedNgCode;


    [Inject]
    TraceService? TraceDataService { get; set; }

    public string? FocusElement { get; set; }
    [Inject]
    IJSRuntime? jSRuntime { get; set; }


    [Inject]
    IToastService? Toast { get; set; }

    public string? ReadOnlyElement { get; set; }
    bool packingWithoutBox { get; set; } = false;
    public IEnumerable<FinishedGood>? masterData { get; set; }
    public List<FinishedGood>? localData { get; set; } = new List<FinishedGood>();
    public string SelectedNgCode { get => selectedNgCode; set { selectedNgCode = value; UpdateUI(); } }
    public List<string> NgCodeList { get; set; } = new List<string>();

    public IGrid? Grid { get; set; }
    string? remark { get; set; }
    string? barcodefg { get; set; } = "";

    public bool IsReady { get; set; }
    public IEnumerable<ModelProperties>? partNoData { get; set; }
    public int? numberBox { get; set; } = 0;
    public int? numberFG { get; set; } = 0;
    public string? partNo { get; set; }
    int flag { get; set; }
    public FinishedGood selectedBox { get; set; }
    public FinishedGood barcodeData { get; set; }
    public List<FinishedGood>? fgScannedData { get; set; } = new List<FinishedGood>();

    
    private static Regex re = new Regex("^\\d{7}([-])\\d{7}([-])\\d{6}([-])\\d{3}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    public bool Sound { get; set; } = true;
    protected override async Task OnInitializedAsync()
    {
        //Sound = true;
        //Data = await TraceDataService.GetScrapCode();
        //if (Data.Count() > 0)
        //{

        //    foreach (Scrap r in Data)
        //    {
        //        Rework temp = new();
        //        if (!string.IsNullOrEmpty(r.NGDescriptionVN))
        //        {
        //            NgCodeList.Add(r.NGDescriptionVN);
        //            int? ngNo = int.Parse((r.NG_Description_VN).Split(".")[0]);
        //            temp.NG_Code = ngNo;
        //            temp.NG_Description_VN = (r.NG_Description_VN).Split(".")[1];
        //            if (temp != null)
        //                NgCodeList.Add(temp);
        //        }

        //    }
        //    NgCodeList.Sort();
        //    await UpdateUI();
        //}

        flag = 0;
    }
    

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            ShouldUpdateUI = true;
        }

    }
    public bool ShouldUpdateUI { get; private set; }
    protected override bool ShouldRender() { return ShouldUpdateUI; }
    private async Task UpdateUI()
    {
        ShouldUpdateUI = true;
        //Update UI
        if (ShouldRender())
        {
            await Task.Delay(5);
            await InvokeAsync(StateHasChanged);
            await Task.Delay(5);
            //ShouldUpdateUI = false;
        }

        Console.WriteLine("UI is updated");
    }



    public bool scanBarcode { get; set; } = false;
    public string message { get; set; } = "";
    public int? boxQty { get; set; } = 0;
    public int? currentQty { get; set; } = 0;
    List<string>? Infofield { get; set; } = new();

    List<string>? InfoCssColor { get; set; } = new();

    public List<string>? Result { get; set; } = new List<string>();

    public List<string>? HighlightMsg { get; set; } = new List<string>();

    private async void HandleBarcodeInput(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            if (packingWithoutBox == false)
            {
                await ResetInfo(false);
                barcodeData = new FinishedGood();
                barcodeData = fgScannedData.Where(e => e.Barcode.Contains(barcodefg)).FirstOrDefault();
                if (barcodeData == null)
                {
                    barcodeData = localData.Where(e => e.Barcode.Contains(barcodefg)).FirstOrDefault();
                    if (barcodeData != null)
                    {
                        fgScannedData.Add(barcodeData);
                        barcodefg = "";
                        ReadOnlyElement = "barcodeFG";
                        currentQty = fgScannedData.Count();
                        UpdateInfoField("green", "SUCCESS", $"Load FG Success.");
                    }
                    else
                    {
                        barcodefg = "";
                        UpdateInfoField("red", "ERROR", $"Barcode not in the list partial Box!");
                    }
                }
                else
                {
                    barcodefg = "";
                    UpdateInfoField("red", "ERROR", $"Barcode already scanned!");
                }
            }
            else {
                List<FinishedGood> validFG = new List<FinishedGood>();
                validFG = await TraceDataService.CheckFGPacked(barcodefg);
                if (validFG != null)
                {
                    barcodeData = new FinishedGood();
                    barcodeData = fgScannedData.Where(e => e.Barcode.Contains(barcodefg)).FirstOrDefault();
                    if (barcodeData == null)
                    {
                        fgScannedData.AddRange(validFG);
                        barcodefg = "";
                        ReadOnlyElement = "barcodeFG";
                        currentQty = fgScannedData.Count();
                        UpdateInfoField("green", "SUCCESS", $"Load Box Success.");
                    }
                    else {
                        barcodefg = "";
                        UpdateInfoField("red", "ERROR", $"Barcode already scanned!");
                    }
                }
                else {
                    barcodefg = "";
                    UpdateInfoField("red", "ERROR", $"Wrong Barcode!");
                }
            }
            if (currentQty == boxQty)
            {
                await Packing();
                await Reset();
            }
            else {
                await ResetInfo(true);
            }            
            await UpdateUI();


        }
    }

    private async Task Packing() {
        int maxBox = await TraceDataService.GetMaxBoxNumber(partNo);
        string contentBox = CreateBoxBarcode(partNo, maxBox);        
        foreach (FinishedGood fg in fgScannedData) {            
            await TraceDataService.UpdateBarcodeBoxForFG(fg.Barcode,contentBox,maxBox);            
        }
        PrintLabel(contentBox, "BarcodeBox", "");
        await Reset();
    }

    public string CreateBoxBarcode(string partNo, int maxPallet)
    {
        var myString = maxPallet.ToString();
        //int countString = myString.Length;
        var loop = 10 - myString.Length;
        var resultString = string.Empty;
        resultString += partNo;
        resultString += " B";
        for (var i = 0; i < loop; i++)
        {
            resultString += '0';
        }
        resultString =resultString + myString + "-78";
        _ = jSRuntime.InvokeAsync<string>("ConsoleLog", resultString);
        return resultString;
    }
    async Task ResetInfo(bool backToStart)
    {
        if (backToStart)
        {
           
            
            await jSRuntime.InvokeVoidAsync("focusEditorByID", "barcodeFG");
        }
        else
        {
            await Task.Run(() =>
            {

                Infofield = new();
                InfoCssColor = new();
                Result = new();
                HighlightMsg = new();
            });
        }
        await UpdateUI();

    }
    async void UpdateInfoField(
        string cssTextColor,
        string? result = null,
        string? content = null,
        string? highlightMsg = null,
        bool reset = false)
    {
        if (reset)
        {
            InfoCssColor = new();
            Result = new();
            Infofield = new();
            HighlightMsg = new();
        }

        if (result == "ERROR")
        {
            //await ResetInfo(false);
            if (Sound)
            {
                await jSRuntime.InvokeVoidAsync("playSound", "/sounds/alert.wav");
            }
        }

        if (result == "SUCCESS")
        {
            //await ResetInfo(false);
            if (Sound)
            {
                await jSRuntime.InvokeVoidAsync("playSound", "/sounds/success.mp3");
            }
        }



        if (string.IsNullOrEmpty(cssTextColor) || string.IsNullOrEmpty(content))
        {
            return;
        }

        InfoCssColor.Add(cssTextColor);

        if (result != null)
        {
            Result.Add(result);
        }
        else
        {
            Result.Add("INFO");
        }

        Infofield.Add(content);

        if (highlightMsg != null)
        {
            HighlightMsg.Add(highlightMsg);
        }
        else
        {
            HighlightMsg.Add(string.Empty);
        }

        await UpdateUI();
    }

    private async void HandleOldBoxInput(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await ResetInfo(false);
            if (numberBox == 0 || partNo.Equals(remark.Substring(0, 7)))
            {
                selectedBox = new FinishedGood();
                selectedBox = localData.Where(e => e.BarcodeBox.Contains(remark)).FirstOrDefault();
                if (selectedBox == null)
                {
                    masterData = await TraceDataService.GetFGByBox(remark);
                    if (masterData.Count() > 0)
                    {
                        partNo = remark.Substring(0, 7);                        
                        localData.AddRange(masterData);
                        UpdateInfoField("green", "SUCCESS", $"Load Box Success.");
                        numberBox = numberBox + 1;
                        numberFG = localData.Count();

                    }
                    else
                    {
                        UpdateInfoField("red", "ERROR", $"Box already packed Pallet");
                    }

                }
                else
                {
                    UpdateInfoField("red", "ERROR", $"Barcode already scanned");
                }
            }
            else {
                UpdateInfoField("red", "ERROR", $"Wong Part No");
            }
            remark = "";
            await UpdateUI();
            Console.Write("");
        }
    }

    public string EmployeeId { get; set; } = "";
    private async Task HandleStartBB()
    {
        if(partNo != null) { 
            scanBarcode = true;
            partNoData = await TraceDataService.GetPalletContentInfoByPartNo(partNo);
            boxQty = partNoData.FirstOrDefault().QtyPerBox;
            FocusElement = "barcodeFG";
            ReadOnlyElement = "remark";
            await UpdateUI();
        }
    }

    private async Task HandleReset()
    {
        await Reset();
    }

    private async Task Reset() {
        numberBox = 0;
        numberFG = 0;
        boxQty = 0;
        currentQty = 0;
        fgScannedData.Clear();
        localData.Clear();
        ReadOnlyElement = "barcodeFG";
        FocusElement = "remark";
        await UpdateUI();
    }

    private async Task HandlePacking()
    {
        if(fgScannedData.Count() > 0) { 
            await Packing();
        }
    }

    public async void PrintLabel(string content, string labelType, string selectedPrinter)
    {
        try
        {
            BarCode barCode = new()
            {
                Symbology = Symbology.DataMatrix
            };
            barCode.Options.DataMatrix.ShowCodeText = false;
            barCode.Options.DataMatrix.MatrixSize = DataMatrixSize.MatrixAuto;
            barCode.CodeText = content;
            barCode.Margins.Right = 0;
            barCode.Margins.Top = 1;
            barCode.Margins.Bottom = 0;
            barCode.Margins.Left = 1;
            barCode.BackColor = Color.White;
            barCode.ForeColor = Color.Black;
            barCode.RotationAngle = 0;
            barCode.CodeBinaryData = Encoding.Default.GetBytes(barCode.CodeText);
            barCode.DpiX = 72;
            barCode.DpiY = 72;
            barCode.Module = 0.7f;
            DirectoryInfo info = new($"wwwroot/images/{labelType}.bmp");
            if (info.Exists)
            {
                info.Delete();
            }
            barCode.Save($"wwwroot/images/{labelType}.bmp", System.Drawing.Imaging.ImageFormat.MemoryBmp);
            barCode.Dispose();

            if (labelType == "BarcodeBox")
            {
                using PdfDocumentProcessor processor = new();
                //Find a printer containing 'PDF' in its name.
                var printerServer = new LocalPrintServer();
                var printerName = printerServer.DefaultPrintQueue.FullName;
                //for (int i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
                //{
                //    string pName = PrinterSettings.InstalledPrinters[i];
                //    if (pName.Contains($"{selectedPrinter}"))
                //    {
                //        printerName = pName;
                //        break;
                //    }
                //}
                PdfPrinterSettings printerSettings = new()
                {
                    PrintingDpi = 203,
                    PrintInGrayscale = true
                };
                printerSettings.Settings.DefaultPageSettings.PaperSize = new PaperSize($"{labelType}", 72, 72);
                printerSettings.Settings.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
                printerSettings.Settings.PrinterName = printerName;
                processor.CreateEmptyDocument();

                //page.Size = addSize;
                PdfPage pdfPage = processor.AddNewPage(new PdfRectangle(0, 0, 72, 72));

                //document.Pages.Add(page); using (PdfGraphics graphics = processor.CreateGraphics())
                using (PdfGraphics graphics = processor.CreateGraphics())
                {
                    // Draw a rectangle.
                    //using (var pen = new Pen(Color.Black, 1))
                    // graphics.DrawRectangle(pen, new RectangleF(2, 2, 68, 68));
                    //barCode.Save("wwwroot/images/barcodepallet.png", System.Drawing.Imaging.ImageFormat.Png);
                    //await Task.Delay(500);
                    //barCode.Dispose();
                    Image? img = Image.FromFile($"wwwroot/images/{labelType}.bmp");
                    graphics.DrawImage(img, new PointF(0, 2));

                    // Add graphics content to the document page.
                    graphics.AddToPageForeground(pdfPage, 72, 72);
                    img.Dispose();
                    graphics.Dispose();
                }

                //processor.SaveDocument($"wwwroot/images/{labelType}.pdf");
                processor.Print(printerSettings);
                await Task.Delay(1);
                //BarcodePallet = string.Empty;
                ////PalletLabel.Content = DateTime.Now.ToString();
                ////await PalletLabel.SetParametersAsync(new ParameterView());
                //LabelContent = PalletCode;
                //PalletCode = string.Empty;
                processor.Dispose();
                await UpdateUI();
            }
        }
        catch (Exception)
        {
        }
    }
}

