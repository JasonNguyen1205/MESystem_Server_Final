﻿using DevExpress.BarCodes;
using DevExpress.Blazor;
using DevExpress.Pdf;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrintingLinks;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;

using MESystem.Data;
using MESystem.Data.TRACE;
using MESystem.LabelComponents;
using MESystem.Service;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

using System.ComponentModel;
using System.Drawing.Printing;
using System.IO;
using System.Text;

namespace MESystem.Pages.Warehouse;

public partial class Shipping : ComponentBase
{
    [Inject]
    IJSRuntime jSRuntime { get; set; }
    [Inject]
    IfsService? IfsService { get; set; }
    [Inject]
    TraceService? TraceDataService { get; set; }
    [Inject]
    IApiClientService? ApiClientService { get; set; }
    [Inject]
    PalleteLabel? PalleteLabel { get; set; }
    bool FormJustRead = true;
    public bool TextBoxEnabled { get; set; }
    string? Scanfield { get; set; }
    string PoData { get; set; } = string.Empty;
    string Infofield { get; set; } = string.Empty;
    string InfoCssColor { get; set; } = "black";
    string PoNumber { get; set; } = string.Empty;
    string PartNo { get; set; } = string.Empty;
    string PartDescription { get; set; } = string.Empty;
    int RevisedQtyDue { get; set; } = 0;
    int QtyShipped { get; set; } = 0;
    int QtyLeft { get; set; } = 0;
    int QtyOfTotalDevices { get; set; } = 0;
    string QtyCssColor { get; set; } = "white";
    bool CheckQtyPlanned { get; set; }
    public string? Css { get; set; }
    public string? Title { get; set; }
    private CustomerOrder valueprop = new();
    public CustomerOrder Value
    {
        get => valueprop;
        set { valueprop = value; Title = Value == null ? "Making palette" : "Link Box <--> PO No. & Making palette"; StateHasChanged(); }
    }

    private Font? printFont;
    private StreamReader? streamToPrint;
    IEnumerable<CustomerOrder>? SelectedPoNumber { get; set; }
    IEnumerable<CustomerOrder>? CustomerOrderData;
    IEnumerable<FinishedGood>? FinishedGoodData;

    //Scan for making palette only
    int QtyPerBox;
    int PaletteCapacity;
    public int TotalFgs { get; set; }
    public bool isSet { get; set; }
    IEnumerable<FinishedGood>? ScannedBox;
    IEnumerable<FinishedGood>? TotalScannedBox;
    bool withoutPOmode;

    //Canvas for barcode                        
    public string? barcodeImg { get; set; }
    public string? LabelContent { get; set; }
    public string? BarcodePallete { get; set; }
    Page page;
    Stream stream;
    //public string barcodeBase64Img{get;set;}
    //string content;
    //BarcodeWriter writer;

    IEnumerable<FinishedGood>? CheckBarcodeBox { get; set; }
    IEnumerable<ModelProperties>? ModelProperties { get; set; }
    public string? SelectedPartNo { get; set; }
    public string? SelectedSO { get; set; }
    public string? SelectedRevision { get; set; }
    public string? CurrentRevision { get; set; }
    public string? CurrentRev { get; set; }
    public bool? IsPhoenix { get; set; } = false;
    public bool? IsDuplicated { get; set; } = false;
    public bool? IsQlyPartBiggerThanQlyBox { get; set; } = false;
    public int totalPCB { get; set; } = 0;
    public bool? NoShowPhoenix { get; set; } = true;

    string PalleteCode = "";
    protected override async Task OnAfterRenderAsync(bool first)
    {
        if (first)
        {
            CustomerOrderData = await TraceDataService.GetCustomerOrders();
            SelectedPoNumber = CustomerOrderData.Take(0);
            isSet = true;
            withoutPOmode = false;
            TextBoxEnabled = false;
            await InvokeAsync(StateHasChanged);
            ScannedBox = new List<FinishedGood>().AsEnumerable();
            TotalScannedBox = new List<FinishedGood>().AsEnumerable();
            Title = "Making pallete";

        }
    }

    //Enter mode just for making palette
    private async Task EnterWithoutPOMode()
    {
        await Task.Delay(5);
        await jSRuntime.InvokeVoidAsync("focusEditorByID", "PartNoField");

        isSet = false;
        await InvokeAsync(StateHasChanged);
    }

