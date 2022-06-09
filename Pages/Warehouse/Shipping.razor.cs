using Blazored.Toast.Services;

using DevExpress.BarCodes;
using DevExpress.Blazor;
using DevExpress.Pdf;

using MESystem.Data;
using MESystem.Data.TRACE;
using MESystem.LabelComponents;
using MESystem.Service;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Windows;
using MouseEventArgs = Microsoft.AspNetCore.Components.Web.MouseEventArgs;

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
    PalleteLabel? PalletLabel { get; set; }

    [Inject]
    IToastService Toast { get; set; }


    bool FormJustRead = true;

    public bool TextBoxEnabled { get; set; }

    string? Scanfield { get; set; }

    string PoData { get; set; } = string.Empty;

    List<string>? Infofield { get; set; } = new();

    List<string>? InfoCssColor { get; set; } = new();

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
        set
        {
            valueprop = value;
            Title = Value == null ? "Making palette" : "Link Box <--> PO No. & Making palette";
            StateHasChanged();
        }
    }

    private Font? printFont;
    private StreamReader? streamToPrint;

    IEnumerable<CustomerOrder>? SelectedPoNumber { get; set; }

    IEnumerable<CustomerOrder>? CustomerOrderData { get; set; }

    //Scan for making palette only
    int QtyPerBox;
    int PaletteCapacity;

    public int TotalFgs { get; set; }

    public bool IsReady { get; set; }

    //Just only one partial box
    public bool IsPartial { get; set; }

    IEnumerable<FinishedGood>? ScannedBox;
    IEnumerable<FinishedGood>? TotalScannedBox;
    bool withoutPOmode;

    //Canvas for barcode                        
    public string? barcodeImg { get; set; }

    public string? LabelContent { get; set; }

    public string? BarcodePallet { get; set; }

    IEnumerable<CustomerRevision>? CustomerRevisionsDetail { get; set; }

    IEnumerable<FinishedGood>? CheckBarcodeBox { get; set; }

    public string? SelectedFamily { get; set; }

    public string? SelectedPartNo { get; set; }

    public string? SelectedSO { get; set; }

    public string? FirstRevisionOnPallet { get; set; }

    public string? FirstRevisionOnPO { get; set; }

    public string? CurrentIFSRevision { get; set; }

    public bool IsPhoenix { get; set; } = false;

    public bool? IsDuplicated { get; set; } = false;

    public bool? IsQlyPartBiggerThanQlyBox { get; set; } = false;

    public bool? NoShowPhoenix { get; set; } = true;

    public bool ForcePrint { get; set; }

    public bool ConfirmPallet { get; set; }

    public bool VerifyTextBoxEnabled { get; set; }

    public List<string> Result { get; set; }

    public List<string> HighlightMsg { get; set; }

    public string PalletCode = string.Empty;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            CustomerOrderData = await TraceDataService.GetCustomerOrders();
            InfoCssColor = new();
            Infofield = new();
            Result = new();
            HighlightMsg = new();
            SelectedPoNumber = CustomerOrderData.Take(0);
            IsReady = false;
            withoutPOmode = false;
            TextBoxEnabled = false;
            CheckBarcodeBox = new List<FinishedGood>().AsEnumerable();
            await UpdateUI();
            ScannedBox = new List<FinishedGood>().AsEnumerable();
            TotalScannedBox = new List<FinishedGood>().AsEnumerable();
            Title = "Making pallet";
            CustomerRevisionsDetail = new List<CustomerRevision>().AsEnumerable();

        }
    }

    //Edit quantity
    public async void EditShipmentQty(MouseEventArgs e)
    {
        await Task.Delay(5);
        CheckQtyPlanned = true;
        await UpdateUI();
    }

    //Enter mode just for making palette
    private async Task EnterWithoutPOMode()
    {
        await Task.Delay(5);
        await jSRuntime.InvokeVoidAsync("focusEditorByID", "PartNoField");
        IsReady = false;
        await UpdateUI();
    }


    //Set pallet capacity by PartNo
    private async Task<bool> SetQuantityPerPalette(string value)
    {
        if (int.TryParse(value, out int qty))
        {
            PaletteCapacity = qty;
            await UpdateUI();
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
        if (value != string.Empty)
        {
            SelectedPartNo = value;

            await UpdateUI();
            return true;
        }
        else
        {
            return false;
        }
    }

    //Set pallet capacity by PartNo
    private async Task<bool> SetQuantityPerBox(string value)
    {
        if (int.TryParse(value, out int qty))
        {
            QtyPerBox = qty;
            await UpdateUI();
            return true;
        }
        else
        {
            return false;
        }
    }

    async void ResetInfo(bool backToStart)
    {
        if (backToStart)
        {
            FormJustRead = true;
            TextBoxEnabled = false;
            PoNumber = string.Empty;
            PartNo = string.Empty;
            PartDescription = string.Empty;
            RevisedQtyDue = 0;
            PoData = string.Empty;
            InfoCssColor = new();
            Infofield = new();
            withoutPOmode = false;
            PoNumber = string.Empty;
            PartNo = string.Empty;
            PartDescription = string.Empty;
            RevisedQtyDue = 0;
            QtyShipped = 0;
            QtyLeft = 0;
            PoData = string.Empty;
            CheckQtyPlanned = false;
            withoutPOmode = false;
            SelectedPartNo = string.Empty;
            SelectedSO = string.Empty;

            //Phoenix info will be show for phoenix product
            IsPhoenix = false;
            NoShowPhoenix = true;

            //Update UI
            await UpdateUI();
        }
        else
        {
            Scanfield = string.Empty;
            TextBoxEnabled = true;
            await UpdateUI();
            await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
        }
    }

    async Task UpdateUI()
    {
        //Update UI
        if (ShouldRender())
        {
            await Task.Delay(5);
            await InvokeAsync(StateHasChanged);
        }

        Console.WriteLine("UI is updated");

    }

    //Get CO info from PO number
    async void GetCustomerPo(CustomerOrder values)
    {
        Title = values == null ? "Making pallet" : "Link Box <--> PO No. & Making pallet";

        if (values is not null)
        {
            try
            {
                Value = values;
                FormJustRead = false;
                TextBoxEnabled = true;
                PoNumber = values.CustomerPoNo;
                PartNo = values.PartNo;
                PartDescription = values.PartDescription;
                RevisedQtyDue = values.RevisedQtyDue;
                QtyShipped = values.QtyShipped;
                QtyLeft = (RevisedQtyDue - QtyShipped);
                PoData = "Part: " + PartNo + " - " + PartDescription;
                CheckQtyPlanned = true;
                SelectedPartNo = PartNo;
                SelectedSO = values.OrderNo;
                IsPhoenix = false;
                //Enable verify pallet code
                ConfirmPallet = true;

                //Using for cases making pallet without PO no, such as BOSCH
                withoutPOmode = false;

                //Get info for making pallet
                QtyPerBox = await TraceDataService.GetQtyFromTrace(3, SelectedPartNo);
                PaletteCapacity = await TraceDataService.GetQtyFromTrace(6, SelectedPartNo);
                TextBoxEnabled = true;
                IsReady = true;
                await UpdateUI();
            }
            catch (Exception)
            {
                QtyPerBox = 0;
                Toast.ShowWarning(
                    $"Cannot find the number box/pallet for part no {SelectedPartNo}",
                    "Missing information");
            }

            //Get family
            CustomerRevisionsDetail = await TraceDataService.GetCustomerRevision(
                0,
                $"{PoNumber}",
                string.Empty,
                string.Empty,
                string.Empty);
            if (CustomerRevisionsDetail != null)
            {
                try
                {
                    SelectedFamily = CustomerRevisionsDetail.FirstOrDefault()?.ProductFamily;
                }
                catch (Exception)
                {
                    SelectedFamily = "Not found from IFS";
                    Toast.ShowWarning(
                        $"Cannot find the prod family for part no {SelectedPartNo}",
                        "Missing information");
                }
            }
            else
            {
                SelectedFamily = "Not found from IFS";
                Toast.ShowWarning($"Cannot find the prod family for part no {SelectedPartNo}", "Missing information");
            }

            await GetNeededInfoByFamily(SelectedFamily);
        }
        else
        {
            ResetInfo(true);
        }
    }

    //Check additional information by family
    async Task GetNeededInfoByFamily(string? family = null)
    {
        if (family is null)
        {
            Toast.ShowError("Cannot find family for this PO", "Missing info");
            return;
        }


        // Check Phoenix
        if (family == "Phoenix")
        {
            IsPhoenix = true;
            NoShowPhoenix = false;
            try
            {
                CurrentIFSRevision = CustomerRevisionsDetail.FirstOrDefault()?.Rev;
            }
            catch
            {
                CurrentIFSRevision = "TBD";
                Toast.ShowWarning($"Cannot find the revision for part no {SelectedPartNo}", "Missing information");
            }
            try
            {
                FirstRevisionOnPO = await TraceDataService.GetCustomerVersion(0, PoNumber);
                if (FirstRevisionOnPO == "null") FirstRevisionOnPO = "TBD";
                FirstRevisionOnPallet = "TBD";
            }
            catch
            {
                FirstRevisionOnPO = "TBD";
                FirstRevisionOnPallet = "TBD";
            }
        }
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
        await UpdateUI();
        await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
        await UpdateUI();

    }

    private void GetInputfield(string content) { Scanfield = content; }

    private async void HandleInput(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            IsPartial = false;

            UpdateInfoField("black", null, Scanfield, null, true);
            await UpdateUI();

            #region Make Partial Pallet by scanning barcode

            if (Scanfield == "PartialPallet")
            {
                var list = ScannedBox.ToList<FinishedGood>();
                if (list.Count == 0) return;
                //var tempBarcodeBox = CheckBarcodeBox.First();
                int maxPalletNo = await TraceDataService.GetMaxPaletteNumber(
                    SelectedPartNo);
                string PalletCode = CreatePalletBarcode(SelectedPartNo, maxPalletNo);
                //TextBoxEnabled = false;

                //Update Finishgood Barcode Pallet
                for (var i = 0; i < list.Count(); i++)
                {
                    await TraceDataService.UpdateFinishedGood(list[i].BarcodeBox, PalletCode, maxPalletNo);
                }

                //Print Barcode
                PrintLabel(PalletCode, "barcodepallet", "SHARED_PRINTER");

                BarcodePallet = "images/barcodepallet.pdf";
                await Task.Delay(0).ContinueWith((t) => ScannedBox = new List<FinishedGood>().AsEnumerable());


                UpdateInfoField("green", "PASS", "The partial pallet is created. Barcode is shown below");


                CheckBarcodeBox = new List<FinishedGood>().AsEnumerable();
                FirstRevisionOnPallet = "TBD";
                if (ConfirmPallet)
                {
                    VerifyTextBoxEnabled = true;
                    await UpdateUI();
                    //Goto verify
                    await Task.Delay(1);
                    QtyOfTotalDevices = await TraceDataService.GetQtyOfAddedPoNumbers(PoNumber, PartNo);
                    Scanfield = string.Empty;
                    await UpdateUI();
                    await jSRuntime.InvokeVoidAsync("focusEditorByID", "VerifyScanField");
                    FlashQtyColor(true);
                    return;
                }

                UpdateInfoField("orange", "WARNING", "The pallet is not verifed");

                await Task.Delay(1);
                QtyOfTotalDevices = await TraceDataService.GetQtyOfAddedPoNumbers(PoNumber, PartNo);
                Scanfield = string.Empty;
                await UpdateUI();
                await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
            }
            #endregion

            VerifyTextBoxEnabled = false;
            InfoCssColor = new();
            Infofield = new();
            Result = new();
            HighlightMsg = new();
            TextBoxEnabled = false;
            GetInputfield(Scanfield);
            await UpdateUI();

            if (Scanfield is null)
            {

                UpdateInfoField("red", "ERROR", "Barcode box is empty");

                Scanfield = null;
                TextBoxEnabled = true;
                await UpdateUI();
                await Task.Delay(5);
                await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
                FlashQtyColor(true);
                return;
            }

            if (string.IsNullOrEmpty(Scanfield.Trim()))
            {
                UpdateInfoField("red", $"Empty barcode");
                Scanfield = null;
                TextBoxEnabled = true;
                await UpdateUI();
                await Task.Delay(5);
                await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
                FlashQtyColor(true);
                return;
            }

            if (!withoutPOmode)
            {
                //Next step is making pallet
                CheckBarcodeBox = await TraceDataService.GetBoxContentInformation(Scanfield, SelectedPartNo);

                IsQlyPartBiggerThanQlyBox = CheckBarcodeBox.Count() != QtyPerBox;

                if (IsQlyPartBiggerThanQlyBox == true)
                {

                    UpdateInfoField("orange", "WARNING: Partial box");

                    if (!ScannedBox.Where(_ => _.Partial == true).Any())
                    {
                        IsPartial = true;
                    }
                    else
                    {

                        UpdateInfoField("red", "ERROR", "More than one partial box on this pallet");

                        Scanfield = null;
                        TextBoxEnabled = true;
                        await UpdateUI();
                        await Task.Delay(5);
                        await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
                        FlashQtyColor(true);
                        return;
                    }
                }
                else
                {

                    UpdateInfoField("green", "PASS", "Box is full");

                }

                var rs = await CheckBoxInfoAndPrintPOLabel(Scanfield);
                //Check box and print PO label
                if (!rs)
                {
                    Toast.ShowWarning(Infofield.LastOrDefault(), "Warning");
                    TextBoxEnabled = true;
                    Scanfield = null;
                    await UpdateUI();
                    await Task.Delay(5);
                    await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
                    FlashQtyColor(false);
                    return;
                }

                //Next step is making pallet
                CheckBarcodeBox = await TraceDataService.GetBoxContentInformation(Scanfield, SelectedPartNo);

                UpdateInfoField("green", "PASS", "Check PO");

                if (ScannedBox == null)
                {
                    IsPartial = false;
                    ScannedBox = new List<FinishedGood>().AsEnumerable();
                }

                var list = ScannedBox.ToList<FinishedGood>();
                var masterList = TotalScannedBox.ToList<FinishedGood>();

                #region Box is made pallet check
                var isUsed = await TraceDataService.CheckBoxInAnyPallete(Scanfield);
                if (isUsed != null && !isUsed.Any())
                {
                    Toast.ShowError("Box Barcode already in used!", "Barcode Error");


                    UpdateInfoField("red", "ERROR", $"Box is already packaged in pallet {CheckBarcodeBox.FirstOrDefault().BarcodePalette}");

                    TextBoxEnabled = true;
                    Scanfield = string.Empty;
                    await Task.Delay(5);
                    await InvokeAsync(() => StateHasChanged());
                    await Task.Delay(5);
                    await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
                    await Task.Delay(5);
                    FlashQtyColor(false);
                    return;
                }

                UpdateInfoField("green", "PASS", "Check already scan to pallet");

                #endregion

                // Check Duplication
                IsDuplicated = masterList.Where(j => j.BarcodeBox == Scanfield).Any();

                if (IsDuplicated == true)
                {
                    TextBoxEnabled = true;
                    Scanfield = string.Empty;
                    await UpdateUI();
                    await Task.Delay(5);
                    await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
                    Toast.ShowError("Barcode duplication", "Barcode Error");

                    UpdateInfoField("red", $"ERROR: This box is duplicated");
                    await UpdateUI();


                    return;
                }

                UpdateInfoField("green", "PASS", "Check duplication");



                await UpdateUI();

                if (IsPhoenix == true)
                {

                    UpdateInfoField("green", null, "This is Phoenix product");

                    // Check Revision inside Pallet
                    var tempRevision = CheckBarcodeBox.FirstOrDefault().Rev;
                    if (ScannedBox.Count() < 1)
                    {
                        if (FirstRevisionOnPO.Equals("TBD"))
                        {
                            FirstRevisionOnPO = tempRevision;

                            UpdateInfoField("green", "PASS", "The customer version is set for this PO");

                        }
                        if (FirstRevisionOnPallet.Equals("TBD"))
                        {
                            FirstRevisionOnPallet = tempRevision;

                            UpdateInfoField("green", "PASS", "The customer version is set for this pallet");

                        }
                    }
                    bool checkRevisionPO = tempRevision == FirstRevisionOnPO;
                    if (!checkRevisionPO)
                    {
                        Toast.ShowError("Different Phoenix Rev", "Wrong Rev");
                        TextBoxEnabled = true;
                        Scanfield = string.Empty;
                        await Task.Delay(5);
                        await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
                        FlashQtyColor(false);
                        return;
                    }

                    UpdateInfoField("green", "PASS", "The customer version as same as PO customer version");


                    bool checkRevision = tempRevision == FirstRevisionOnPallet;
                    if (!checkRevision)
                    {
                        Toast.ShowError("Different Phoenix Rev", "Wrong Rev");
                        TextBoxEnabled = true;
                        Scanfield = string.Empty;
                        await Task.Delay(5);
                        await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
                        FlashQtyColor(false);
                        return;
                    }

                    UpdateInfoField("green", "PASS", "The customer version as same as pallet customer version");

                    list.Add(
                        new FinishedGood
                        {
                            PartNo = CheckBarcodeBox.FirstOrDefault().PartNo,
                            BarcodeBox = CheckBarcodeBox.FirstOrDefault().BarcodeBox,
                            DateOfPackingBox = CheckBarcodeBox.FirstOrDefault().DateOfPackingBox,
                            InvoiceNumber = CheckBarcodeBox.FirstOrDefault().InvoiceNumber,
                            QtyBox = CheckBarcodeBox.Count(),
                            Partial = IsPartial
                        });

                    masterList.Add(
                        new FinishedGood
                        {
                            PartNo = CheckBarcodeBox.FirstOrDefault().PartNo,
                            BarcodeBox = CheckBarcodeBox.FirstOrDefault().BarcodeBox,
                            DateOfPackingBox = CheckBarcodeBox.FirstOrDefault().DateOfPackingBox,
                            InvoiceNumber = CheckBarcodeBox.FirstOrDefault().InvoiceNumber,
                            QtyBox = CheckBarcodeBox.Count(),
                            Partial = IsPartial
                        });

                    ScannedBox = list.AsEnumerable();
                    TotalScannedBox = masterList.AsEnumerable();
                    TotalFgs = TotalFgs + CheckBarcodeBox.Count();

                    UpdateInfoField("green", "PASS", "The box is added to buffer for making pallet");

                }
                else
                {
                    list.Add(
                        new FinishedGood
                        {
                            PartNo = CheckBarcodeBox.FirstOrDefault().PartNo,
                            BarcodeBox = CheckBarcodeBox.FirstOrDefault().BarcodeBox,
                            DateOfPackingBox = CheckBarcodeBox.FirstOrDefault().DateOfPackingBox,
                            InvoiceNumber = CheckBarcodeBox.FirstOrDefault().InvoiceNumber,
                            QtyBox = CheckBarcodeBox.Count(),
                            Partial = IsPartial
                        });

                    masterList.Add(
                        new FinishedGood
                        {
                            InvoiceNumber = CheckBarcodeBox.FirstOrDefault().InvoiceNumber,
                            PartNo = CheckBarcodeBox.FirstOrDefault().PartNo,
                            BarcodeBox = CheckBarcodeBox.FirstOrDefault().BarcodeBox,
                            DateOfPackingBox = CheckBarcodeBox.FirstOrDefault().DateOfPackingBox,
                            QtyBox = CheckBarcodeBox.Count(),
                            Partial = IsPartial
                        });

                    ScannedBox = list.AsEnumerable();
                    TotalScannedBox = masterList.AsEnumerable();
                    TotalFgs = TotalFgs + CheckBarcodeBox.Count();
                }

                if (ScannedBox.Count() >= PaletteCapacity)
                {
                    //var tempBarcodeBox = CheckBarcodeBox.First();
                    int maxPalletNo = await TraceDataService.GetMaxPaletteNumber(
                        CheckBarcodeBox.FirstOrDefault().PartNo);
                    string PalletCode = CreatePalletBarcode(
                        CheckBarcodeBox.FirstOrDefault().PartNo, maxPalletNo);
                    //TextBoxEnabled = false;

                    //Update Finishgood Barcode Pallet
                    for (var i = 0; i < list.Count(); i++)
                    {
                        await TraceDataService.UpdateFinishedGood(list[i].BarcodeBox, PalletCode, maxPalletNo);
                    }

                    // Print Barcode
                    PrintLabel(PalletCode, "barcodepallet", "SHARED_PRINTER");

                    BarcodePallet = "images/barcodepallet.pdf";
                    ScannedBox = new List<FinishedGood>().AsEnumerable();


                    UpdateInfoField("green", "PASS", "The pallet is created. Barcode is shown below");


                    CheckBarcodeBox = new List<FinishedGood>().AsEnumerable();
                    FirstRevisionOnPallet = "TBD";
                    if (ConfirmPallet)
                    {
                        VerifyTextBoxEnabled = true;
                        await UpdateUI();
                        //Goto verify
                        await Task.Delay(1);
                        QtyOfTotalDevices = await TraceDataService.GetQtyOfAddedPoNumbers(PoNumber, PartNo);
                        Scanfield = string.Empty;
                        await UpdateUI();
                        await jSRuntime.InvokeVoidAsync("focusEditorByID", "VerifyScanField");
                        FlashQtyColor(true);
                        return;
                    }
                }

                await Task.Delay(1);
                QtyOfTotalDevices = await TraceDataService.GetQtyOfAddedPoNumbers(PoNumber, PartNo);
                Scanfield = string.Empty;
                TextBoxEnabled = true;
                await UpdateUI();
                //if (!ConfirmPallet)
                await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
                FlashQtyColor(true);
            }
            else
            {
                TextBoxEnabled = false;
                CheckBarcodeBox = await TraceDataService.GetBoxContentInformation(Scanfield, SelectedPartNo);
                if (ScannedBox == null)
                {
                    ScannedBox = new List<FinishedGood>().AsEnumerable();
                }
                //Next step is making pallet
                var list = ScannedBox.ToList();
                var masterList = TotalScannedBox.ToList();

                if (!CheckBarcodeBox.Any())
                {
                    //await jSRuntime.InvokeAsync<string>("ShowText", "ShowError", "Wrong barcode or Box Barcode already in used!");
                    Toast.ShowError("Wrong barcode or Box Barcode already in used!", "Barcode Error");

                    TextBoxEnabled = true;
                    Scanfield = string.Empty;
                    await Task.Delay(5);
                    await InvokeAsync(() => StateHasChanged());
                    await Task.Delay(5);
                    await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
                    await Task.Delay(5);
                    FlashQtyColor(false);
                    return;
                }

                // Check Duplication
                IsDuplicated = masterList.Where(j => j.BarcodeBox == Scanfield).Any();

                if (IsDuplicated == true)
                {
                    TextBoxEnabled = true;
                    Scanfield = string.Empty;
                    await UpdateUI();
                    await Task.Delay(5);
                    await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");

                    Toast.ShowError("Barcode duplication", "Barcode Error");
                    return;
                }


                IsQlyPartBiggerThanQlyBox = CheckBarcodeBox.Count() != QtyPerBox;

                if (IsQlyPartBiggerThanQlyBox == true)
                {
                    Toast.ShowWarning("Partial box", "Box quantity");
                }


                //TotalFgs += CheckBarcodeBox.Count();
                await Task.Delay(5);
                await UpdateUI();

                //Check duplication
                if (CheckBarcodeBox.Count() > 0)
                {
                    var tempBarcodeBox = CheckBarcodeBox.FirstOrDefault();

                    if (IsPhoenix == true)
                    {
                        // Check Revision
                        var tempRevision = int.Parse((tempBarcodeBox.Barcode).Substring(7, 2));
                        if (ScannedBox.Count() < 1)
                        {
                            CurrentIFSRevision = await TraceDataService.GetCustomerVersion(
                                2,
                                tempBarcodeBox.BarcodeBox);


                            FirstRevisionOnPallet = await TraceDataService.GetCustomerVersion(
                                1,
                                tempBarcodeBox.BarcodeBox);
                            if (FirstRevisionOnPallet.Equals(string.Empty))
                            {
                                FirstRevisionOnPallet = CheckBarcodeBox.FirstOrDefault().Barcode.Substring(7, 2);
                            }
                        }

                        bool checkRevision = tempRevision == int.Parse(FirstRevisionOnPallet);
                        if (checkRevision)
                        {
                            list.Add(
                                new FinishedGood
                                {
                                    PartNo = tempBarcodeBox.PartNo,
                                    BarcodeBox = tempBarcodeBox.BarcodeBox,
                                    DateOfPackingBox = tempBarcodeBox.DateOfPackingBox,
                                    QtyBox = CheckBarcodeBox.Count(),
                                    InvoiceNumber = tempBarcodeBox.InvoiceNumber,
                                    Partial = tempBarcodeBox.Partial
                                });
                            masterList.Add(
                                new FinishedGood
                                {
                                    PartNo = tempBarcodeBox.PartNo,
                                    BarcodeBox = tempBarcodeBox.BarcodeBox,
                                    DateOfPackingBox = tempBarcodeBox.DateOfPackingBox,
                                    QtyBox = CheckBarcodeBox.Count(),
                                    InvoiceNumber = tempBarcodeBox.InvoiceNumber,
                                    Partial = tempBarcodeBox.Partial
                                });
                            ScannedBox = list.AsEnumerable();
                            TotalScannedBox = masterList.AsEnumerable();

                        }
                        else
                        {
                            TextBoxEnabled = true;
                            Scanfield = string.Empty;
                            await Task.Delay(5);
                            await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
                            FlashQtyColor(false);
                            return;
                        }
                    }
                    else
                    {
                        list.Add(
                            new FinishedGood
                            {
                                PartNo = tempBarcodeBox.PartNo,
                                BarcodeBox = tempBarcodeBox.BarcodeBox,
                                DateOfPackingBox = tempBarcodeBox.DateOfPackingBox,
                                QtyBox = CheckBarcodeBox.Count()
                            });
                        masterList.Add(
                            new FinishedGood
                            {
                                PartNo = tempBarcodeBox.PartNo,
                                BarcodeBox = tempBarcodeBox.BarcodeBox,
                                DateOfPackingBox = tempBarcodeBox.DateOfPackingBox,
                                QtyBox = CheckBarcodeBox.Count()
                            });
                        ScannedBox = list.AsEnumerable();
                        TotalScannedBox = masterList.AsEnumerable();
                        TotalFgs = TotalFgs + QtyPerBox;
                    }


                    if (ScannedBox.Count() >= PaletteCapacity)
                    {
                        int maxPalletNo = await TraceDataService.GetMaxPaletteNumber(tempBarcodeBox.PartNo);
                        PalletCode = CreatePalletBarcode(tempBarcodeBox.PartNo, maxPalletNo);
                        TextBoxEnabled = false;

                        // Print Barcode
                        await PrintLabel(PalletCode, "barcodepallet", "SHARED_PRINTER");

                        BarcodePallet = "images/barcodepallet.pdf";
                        await Task.Delay(0)
                            .ContinueWith((t) => ScannedBox = new List<FinishedGood>().AsEnumerable());
                        TextBoxEnabled = true;
                        Scanfield = string.Empty;
                        await Task.Delay(1);
                        await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");

                        FlashQtyColor(true);

                        // Update Finishgood Barcode Pallet
                        await TraceDataService.UpdateFinishedGood(
                            tempBarcodeBox.BarcodeBox,
                            PalletCode,
                            maxPalletNo);
                    }

                    TextBoxEnabled = true;
                    Scanfield = string.Empty;
                    await Task.Delay(5);
                    await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
                }
            }


        }
    }

    public string CreatePalletBarcode(string partNo, int maxPallet)
    {
        string myString = maxPallet.ToString();
        //int countString = myString.Length;
        int loop = 10 - myString.Length;
        string resultString = string.Empty;
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
            IsPartial = false;
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

            if (labelType == "barcodepallet")
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
                        //barCode.Save("wwwroot/images/barcodepallet.png", System.Drawing.Imaging.ImageFormat.Png);
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
                    BarcodePallet = string.Empty;
                    //PalletLabel.Content = DateTime.Now.ToString();
                    //await PalletLabel.SetParametersAsync(new ParameterView());
                    await InvokeAsync(() => StateHasChanged());
                    LabelContent = PalletCode;
                    PalletCode = string.Empty;
                    await InvokeAsync(() => StateHasChanged());
                    processor.Dispose();
                }
            }
        }
        catch (Exception)
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

    public async Task FlashQtyColor(bool good)
    {
        QtyCssColor = good ? "greenyellow" : "red";
        await InvokeAsync(
            () =>
            {
                StateHasChanged();
            });
        await Task.Delay(500);
        QtyCssColor = "white";
        await InvokeAsync(
            () =>
            {
                StateHasChanged();
            });
        await Task.Delay(500);
        QtyCssColor = good ? "greenyellow" : "red";
        await InvokeAsync(
            () =>
            {
                StateHasChanged();
            });
        await Task.Delay(500);
        QtyCssColor = "white";
        await InvokeAsync(
            () =>
            {
                StateHasChanged();
            });
        await Task.Delay(500);
        QtyCssColor = good ? "greenyellow" : "red";
        await InvokeAsync(
            () =>
            {
                StateHasChanged();
            });
        await Task.Delay(500);
        QtyCssColor = "white";
        await InvokeAsync(
            () =>
            {
                StateHasChanged();
            });
    }

    async Task UpdateInfoField(string cssTextColor, string? result = null, string? content = null, string? highlightMsg = null, bool reset = false)
    {
        if (result == "ERROR")
        {
            await jSRuntime.InvokeVoidAsync("playSound", "/sounds/alert.wav");
        }
        if (reset)
        {
            InfoCssColor = new();
            Result = new();
            Infofield = new();
            HighlightMsg = new();
        }
        if (string.IsNullOrEmpty(cssTextColor) || string.IsNullOrEmpty(content)) return;

        InfoCssColor.Add(cssTextColor);

        if (result != null)
            Result.Add(result);
        else
            Result.Add("INFO");

        Infofield.Add(content);

        if (highlightMsg != null)
            HighlightMsg.Add(highlightMsg);
        else
            HighlightMsg.Add("");

        await UpdateUI();
    }

    private async Task<bool> CheckBoxInfoAndPrintPOLabel(string barcodeBox)
    {
        //Find any record follows scanned and check part no
        CheckBarcodeBox = await TraceDataService.GetBoxContentInformation(barcodeBox, SelectedPartNo);


        //Check the box exists
        if (CheckBarcodeBox.Count() > 0)
        {
            //Check CV inside box have to be the same
            var cv = await CheckCVInsideBox();
            if (!cv)
            {
                UpdateInfoField("red", "ERROR", "The customer version of FGs in box is not unique!");

                return false;
            }

            UpdateInfoField("green", "PASS", $"The customer version is {CheckBarcodeBox.FirstOrDefault().Rev} and it is unique in box");


            UpdateInfoField("green", "PASS", $"The box belongs to part no: {SelectedPartNo}");


            if (CheckBarcodeBox.Count() > QtyLeft)
            {

                UpdateInfoField("red", "ERROR", $"Quantity check fail", $"{CheckBarcodeBox.Count()} > {QtyLeft}");

                return false;
            }


            if (CheckBarcodeBox.FirstOrDefault().InvoiceNumber is not null)
            {
                UpdateInfoField("orange","WARNING",$"This box is used for PO: {CheckBarcodeBox.FirstOrDefault().InvoiceNumber}");

                if (CheckBarcodeBox.FirstOrDefault().InvoiceNumber == PoNumber)
                {

                    Printing(PoNumber);
                    UpdateInfoField("green",null, "Label is printed");

                    UpdateInfoField("green", "PASS", $"The box is already linked to {PoNumber}");

                    return true;

                }
                else
                {

                    UpdateInfoField("red", "ERROR", $"The box is already linked to another PO {CheckBarcodeBox.FirstOrDefault().InvoiceNumber}");


                    if (ForcePrint)
                    {
                        //For reprint
                    }
                    return false;
                }


            }
            else
            {
                await InsertPoNumber();
                Printing(PoNumber);
                UpdateInfoField("green", null, $"Box: {barcodeBox} linked to PO: {PoNumber}");
                Toast.ShowSuccess($"{barcodeBox} is linked to PO: {PoNumber}", "PO checking");
                QtyLeft = QtyLeft - CheckBarcodeBox.Count();
                return true;
            }


        }
        else
        {
            UpdateInfoField("red", "ERROR", "Invalid Part/PO/Box");
            await UpdateUI();
            return false;
        }
    }

    private async Task<bool> CheckCVInsideBox()
    {

        var ver = CheckBarcodeBox.FirstOrDefault().Rev;
        foreach (var item in CheckBarcodeBox)
        {
            if (item.Rev != ver) return false;
        }
        return true;
    }

    private async Task<bool> InsertPoNumber()
    {
        //Console.WriteLine(Scanfield);
        var rs = await TraceDataService.InsertPurchaseOrderNo(Scanfield, PoNumber);

        if (rs) UpdateInfoField("green", "PASS", "Box is updated"); else UpdateInfoField("red", "ERROR", "Box is not updated");

        return rs;
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

            printFont = new Font("Arial", 23);
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(pd_PrintPage);

            // Print the document.
            pd.Print();
        }
        catch (Exception ex)
        {
            string test = ex.ToString();
        }
        finally
        {
            streamToPrint.Close();
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