    private async void MoveFirst(KeyboardEventArgs args)
    {
        if (args.Key == "Enter")
        {
            // Check Phoenix
            if (await CheckPhoenix())
            {
                IsPhoenix = true;
                NoShowPhoenix = false;
            }
            else
            {
                IsPhoenix = false;
                NoShowPhoenix = true;
            }
            StateHasChanged();
            await Task.Delay(5);
            SelectedPartNo = await jSRuntime.InvokeAsync<string>("GetValueTextBox", "PartNoField");
            QtyPerBox = Int32.Parse(await TraceDataService.GetQTYperBox(3, SelectedPartNo));
            await jSRuntime.InvokeAsync<string>("SetValueTextBox", "QuantityPerBoxScanField", QtyPerBox);
            await jSRuntime.InvokeVoidAsync("focusEditorByID", "QuantityPerPaletteScanField");
            await InvokeAsync(StateHasChanged);
        }
    }

    private async void MoveNext(KeyboardEventArgs args)
    {
        if (args.Key == "Enter")
        {

            isSet = true;
            TextBoxEnabled = true;
            await Task.Delay(1);
            await InvokeAsync(StateHasChanged);
            await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");


        }
    }

    //Start scan box
    private async void StartJob(KeyboardEventArgs args)
    {
        if (args.Key == "Enter")
        {
            await Task.Delay(5);
            await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
            isSet = true;
            await InvokeAsync(StateHasChanged);
        }
    }
    //Set pallete capacity by PartNo
    private async Task<bool> SetQuantityPerPalette(string value)
    {
        if (int.TryParse(value, out int qty))
        {
            PaletteCapacity = qty;
            await InvokeAsync(StateHasChanged);
            return true;
        }
        else
        {
            return false;
        }
    }

    // Set partNo 
    private async Task<bool> SetPartNo(string value)
    {
        if (value != "")
        {
            SelectedPartNo = value;

            await InvokeAsync(StateHasChanged);
            return true;
        }
        else
        {
            return false;
        }
    }

    //Set pallete capacity by PartNo
    private async Task<bool> SetQuantityPerBox(string value)
    {
        if (int.TryParse(value, out int qty))
        {
            QtyPerBox = qty;
            await InvokeAsync(StateHasChanged);
            return true;
        }
        else
        {
            return false;
        }

    }
    async void GetCustomerPo(CustomerOrder values)
    {
        IsPhoenix = false;
        Title = values == null ? "Making palette" : "Link Box <--> PO No. & Making palette";
        if (values is not null)
        {
            FormJustRead = false;
            TextBoxEnabled = true;
            PoNumber = values.CustomerPoNo;
            PartNo = values.PartNo;
            PartDescription = values.PartDescription;
            RevisedQtyDue = values.RevisedQtyDue;
            QtyShipped = values.QtyShipped;
            QtyLeft = (RevisedQtyDue - QtyShipped);
            PoData = "Part: " + PartNo + " - " + PartDescription;
            Infofield = string.Empty;
            CheckQtyPlanned = true;
            withoutPOmode = false;
            SelectedPartNo = values.PartNo;
            SelectedSO = values.OrderNo;
            QtyPerBox = Int32.Parse(await TraceDataService.GetQTYperBox(3, SelectedPartNo));
            await jSRuntime.InvokeAsync<string>("SetValueTextBox", "PartNoField", SelectedPartNo);
            await jSRuntime.InvokeAsync<string>("SetValueTextBox", "QuantityPerBoxScanField", QtyPerBox);

            await SetPartNo(SelectedPartNo);

            // Check Phoenix
            if (await CheckPhoenix())
            {
                IsPhoenix = true;
                NoShowPhoenix = false;
                try
                {
                    CurrentRevision = await TraceDataService.GetCustomerVersion(2, PoNumber);
                    CurrentRevision = int.Parse(CurrentRevision.Replace("V", "")).ToString();
                }
                catch
                {
                    CurrentRevision = "Not found";

                }
                try
                {

                    SelectedRevision = await TraceDataService.GetCustomerVersion(0, PoNumber);
                }
                catch
                {
                    SelectedRevision = "Not found";
                }
            }
            else
            {
                IsPhoenix = false;
                NoShowPhoenix = true;
            }


        }
        else
        {
            FormJustRead = true;
            TextBoxEnabled = false;
            PoNumber = string.Empty;
            PartNo = string.Empty;
            PartDescription = string.Empty;
            RevisedQtyDue = 0;
            PoData = string.Empty;
            Infofield = string.Empty;
            FinishedGoodData = null;
            withoutPOmode = true;

        }


        
        await Task.Delay(5);
        await InvokeAsync(StateHasChanged);

    }

    private async void OnValueChanged(string newValue)
    {
        QtyLeft = int.Parse(new string(newValue.Where(c => char.IsDigit(c)).ToArray()));
        QtyOfTotalDevices = await TraceDataService.GetQtyOfAddedPoNumbers(PoNumber, PartNo);
        QtyLeft = QtyLeft - QtyOfTotalDevices;
        CheckQtyPlanned = false;
    }

    async void PopupClosing(PopupClosingEventArgs args)
    {
        await Task.Delay(5);
        await jSRuntime.InvokeAsync<string>("focusEditor", "QuantityPerPaletteScanField");
        await jSRuntime.InvokeAsync<string>("SetValueTextBox", "PartNoField", SelectedPartNo);
        //await jSRuntime.InvokeAsync<string>("focusEditor", "ShippingScanfield");
        StateHasChanged();
    }

    private void GetInputfield(string content)
    {
        Scanfield = content;
    }

    private async void HandleInput(KeyboardEventArgs e)
    {
        
        GetInputfield(Scanfield);

        if (e.Key == "Enter")
        {


            if (!string.IsNullOrEmpty(Scanfield.Trim()))
            {
                TextBoxEnabled = false;
                await Task.Delay(5);
                await jSRuntime.InvokeAsync<string>("ShowText", "ShowError", "");
                await Task.Delay(1);
                await InvokeAsync(StateHasChanged);

                var list = ScannedBox.ToList<FinishedGood>();
                var masterList = TotalScannedBox.ToList<FinishedGood>();
                // Check Duplication
                IsDuplicated = masterList.Where(j => j.BarcodeBox == Scanfield).Any();

                if (IsDuplicated == true)
                {
                    TextBoxEnabled = true;
                    Scanfield = "";
                    StateHasChanged();
                    await Task.Delay(5);
                    await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
                    await jSRuntime.InvokeAsync<string>("ShowText", "ShowError", "Box is duplicated");
                    return;
                }
                await GetBoxContent(Scanfield);
                CheckBarcodeBox = await TraceDataService.CheckExistBarcodeBox(Scanfield);
                //CheckBarcodeBox = FinishedGoodData;


                if (!CheckBarcodeBox.Any())
                {
                    await jSRuntime.InvokeAsync<string>("ShowText", "ShowError", "Wrong barcode or Box Barcode already in used!");
                    TextBoxEnabled = true;
                    Scanfield = string.Empty;
                    await Task.Delay(5);
                    await InvokeAsync(StateHasChanged);
                    await Task.Delay(5);
                    await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
                    await Task.Delay(5);
                    await FlashQtyColor();
                    return;
                }

                IsQlyPartBiggerThanQlyBox = CheckBarcodeBox.Count() != QtyPerBox;

                if (IsQlyPartBiggerThanQlyBox == true)
                {
                    await jSRuntime.InvokeAsync<string>("ShowText", "ShowError", "");

                    await jSRuntime.InvokeAsync<string>("ShowText", "ShowError", "This is partial Box!");
                }


                totalPCB += CheckBarcodeBox.Count();
                StateHasChanged();
                if (!withoutPOmode)
                {

                    TextBoxEnabled = false;

                    await Task.Delay(1);
                    //CheckBarcodePallete = new List<FinishedGood>().AsEnumerable();
                    //await jSRuntime.InvokeAsync<string>("ConsoleLog", await TraceDataService.CheckExistBarcodeBox(Scanfield));
                    CheckBarcodeBox = await TraceDataService.CheckExistBarcodeBox(Scanfield);
                    //   await jSRuntime.InvokeAsync<string>("ConsoleLog", CheckBarcodeBox);
                    //s await jSRuntime.InvokeAsync<string>("ConsoleLog", CheckBarcodePallete);
                    //await jSRuntime.InvokeAsync<string>("ConsoleLog", CheckBarcodeBox);

                    // CheckBarcodePallete = new List<FinishedGood>().AsEnumerable();
                    //await jSRuntime.InvokeAsync<string>("ConsoleLog", await TraceDataService.CheckExistBarcodeBox(Scanfield));
                    //CheckBarcodeBox = await TraceDataService.CheckExistBarcodeBox(Scanfield);

                    //   await jSRuntime.InvokeAsync<string>("ConsoleLog", CheckBarcodeBox);
                    //await jSRuntime.InvokeAsync<string>("ConsoleLog", CheckBarcodePallete);
                    //await jSRuntime.InvokeAsync<string>("ConsoleLog", CheckBarcodeBox);
                    if (CheckBarcodeBox.Count() > 0)
                    {

                        var tempBarcodeBox = CheckBarcodeBox.FirstOrDefault();
                        if (IsPhoenix == true)
                        {
                            // Check Revision 
                            var tempRevision = int.Parse((tempBarcodeBox.Barcode).Substring(7, 2));
                            if (ScannedBox.Count() < 1)
                            {
                                //CurrentRevision = await TraceDataService.GetCustomerVersion(2, tempBarcodeBox.BarcodeBox);



                                //SelectedRevision = await TraceDataService.GetCustomerVersion(1, tempBarcodeBox.BarcodeBox);
                                if (SelectedRevision.Equals("null"))
                                {

                                    SelectedRevision = CheckBarcodeBox.FirstOrDefault().Barcode.Substring(7, 2);

                                }
                            }
                            //await jSRuntime.InvokeAsync<string>("ShowText", "CheckPhoenix", SelectedRevision);
                            bool checkRevision = tempRevision == int.Parse(SelectedRevision);
                            if (checkRevision)
                            {
                                list.Add(new FinishedGood { PartNo = tempBarcodeBox.PartNo, BarcodeBox = tempBarcodeBox.BarcodeBox, DateOfPackingBox = tempBarcodeBox.DateOfPackingBox, QtyBox = FinishedGoodData.Select(fgd => fgd.QtyBox).FirstOrDefault() });
                                masterList.Add(new FinishedGood { PartNo = tempBarcodeBox.PartNo, BarcodeBox = tempBarcodeBox.BarcodeBox, DateOfPackingBox = tempBarcodeBox.DateOfPackingBox, QtyBox = FinishedGoodData.Select(fgd => fgd.QtyBox).FirstOrDefault() });
                                ScannedBox = list.AsEnumerable();
                                TotalScannedBox = masterList.AsEnumerable();
                                TotalFgs = TotalFgs + QtyPerBox;

                            }
                            else
                            {
                                await jSRuntime.InvokeAsync<string>("ShowText", "ShowError", "Different Phoenix Rev");
                                TextBoxEnabled = true;
                                Scanfield = string.Empty;
                                await Task.Delay(5);
                                await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
                                await FlashQtyColor();
                                return;

                            }
                        }
                        else
                        {
                            list.Add(new FinishedGood { PartNo = tempBarcodeBox.PartNo, BarcodeBox = tempBarcodeBox.BarcodeBox, DateOfPackingBox = tempBarcodeBox.DateOfPackingBox, QtyBox = FinishedGoodData.Select(fgd => fgd.QtyBox).FirstOrDefault() });
                            masterList.Add(new FinishedGood { PartNo = tempBarcodeBox.PartNo, BarcodeBox = tempBarcodeBox.BarcodeBox, DateOfPackingBox = tempBarcodeBox.DateOfPackingBox, QtyBox = FinishedGoodData.Select(fgd => fgd.QtyBox).FirstOrDefault() });
                            ScannedBox = list.AsEnumerable();
                            TotalScannedBox = masterList.AsEnumerable();
                            TotalFgs = TotalFgs + QtyPerBox;
                        }


                        if (ScannedBox.Count() >= PaletteCapacity)
                        {
                            // var tempBarcodeBox = CheckBarcodeBox.First();
                            int maxPalleteNo = await TraceDataService.GetMaxPaletteNumber(tempBarcodeBox.PartNo);
                            string PalleteCode = CreatePalleteBarcode(tempBarcodeBox.PartNo, maxPalleteNo);
                            TextBoxEnabled = false;


                            // Print Barcode
                            await PrintLabel(PalleteCode, "barcodepallete", "SHARED_PRINTER");

                            BarcodePallete = "images/barcodepallete.pdf";
                            await Task.Delay(0).ContinueWith((t) => ScannedBox = new List<FinishedGood>().AsEnumerable());
                            //Update Finishgood Barcode Pallete
                            await TraceDataService.UpdateFinishedGood(tempBarcodeBox.BarcodeBox, PalleteCode, maxPalleteNo);

                        }


                        //ModelProperties = await TraceDataService.GetFamily("Phoenix", SelectedPartNo);
                        // await jSRuntime.InvokeAsync<string>("ConsoleLog", ModelProperties);


                        // Print Barcode
                        //await PrintLabel(PalleteCode, "barcodepallete", "SHARED_PRINTER");

                        //BarcodePallete = "images/barcodepallete.pdf";

                        //TextBoxEnabled = true;
                        //Scanfield = string.Empty;
                        //await Task.Delay(1);
                        //await jSRuntime.InvokeAsync<string>("focusEditor", "ShippingScanField");

                        //await FlashQtyColor();
                        //SeB part
                        //FinishedGoodData = null;
                        Infofield = "";
                        TextBoxEnabled = false;
                        await Task.Delay(1);
                        //GetBoxContent(Scanfield);
                        QtyOfTotalDevices = await TraceDataService.GetQtyOfAddedPoNumbers(PoNumber, PartNo);
                        TextBoxEnabled = true;
                        Scanfield = "";
                        StateHasChanged();
                        await Task.Delay(5);
                        await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
                        await FlashQtyColor();
                    }
                    else
                    {
                        await jSRuntime.InvokeAsync<string>("ShowText", "ShowError", "Invalid Barcode Box");
                    }
                    //Get Family
                }
                else
                {

                    TextBoxEnabled = false;

                    // CheckBarcodePallete = new List<FinishedGood>().AsEnumerable();
                    //await jSRuntime.InvokeAsync<string>("ConsoleLog", await TraceDataService.CheckExistBarcodeBox(Scanfield));
                    CheckBarcodeBox = await TraceDataService.CheckExistBarcodeBox(Scanfield);
                    //   await jSRuntime.InvokeAsync<string>("ConsoleLog", CheckBarcodeBox);
                    //s await jSRuntime.InvokeAsync<string>("ConsoleLog", CheckBarcodePallete);
                    //await jSRuntime.InvokeAsync<string>("ConsoleLog", CheckBarcodeBox);
                    // Check Duplication

                    if (CheckBarcodeBox.Count() > 0)
                    {

                        var tempBarcodeBox = CheckBarcodeBox.FirstOrDefault();

                        if (IsPhoenix == true)
                        {
                            // Check Revision
                            var tempRevision = int.Parse((tempBarcodeBox.Barcode).Substring(7, 2));
                            if (ScannedBox.Count() < 1)
                            {
                                CurrentRevision = await TraceDataService.GetCustomerVersion(2, tempBarcodeBox.BarcodeBox);



                                SelectedRevision = await TraceDataService.GetCustomerVersion(1, tempBarcodeBox.BarcodeBox);
                                if (SelectedRevision.Equals(""))
                                {

                                    SelectedRevision = CheckBarcodeBox.FirstOrDefault().Barcode.Substring(7, 2);

                                }
                            }

                            bool checkRevision = tempRevision == int.Parse(SelectedRevision);
                            if (checkRevision)
                            {
                                list.Add(new FinishedGood { PartNo = tempBarcodeBox.PartNo, BarcodeBox = tempBarcodeBox.BarcodeBox, DateOfPackingBox = tempBarcodeBox.DateOfPackingBox, QtyBox = FinishedGoodData.Select(fgd => fgd.QtyBox).FirstOrDefault() });
                                masterList.Add(new FinishedGood { PartNo = tempBarcodeBox.PartNo, BarcodeBox = tempBarcodeBox.BarcodeBox, DateOfPackingBox = tempBarcodeBox.DateOfPackingBox, QtyBox = FinishedGoodData.Select(fgd => fgd.QtyBox).FirstOrDefault() });
                                ScannedBox = list.AsEnumerable();
                                TotalScannedBox = masterList.AsEnumerable();
                                TotalFgs = TotalFgs + QtyPerBox;

                            }
                            else
                            {
                                TextBoxEnabled = true;
                                Scanfield = string.Empty;
                                await Task.Delay(5);
                                await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
                                await FlashQtyColor();
                                return;
                            }
                        }
                        else
                        {
                            list.Add(new FinishedGood { PartNo = tempBarcodeBox.PartNo, BarcodeBox = tempBarcodeBox.BarcodeBox, DateOfPackingBox = tempBarcodeBox.DateOfPackingBox, QtyBox = FinishedGoodData.Select(fgd => fgd.QtyBox).FirstOrDefault() });
                            masterList.Add(new FinishedGood { PartNo = tempBarcodeBox.PartNo, BarcodeBox = tempBarcodeBox.BarcodeBox, DateOfPackingBox = tempBarcodeBox.DateOfPackingBox, QtyBox = FinishedGoodData.Select(fgd => fgd.QtyBox).FirstOrDefault() });
                            ScannedBox = list.AsEnumerable();
                            TotalScannedBox = masterList.AsEnumerable();
                            TotalFgs = TotalFgs + QtyPerBox;
                        }


                        if (ScannedBox.Count() >= PaletteCapacity)
                        {
                            // var tempBarcodeBox = CheckBarcodeBox.First();
                            int maxPalleteNo = await TraceDataService.GetMaxPaletteNumber(tempBarcodeBox.PartNo);
                            PalleteCode = CreatePalleteBarcode(tempBarcodeBox.PartNo, maxPalleteNo);
                            TextBoxEnabled = false;


                            // Print Barcode
                            await PrintLabel(PalleteCode, "barcodepallete", "SHARED_PRINTER");

                            BarcodePallete = "images/barcodepallete.pdf";
                            await Task.Delay(0).ContinueWith((t) => ScannedBox = new List<FinishedGood>().AsEnumerable());
                            TextBoxEnabled = true;
                            Scanfield = string.Empty;
                            await Task.Delay(1);
                            await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");

                            await FlashQtyColor();

                            // Update Finishgood Barcode Pallete
                            await TraceDataService.UpdateFinishedGood(tempBarcodeBox.BarcodeBox, PalleteCode, maxPalleteNo);
                        }

                        //ModelProperties = await TraceDataService.GetFamily("Phoenix", SelectedPartNo);
                        // await jSRuntime.InvokeAsync<string>("ConsoleLog", ModelProperties);

                        //Update Finishgood Barcode Pallete
                        //await TraceDataService.UpdateFinishedGood(tempBarcodeBox.BarcodeBox, PalleteCode, maxPalleteNo);

                        // Print Barcode
                        //await PrintLabel(PalleteCode, "barcodepallete", "SHARED_PRINTER");

                        //BarcodePallete = "images/barcodepallete.pdf";

                        //TextBoxEnabled = true;
                        //Scanfield = string.Empty;
                        //await Task.Delay(1);
                        //await jSRuntime.InvokeAsync<string>("focusEditor", "ShippingScanField");

                        //await FlashQtyColor();

                        TextBoxEnabled = true;
                        Scanfield = string.Empty;
                        await Task.Delay(5);
                        await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
                        await FlashQtyColor();

                    }

                }
            }

            else
            {
                await jSRuntime.InvokeAsync<string>("ShowText", "ShowError", "Invalid Barcode Box");
            }
        }
    }
    public async Task<bool> CheckPhoenix()
    {
        //Check Phoenix
        ModelProperties = await TraceDataService.GetFamily("Phoenix", SelectedPartNo);
        if (ModelProperties.Any())
        {
            await jSRuntime.InvokeAsync<string>("ConsoleLog", ModelProperties.FirstOrDefault());
            return true;
        }
        return false;
    }

    public string CreatePalleteBarcode(string partNo, int maxPallete)
    {
        string myString = maxPallete.ToString();
        //int countString = myString.Length;
        int loop = 10 - myString.Length;
        string resultString = "";
        resultString += partNo;
        resultString += " P";
        for (int i = 0; i < loop; i++)
        {
            resultString += '0';
        }
        resultString += myString;
        jSRuntime.InvokeAsync<string>("ConsoleLog", resultString);
        return resultString;
    }

    public async Task PrintLabel(string content, string labelType, string selectedPrinter)
    {
        try
        {
            BarCode barCode = new BarCode();
            barCode.Symbology = Symbology.DataMatrix;
            barCode.Options.DataMatrix.ShowCodeText = false;
            barCode.Options.DataMatrix.MatrixSize = DataMatrixSize.Matrix10x10;
            barCode.CodeText = content;
            barCode.Margins.Right = 0;
            barCode.Margins.Top = 0;
            barCode.Margins.Bottom = 0;
            barCode.Margins.Left = 0;
            barCode.BackColor = Color.White;
            barCode.ForeColor = Color.Black;
            barCode.RotationAngle = 0;
            barCode.CodeBinaryData = Encoding.Default.GetBytes(barCode.CodeText);
            barCode.DpiX = 72;
            barCode.DpiY = 72;
            barCode.Module = 0.6f;
            DirectoryInfo info = new DirectoryInfo($"wwwroot/images/{labelType}.bmp");
            if (info.Exists)
            {
                info.Delete();
            }
            barCode.Save($"wwwroot/images/{labelType}.bmp", System.Drawing.Imaging.ImageFormat.MemoryBmp);
            barCode.Dispose();
           
            if (labelType == "barcodepallete")
            {
                using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
                { // Find a printer containing 'PDF' in its name.
                    string printerName = String.Empty;
                    for (int i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
                    {
                        string pName = PrinterSettings.InstalledPrinters[i];
                        if (pName.Contains($"{selectedPrinter}"))
                        {
                            printerName = pName;
                            break;
                        }
                    }
                    PdfPrinterSettings printerSettings = new PdfPrinterSettings();
                    printerSettings.PrintingDpi = 203;
                    printerSettings.PrintInGrayscale = true;
                    printerSettings.Settings.DefaultPageSettings.PaperSize = new PaperSize($"{labelType}", 72, 72);
                    printerSettings.Settings.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
                    printerSettings.Settings.PrinterName = printerName;
                    processor.CreateEmptyDocument();
                    //page.Size = addSize;
                    PdfPage pdfPage = processor.AddNewPage(new PdfRectangle(0, 0, 72, 72));
                    //document.Pages.Add(page); using (PdfGraphics graphics = processor.CreateGraphics())
                    using (PdfGraphics graphics = processor.CreateGraphics())
                    { // Draw a rectangle.
                      //using (var pen = new Pen(Color.Black, 1))
                      // graphics.DrawRectangle(pen, new RectangleF(2, 2, 68, 68));
                      //barCode.Save("wwwroot/images/barcodepallete.png", System.Drawing.Imaging.ImageFormat.Png);
                      //await Task.Delay(500);
                      //barCode.Dispose();
                        var img = Image.FromFile($"wwwroot/images/{labelType}.bmp");
                        graphics.DrawImage(img, new PointF(5, 5));
                        // Add graphics content to the document page.
                        graphics.AddToPageForeground(pdfPage, 72, 72);
                        img.Dispose();
                        graphics.Dispose();
                    }



                    processor.SaveDocument($"wwwroot/images/{labelType}.pdf");
                    processor.Print(printerSettings);
                    await Task.Delay(1);
                    BarcodePallete = "";
                    //PalleteLabel.Content = DateTime.Now.ToString();
                    //await PalleteLabel.SetParametersAsync(new ParameterView());
                    await InvokeAsync(() => StateHasChanged());
                    LabelContent = PalleteCode;
                    PalleteCode = "";
                    await InvokeAsync(() => StateHasChanged());
                    processor.Dispose();



                }
            } 
        }
        catch (Exception ex)
        {
           
        }
    }



    public static void SaveStreamAsFile(string filePath, Stream inputStream, string fileName)
    {
        DirectoryInfo info = new DirectoryInfo(filePath);
        if (!info.Exists)
        {
            info.Create();
        }

        string path = Path.Combine(filePath, fileName);
        using (FileStream outputFileStream = new FileStream(path, FileMode.Create))
        {
            inputStream.CopyTo(outputFileStream);
        }
    }

    public async Task FlashQtyColor()
    {
        QtyCssColor = "yellow";
        await InvokeAsync(() => { StateHasChanged(); });
        await Task.Delay(500);
        QtyCssColor = "white";
        await InvokeAsync(() => { StateHasChanged(); });
        await Task.Delay(500);
        QtyCssColor = "yellow";
        await InvokeAsync(() => { StateHasChanged(); });
        await Task.Delay(500);
        QtyCssColor = "white";
        await InvokeAsync(() => { StateHasChanged(); });
        await Task.Delay(500);
        QtyCssColor = "yellow";
        await InvokeAsync(() => { StateHasChanged(); });
        await Task.Delay(500);
        QtyCssColor = "white";
        await InvokeAsync(() => { StateHasChanged(); });
    }

    private async Task GetBoxContent(string barcodeBox)
    {
        FinishedGoodData = await TraceDataService.GetBoxContentInformation(barcodeBox, PartNo);
        if (FinishedGoodData.Count() != 0)
        {
            int gtyBox = FinishedGoodData.Select(fgd => fgd.QtyBox).FirstOrDefault();
            if (QtyLeft >= gtyBox)
            {
                await InsertPoNumber();
                QtyLeft = QtyLeft - gtyBox;
                Printing(PoNumber);
                InfoCssColor = "black";
                Infofield = "Box: " + Scanfield + "  linked to PO: " + PoNumber;
            }
            else
            {
                InfoCssColor = "red";
                Infofield = "QTY of box: " + Scanfield + " is higher than shipment request!";
            }
        }
        else
        {
            
            Infofield = "PO and box belongs not together!";
            InfoCssColor = "red";
        }
        await Task.Delay(1);
        TextBoxEnabled = true;
        await InvokeAsync(StateHasChanged);
        await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
        await Task.Delay(1);
    }

    private async Task<bool> InsertPoNumber()
    {
        return await TraceDataService.InsertPurchaseOrderNo(Scanfield, PoNumber);
        //return true;
    }

    // The PrintPage event is raised for each page to be printed.
    private void pd_PrintPage(object sender, PrintPageEventArgs ev)
    {
        float linesPerPage = 0;
        float yPos = 5; //9
        int count = 0;
        //float leftMargin = ev.MarginBounds.Left;
        //float topMargin = ev.MarginBounds.Top;
        float leftMargin = 2; //79
        float topMargin = 0;
        string? line = null;

        // Calculate the number of lines per page.
        linesPerPage = ev.MarginBounds.Height / printFont.GetHeight(ev.Graphics);

        // Iterate over the file, printing each line.
        while (count < linesPerPage && ((line = streamToPrint.ReadLine()) != null))
        {
            yPos = topMargin + (count * printFont.GetHeight(ev.Graphics));
            ev.Graphics.DrawString(line, printFont, Brushes.Black, leftMargin, yPos, new StringFormat());
            count++;
        }
        ev.Graphics.DrawString(PoNumber, printFont, Brushes.Black, leftMargin, yPos, new StringFormat());
        // If more lines exist, print another page.
        if (line != null)
            ev.HasMorePages = true;
        else
            ev.HasMorePages = false;
    }

    // Print the file.
    public void Printing(string content)
    {
        try
        {
            //WebClient client = new WebClient();
            //Stream stream = client.OpenRead("https://filesamples.com/samples/document/txt/sample3.txt");
            Stream stream = GenerateStreamFromString(content);
            streamToPrint = new StreamReader(stream);
            try
            {
                printFont = new Font("Arial", 23);
                PrintDocument pd = new PrintDocument();
                pd.PrintPage += new PrintPageEventHandler(pd_PrintPage);
                // Print the document.
                pd.Print();
            }
            finally
            {
                streamToPrint.Close();
            }
        }
        catch (Exception ex)
        {
            string test = ex.ToString();
        }
    }

    private Stream GenerateStreamFromString(string input)
    {
        UTF8Encoding utf8 = new UTF8Encoding();
        MemoryStream stream = new MemoryStream();
        var streamWriter = new StreamWriter(stream);
        streamWriter.WriteLine(input);
        byte[] encodedBytes = utf8.GetPreamble();
        encodedBytes = Encoding.ASCII.GetBytes(streamWriter.ToString());
        stream.Write(encodedBytes, 0, encodedBytes.Length);
        return stream;
    }



}
