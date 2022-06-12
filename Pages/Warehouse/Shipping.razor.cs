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

    public bool ComboBox1ReadOnly { get; set; }

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
            Title = Value == null ? "Making palette" : "Link carton and PO & Making palette";
            StateHasChanged();
        }
    }

    private Font? printFont;
    private StreamReader? streamToPrint;

    IEnumerable<CustomerOrder>? SelectedPoNumber { get; set; }

    public IEnumerable<CustomerOrder> CustomerOrderData { get; set; }

    //Scan for making palette only
    int QtyPerBox;
    int PaletteCapacity;

    public int TotalFgs { get; set; }

    public bool IsReady { get; set; }
    public bool IsWorking { get => isWorking; set { isWorking = value; if(!isWorking)Scanfield = String.Empty; } }


    //Just only one partial box
    public bool IsPartial { get; set; }

    IEnumerable<FinishedGood>? ScannedBox;

    public List<FinishedGood> CurrentList { get; set; }

    public List<FinishedGood> MasterList { get; set; }

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

    public bool ForceDoNotPrint { get; set; }

    public bool ConfirmPallet { get; set; }

    public bool VerifyTextBoxEnabled { get; set; }

    public List<string> Result { get; set; }

    public List<string> HighlightMsg { get; set; }

    public string? PhoenixPart { get; set; }
    public bool ShouldUpdateUI { get; private set; }

    public string PalletCode = string.Empty;
    private bool isWorking;

    public IEnumerable<string> Printers { get; set; }

    public string SelectedPrinter { get; set; }
    public bool Sound { get; set; }



    protected override async Task OnInitializedAsync()
    {
        IsReady = false;
        ComboBox1ReadOnly = true;
        
        ShouldUpdateUI = true;
    }

    protected override bool ShouldRender()
    {
        return ShouldUpdateUI;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            CustomerOrderData = await TraceDataService.GetCustomerOrders().ConfigureAwait(false);
            ForceDoNotPrint = false;
            ComboBox1ReadOnly = false;
            InfoCssColor = new();
            Infofield = new();
            Result = new();
            HighlightMsg = new();
            SelectedPoNumber = CustomerOrderData.Take(0);
            IsReady = true;
            IsWorking = false;
            withoutPOmode = false;
            TextBoxEnabled = false;
            CheckBarcodeBox = new List<FinishedGood>().AsEnumerable();
            await UpdateUI();
            ScannedBox = new List<FinishedGood>().AsEnumerable();
            TotalScannedBox = new List<FinishedGood>().AsEnumerable();
            Title = "Making pallet";
            CustomerRevisionsDetail = new List<CustomerRevision>().AsEnumerable();
            SelectedPrinter = string.Empty;
            var printers = new List<string>();
            for (int i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
            {
                string pName = PrinterSettings.InstalledPrinters[i];
                printers.Add(pName);
                if (pName.Contains("Brother"))
                    SelectedPrinter = pName;
            }
            Printers = printers.AsEnumerable();
            Sound = true;
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
            Scanfield = String.Empty;
            IsWorking = false;
            await UpdateUI();
            await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");

        }
    }

    async Task UpdateUI()
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

    //Get CO info from PO number
    async void GetCustomerPo(CustomerOrder values)
    {
        if (values == null)
        {
            ResetInfo(true);
            return;
        }
        CheckQtyPlanned = true;

        Title = values == null ? "Making pallet" : "Link carton <--> PO & Making pallet";

        if (values is not null)
        {
            try
            {

                ComboBox1ReadOnly = true;
                Value = values;
                FormJustRead = false;
                TextBoxEnabled = true;
                PoNumber = values.CustomerPoNo;
                PartNo = values.PartNo;
                PartDescription = values.PartDescription;
                RevisedQtyDue = values.RevisedQtyDue;
                QtyShipped = values.QtyShipped;
                QtyLeft = (RevisedQtyDue - QtyShipped);
                PoData = "FRIWO PN: " + PartNo + " - " + PartDescription;

                SelectedPartNo = PartNo;
                SelectedSO = values.OrderNo;


                //Enable verify pallet code
                ConfirmPallet = true;

                //Using for cases making pallet without PO no, such as BOSCH
                withoutPOmode = false;


                TextBoxEnabled = true;
                IsReady = true;
                await UpdateUI();
            }
            catch (Exception)
            {
                QtyPerBox = 0;

                ////Toast.ShowWarning(
                //    $"Cannot find the number box/pallet for part no {SelectedPartNo}",
                //    "Missing information");
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
                    ////Toast.ShowWarning(
                    //    $"Cannot find the prod family for part no {SelectedPartNo}",
                    //    "Missing information");
                }
            }
            else
            {
                SelectedFamily = "Not found from IFS";
                ////Toast.ShowWarning($"Cannot find the prod family for part no {SelectedPartNo}", "Missing information");
            }

            await GetNeededInfoByFamily(SelectedFamily);
        }

    }

    //Check additional information by family
    async Task GetNeededInfoByFamily(string? family = null)
    {
        IsPhoenix = false;

        if (family is null)
        {
            ////Toast.ShowError("Cannot find family for this PO", "Missing info");
            return;
        }


        // Check Phoenix
        if (family == "Phoenix")
        {
            IsPhoenix = true;
            NoShowPhoenix = false;
            PhoenixPart = PartDescription.Substring(PartDescription.Length - 7);
            try
            {
                CurrentIFSRevision = CustomerRevisionsDetail.FirstOrDefault()?.Rev;
            }
            catch
            {
                CurrentIFSRevision = "TBD";
                ////Toast.ShowWarning($"Cannot find the revision for part no {SelectedPartNo}", "Missing information");
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

        //Get info for making pallet
        QtyPerBox = await TraceDataService.GetQtyFromTrace(3, SelectedPartNo);
        PaletteCapacity = await TraceDataService.GetQtyFromTrace(6, SelectedPartNo);
    }

    private async void OnValueChanged(string newValue)
    {
        QtyLeft = int.Parse(new string(newValue.Where(c => char.IsDigit(c)).ToArray()));
        QtyOfTotalDevices = await TraceDataService.GetQtyOfAddedPoNumbers(PoNumber, PartNo);
        QtyLeft = QtyLeft - QtyOfTotalDevices;
        CheckQtyPlanned = false;
        await UpdateUI();
    }

    async void PopupClosing(PopupClosingEventArgs args)
    {
        IsWorking = false;
        await UpdateUI();
        await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
    }

    private void GetInputfield(string content) { Scanfield = content; }

    private async Task HandleInput_Test(KeyboardEventArgs e)
    {
        if (e.Key != "Enter") return;
        GetInputfield(Scanfield);
        IsWorking = true;
        await UpdateUI();
        var rs1 = Printing(Scanfield);
        var rs2 = PrintLabel(Scanfield, "barcodepallet", "Microsoft Print to PDF");
        var rs = Task.WhenAll(rs1);
        await rs.ConfigureAwait(false);
        IsWorking = false;
        await UpdateUI();
    }

    #region Handle Scan Job

    private async Task HandleInput(KeyboardEventArgs e)
    {
        if (e.Key != "Enter") return;
        //jSRuntime.InvokeVoidAsync("playSound", "/sounds/palletbuilt.mp3");
        GetInputfield(Scanfield);

        if (Scanfield is null || Scanfield == string.Empty)
        {

            await UpdateInfoField("red", "ERROR", "Empty Input", "", true);

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
            await UpdateInfoField("red", "ERROR", $"Empty barcode");
            Scanfield = null;
            TextBoxEnabled = true;
            await UpdateUI();
            await Task.Delay(5);
            await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
            FlashQtyColor(true);
            return;
        }

        IsWorking = true;
        IsPartial = false;
        VerifyTextBoxEnabled = false;
        InfoCssColor = new();
        Infofield = new();
        Result = new();
        HighlightMsg = new();
        TextBoxEnabled = false;

        await UpdateUI();

        //Find any record follows scanned and check part no


        #region Make Partial Pallet by scanning barcode

        if (Scanfield == "PartialPallet")
        {
            this.CurrentList = ScannedBox.ToList();
            if (this.CurrentList.Count == 0) return;
            //var tempBarcodeBox = CheckBarcodeBox.First();
            int maxPalletNo = await TraceDataService.GetMaxPaletteNumber(
                SelectedPartNo);
            string PalletCode = CreatePalletBarcode(SelectedPartNo, maxPalletNo);
            //TextBoxEnabled = false;

            //Update Finishgood Barcode Pallet
            for (var i = 0; i < this.CurrentList.Count(); i++)
            {
                await TraceDataService.UpdateFinishedGood(this.CurrentList[i].BarcodeBox, PalletCode, maxPalletNo);
            }

            //Print Barcode
            PrintLabel(PalletCode, "barcodepallet", "FVN-P-MB001");



            BarcodePallet = "images/barcodepallet.pdf";
            await Task.Delay(0).ContinueWith((t) => ScannedBox = new List<FinishedGood>().AsEnumerable());

            if (IsPhoenix)
            {
                //Print Rev
                Printing($"{PhoenixPart}-{CheckBarcodeBox.FirstOrDefault().Rev}");
            }

            await UpdateInfoField("green", "PASS", "The partial pallet is created. Barcode is shown below");
            ScannedBox = new List<FinishedGood>().AsEnumerable();
            jSRuntime.InvokeVoidAsync("playSound", "/sounds/palletbuilt.mp3");

            FirstRevisionOnPallet = "TBD";
            if (ConfirmPallet)
            {
                VerifyTextBoxEnabled = true;
                await UpdateUI();
                //Goto verify
                await Task.Delay(1);

                await UpdateUI();
                await jSRuntime.InvokeVoidAsync("focusEditorByID", "VerifyScanField");
                FlashQtyColor(true);
                return;
            }

            //await UpdateInfoField("orange", "WARNING", "The pallet is not verifed");
            IsWorking = false;

            await UpdateUI();
            await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
        }

        #endregion



        //Check is there partial box in pallet
        if (ScannedBox.Any(_ => _.Partial))
        {
            IsPartial = true;
        }


        if (withoutPOmode)
        {
            MakingPalletNoPO();
            return;
        }

        //Next step is making pallet
        CheckBarcodeBox = await TraceDataService.GetBoxContentInformation(Scanfield, SelectedPartNo);

        //Check barcode and partno
        if (!CheckBarcodeBox.Any() || CheckBarcodeBox == null)
        {
            ResetInfo(false);
            await UpdateInfoField("red", "ERROR", $"Invalid code ", $"{Scanfield}");
            return;
        }

        //Check is full box or partial
        IsQlyPartBiggerThanQlyBox = CheckBarcodeBox.Count() != QtyPerBox;
        if (IsQlyPartBiggerThanQlyBox == true)
        {

            await UpdateInfoField("orange", "WARNING: Partial box");

            if (IsPartial)
            {
                await UpdateInfoField("red", "ERROR", "More than one partial carton on this pallet");

                Scanfield = null;
                TextBoxEnabled = true;
                await UpdateUI();
                await Task.Delay(5);
                await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
                FlashQtyColor(true);
                return;
            }
            else
            {
                IsPartial = true;
                await UpdateInfoField("orange", "WARNING", "Partial carton");
            }
        }
        else
        {

            await UpdateInfoField("green", "PASS", "Carton is full");

        }

        CheckBarcodeBox = await TraceDataService.GetBoxContentInformation(Scanfield, SelectedPartNo);

        if (CheckBarcodeBox.Count() > QtyLeft)
        {

            await UpdateInfoField("red", "ERROR", $"Quantity check fail", $"{CheckBarcodeBox.Count()} > {QtyLeft}");

            return;
        }
        var rs1 = CheckBoxInfoAndPrintPOLabel(Scanfield);
        var rs2 = MakingPallet();

        var rs = await Task.WhenAll<bool>(rs1, rs2);

        //Check box and print PO label
        //if (!rs)
        //{
        //    IsWorking = false;
        //    await Task.Delay(1);
        //    QtyOfTotalDevices = await TraceDataService.GetQtyOfAddedPoNumbers(PoNumber, PartNo);
        //    
        //    TextBoxEnabled = true;
        //    await UpdateUI();
        //    await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
        //    FlashQtyColor(true);
        //    return;
        //}

        IsWorking = false;
        await Task.Delay(1);
        QtyOfTotalDevices = await TraceDataService.GetQtyOfAddedPoNumbers(PoNumber, PartNo);
        Scanfield = string.Empty;
        TextBoxEnabled = true;
        await UpdateUI();
        await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
        FlashQtyColor(true);

    }

    #endregion
    async Task<bool> MakingPallet()
    {
        CheckBarcodeBox = await TraceDataService.GetBoxContentInformation(Scanfield, SelectedPartNo);

        if (CheckBarcodeBox.Count() == 0) return false;

        // Check Duplication
        IsDuplicated = TotalScannedBox.Any(j => j.BarcodeBox == Scanfield);

        if (IsDuplicated == true)
        {
            TextBoxEnabled = true;

            await UpdateUI();
            await Task.Delay(5);
            await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
            ////Toast.ShowError("Barcode duplication", "Barcode Error");

            await UpdateInfoField("red", "ERROR", "This carton is already scanned into the list for making pallet");
            await UpdateUI();


            return false;
        }


        #region Box is made pallet check

        var isUsed = await TraceDataService.CheckBoxInAnyPallete(Scanfield);
        if (isUsed != null && isUsed.Any())
        {
            //Toast.ShowError($"Box is already packaged in pallet {isUsed.FirstOrDefault().BarcodePalette}"
            //, "Barcode Error");


            await UpdateInfoField("red", "ERROR", $"Carton is already packaged in pallet", $"{isUsed.FirstOrDefault().BarcodePalette}");

            TextBoxEnabled = true;

            await Task.Delay(5);
            await InvokeAsync(() => StateHasChanged());
            await Task.Delay(5);
            await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
            await Task.Delay(5);
            FlashQtyColor(false);
            return false;
        }
        else
        {
            await UpdateInfoField("green", "PASS", "Check already scan to pallet");

        }

        #endregion


        #region Check Customer Version
        //Check customer version
        if (IsPhoenix == true)
        {

            await UpdateInfoField("green", null, "This is Phoenix product");

            // Set cv for Pallet and PO
            var tempRevision = CheckBarcodeBox.FirstOrDefault().Barcode.Substring(7, 2);

            if (ScannedBox.Count() < 1)
            {
                if (FirstRevisionOnPO.Equals("TBD"))
                {
                    FirstRevisionOnPO = tempRevision;

                    await UpdateInfoField("green", "PASS", "The customer version is set for this PO");

                }
                if (FirstRevisionOnPallet.Equals("TBD"))
                {
                    FirstRevisionOnPallet = tempRevision;

                    await UpdateInfoField("green", "PASS", "The customer version is set for this pallet");

                }
            }

            //Check cv is same as PO cv
            bool checkRevisionPO = tempRevision == FirstRevisionOnPO;
            if (!checkRevisionPO)
            {
                //Toast.ShowError("Different Phoenix Rev", "Wrong Rev");
                TextBoxEnabled = true;

                await Task.Delay(5);
                await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
                FlashQtyColor(false);
                return false;
            }
            else
            {
                await UpdateInfoField("green", "PASS", "The customer version as same as PO customer version");
            }

            //Check cv is same as 1st box is pallet
            bool checkRevision = tempRevision == FirstRevisionOnPallet;
            if (!checkRevision)
            {
                //Toast.ShowError("Different Phoenix Rev", "Wrong Rev");
                TextBoxEnabled = true;

                await Task.Delay(5);
                await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
                FlashQtyColor(false);
                return false;
            }
            else
            {
                await UpdateInfoField("green", "PASS", "The customer version as same as pallet customer version");

            }


        }

        #endregion

        #region Add box to list and calculate quantity
        //Add box to list for making pallet
        CurrentList = ScannedBox.ToList<FinishedGood>();
        MasterList = TotalScannedBox.ToList<FinishedGood>();

        CurrentList.Add(
                new FinishedGood
                {
                    PartNo = CheckBarcodeBox.FirstOrDefault().PartNo,
                    BarcodeBox = CheckBarcodeBox.FirstOrDefault().BarcodeBox,
                    DateOfPackingBox = CheckBarcodeBox.FirstOrDefault().DateOfPackingBox,
                    InvoiceNumber = CheckBarcodeBox.FirstOrDefault().InvoiceNumber,
                    QtyBox = CheckBarcodeBox.Count(),
                    Rev = CheckBarcodeBox.FirstOrDefault().Barcode.Substring(7, 2),
                    Partial = IsPartial
                });

        MasterList.Add(
            new FinishedGood
            {
                PartNo = CheckBarcodeBox.FirstOrDefault().PartNo,
                BarcodeBox = CheckBarcodeBox.FirstOrDefault().BarcodeBox,
                DateOfPackingBox = CheckBarcodeBox.FirstOrDefault().DateOfPackingBox,
                InvoiceNumber = CheckBarcodeBox.FirstOrDefault().InvoiceNumber,
                QtyBox = CheckBarcodeBox.Count(),
                Rev = CheckBarcodeBox.FirstOrDefault().Barcode.Substring(7, 2),
                Partial = IsPartial
            });

        ScannedBox = CurrentList.AsEnumerable();
        TotalScannedBox = MasterList.AsEnumerable();
        TotalFgs = TotalFgs + CheckBarcodeBox.Count();

        await UpdateInfoField("green", "PASS", "The carton is added to buffer for making pallet");

        #endregion

        #region Build Pallet when it is full
        //Check pallet is full
        if (ScannedBox.Count() >= PaletteCapacity)
        {
            //var tempBarcodeBox = CheckBarcodeBox.First();
            int maxPalletNo = await TraceDataService.GetMaxPaletteNumber(
                CheckBarcodeBox.FirstOrDefault().PartNo);
            string PalletCode = CreatePalletBarcode(
                CheckBarcodeBox.FirstOrDefault().PartNo, maxPalletNo);
            //TextBoxEnabled = false;

            ////Update Finishgood Barcode Pallet
            //for (var i = 0; i < CurrentList.Count(); i++)
            //{
            //    await TraceDataService.UpdateFinishedGood(CurrentList[i].BarcodeBox, PalletCode, maxPalletNo);
            //}
            foreach (var item in CurrentList)
            {
                if (item.BarcodeBox == null)
                {
                    await UpdateInfoField("red", "ERROR", $"Fail on update at ", $"{CurrentList.IndexOf(item)}");
                    return false;
                }
                if (item.BarcodeBox != null)
                    await TraceDataService.UpdateFinishedGood(item.BarcodeBox, PalletCode, maxPalletNo);
            }

            // Print Barcode
            //PrintLabel(PalletCode, "barcodepallet", "Microsoft Print to PDF");
            PrintLabel(PalletCode, "barcodepallet", "FVN-P-MB001");


            BarcodePallet = "images/barcodepallet.pdf";
            ScannedBox = new List<FinishedGood>().AsEnumerable();

            await UpdateInfoField("green", "PASS", "The pallet is created. Barcode is shown below");
            jSRuntime.InvokeVoidAsync("playSound", "/sounds/palletbuilt.mp3");


            if (IsPhoenix)
            {
                //Print Rev
                Printing($"{PhoenixPart}-{CheckBarcodeBox.FirstOrDefault().Rev}");
            }

            //CheckBarcodeBox = new List<FinishedGood>().AsEnumerable();
            FirstRevisionOnPallet = "TBD";
            if (ConfirmPallet)
            {
                VerifyTextBoxEnabled = true;
                await UpdateUI();
                //Goto verify
                await Task.Delay(1);
                QtyOfTotalDevices = await TraceDataService.GetQtyOfAddedPoNumbers(PoNumber, PartNo);

                await UpdateUI();
                await jSRuntime.InvokeVoidAsync("focusEditorByID", "VerifyScanField");
                FlashQtyColor(true);
            }
        }

        return true;
        #endregion
    }
    async Task MakingPalletNoPO()
    {

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
            //Toast.ShowError("Wrong barcode or Box Barcode already in used!", "Barcode Error");

            TextBoxEnabled = true;

            await Task.Delay(5);
            await InvokeAsync(() => StateHasChanged());
            await Task.Delay(5);
            await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
            await Task.Delay(5);
            FlashQtyColor(false);
            return;
        }

        #region Check CV in box unique
        //Check CV inside box have to be the same
        var cv = await CheckCVInsideBox();

        if (!cv)
        {
            await UpdateInfoField("red", "ERROR", "The customer version of FGs in box is not unique!");

            return;
        }
        else
        {
            await UpdateInfoField("green", "PASS", $"The customer version is {CheckBarcodeBox.FirstOrDefault().Rev} and it is unique in box");

        }
        #endregion

        // Check Duplication
        IsDuplicated = masterList.Where(j => j.BarcodeBox == Scanfield).Any();

        if (IsDuplicated == true)
        {
            TextBoxEnabled = true;

            await UpdateUI();
            await Task.Delay(5);
            await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");

            //Toast.ShowError("Barcode duplication", "Barcode Error");
            return;
        }


        IsQlyPartBiggerThanQlyBox = CheckBarcodeBox.Count() != QtyPerBox;

        if (IsQlyPartBiggerThanQlyBox == true)
        {
            //Toast.ShowWarning("Partial box", "Box quantity");
        }


        //TotalFgs += CheckBarcodeBox.Count();
        await Task.Delay(5);
        await UpdateUI();


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
                await PrintLabel(PalletCode, "barcodepallet", "Microsoft Print to PDF");

                BarcodePallet = "images/barcodepallet.pdf";
                await Task.Delay(0)
                    .ContinueWith((t) => ScannedBox = new List<FinishedGood>().AsEnumerable());
                TextBoxEnabled = true;

                await Task.Delay(1);
                await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");

                FlashQtyColor(true);

                //Update Finishgood Barcode Pallet
                for (var i = 0; i < CurrentList.Count(); i++)
                {
                    await TraceDataService.UpdateFinishedGood(CurrentList[i].BarcodeBox, PalletCode, maxPalletNo);
                }
            }

            TextBoxEnabled = true;

            await Task.Delay(5);
            await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
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
            BarCode barCode = new BarCode();
            barCode.Symbology = Symbology.DataMatrix;
            barCode.Options.DataMatrix.ShowCodeText = false;
            barCode.Options.DataMatrix.MatrixSize = DataMatrixSize.MatrixAuto;
            barCode.CodeText = content;
            barCode.Margins.Right = 0;
            barCode.Margins.Top = 0;
            barCode.Margins.Bottom = 0;
            barCode.Margins.Left = 0;
            barCode.BackColor = Color.White;
            barCode.ForeColor = Color.Black;
            barCode.RotationAngle = 0;
            barCode.CodeBinaryData = Encoding.Default.GetBytes(barCode.CodeText);
            barCode.DpiX = 203;
            barCode.DpiY = 203;
            barCode.Module = 1f;
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
                    string printerName = SelectedPrinter;
                    //for (int i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
                    //{
                    //    string pName = PrinterSettings.InstalledPrinters[i];
                    //    if (pName.Contains($"{selectedPrinter}"))
                    //    {
                    //        printerName = pName;
                    //        break;
                    //    }
                    //}
                    PdfPrinterSettings printerSettings = new PdfPrinterSettings();
                    //printerSettings.PrintingDpi = 203;
                    printerSettings.PrintInGrayscale = true;
                    printerSettings.Settings.DefaultPageSettings.PaperSize = new PaperSize($"{labelType}", 72, 203);
                    printerSettings.Settings.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
                    printerSettings.Settings.PrinterName = printerName;
                    processor.CreateEmptyDocument();

                    //page.Size = addSize;
                    PdfPage pdfPage = processor.AddNewPage(new PdfRectangle(0, 0, 72, 203));

                    //document.Pages.Add(page); using (PdfGraphics graphics = processor.CreateGraphics())
                    using (PdfGraphics graphics = processor.CreateGraphics())
                    {
                        // Draw a rectangle.
                        //using (var pen = new Pen(Color.Black, 1))
                        // graphics.DrawRectangle(pen, new RectangleF(2, 2, 68, 68));
                        //barCode.Save("wwwroot/images/barcodepallet.png", System.Drawing.Imaging.ImageFormat.Png);
                        //await Task.Delay(500);
                        //barCode.Dispose();
                        var img = Image.FromFile($"wwwroot/images/{labelType}.bmp");
                        graphics.DrawImage(img, new PointF(1, 1));

                        // Add graphics content to the document page.
                        graphics.AddToPageForeground(pdfPage, 203, 203);
                        img.Dispose();
                        graphics.Dispose();
                    }

                    processor.SaveDocument($"wwwroot/images/{labelType}.pdf");
                    processor.Print(printerSettings);
                    await Task.Delay(1);
                    BarcodePallet = string.Empty;
                    //PalletLabel.Content = DateTime.Now.ToString();
                    //await PalletLabel.SetParametersAsync(new ParameterView());                
                    LabelContent = PalletCode;
                    PalletCode = string.Empty;
                    processor.Dispose();
                    await UpdateUI();
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
        if (reset)
        {
            InfoCssColor = new();
            Result = new();
            Infofield = new();
            HighlightMsg = new();
        }

        if (result == "ERROR")
        {
            ResetInfo(false);
            if (Sound)
            {
                await jSRuntime.InvokeVoidAsync("playSound", "/sounds/alert.wav");
            }

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
            HighlightMsg.Add(string.Empty);

        await UpdateUI();
    }

    #region Check & print PO label
    //Check & print PO label
    private async Task<bool> CheckBoxInfoAndPrintPOLabel(string barcodeBox)
    {
        //Check the box exists
        if (CheckBarcodeBox.Count() > 0)
        {

            await UpdateInfoField("green", "PASS", $"The carton belongs to part no:", $"{SelectedPartNo}");

            if (CheckBarcodeBox.FirstOrDefault().InvoiceNumber is not null)
            {
                await UpdateInfoField("orange", "WARNING", $"This carton already linked to PO: {CheckBarcodeBox.FirstOrDefault().InvoiceNumber}");

                if (CheckBarcodeBox.FirstOrDefault().InvoiceNumber == PoNumber)
                {
                    if (!ForceDoNotPrint)
                    {
                        Printing(PoNumber);
                        await UpdateInfoField("green", null, "Label is re-printed");

                        //await UpdateInfoField("green", "PASS", $"The box is already linked to {PoNumber}");

                    }
                    return true;

                }
                else
                {

                    await UpdateInfoField("red", "ERROR", $"The carton is already linked to another PO {CheckBarcodeBox.FirstOrDefault().InvoiceNumber}");


                    //if (ForceDoNotPrint)
                    //{
                    //    //For reprint
                    //}
                    return false;
                }


            }
            else
            {
                await InsertPoNumber();
                if (!ForceDoNotPrint)
                {
                    Printing(PoNumber);
                }

                await UpdateInfoField("green", null, $"Box: {barcodeBox} linked to PO: {PoNumber}");
                //Toast.ShowSuccess($"{barcodeBox} is linked to PO: {PoNumber}", "PO checking");
                QtyLeft = QtyLeft - CheckBarcodeBox.Count();
                return true;
            }

            await UpdateUI();
            FlashQtyColor(false);
            return true;
        }
        else
        {
            await UpdateInfoField("red", "ERROR", "Invalid Part/PO/Box");
            await UpdateUI();
            return false;
        }
    }
    #endregion

    private async Task<bool> CheckCVInsideBox()
    {
        if (CheckBarcodeBox == null) return false;

        var ver = CheckBarcodeBox.FirstOrDefault().BarcodeBox.Substring(7, 2);

        if (ver == null) return false;

        foreach (var item in CheckBarcodeBox)
        {
            if (item.Rev == null || item.Rev != ver) return false;
        }
        return true;
    }

    private async Task<bool> InsertPoNumber()
    {
        //Console.WriteLine(Scanfield);
        var rs = await TraceDataService.InsertPurchaseOrderNo(Scanfield, PoNumber);

        if (rs) await UpdateInfoField("green", "PASS", "Database is updated. ", PoNumber); else await UpdateInfoField("red", "ERROR", "Carton is not updated");

        return rs;
    }

    // The PrintPage event is raised for each page to be printed.
    private void pd_PrintPage(string content, object sender, PrintPageEventArgs ev)
    {
        float linesPerPage = 0;
        float yPos = 5; //9
        int count = 0;
        //float leftMargin = ev.MarginBounds.Left;
        //float topMargin = ev.MarginBounds.Top;
        float leftMargin = 0; //79
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
        ev.Graphics.DrawString(content, printFont, Brushes.Black, leftMargin, yPos, new StringFormat());
        // If more lines exist, print another page.
        if (line != null)
            ev.HasMorePages = true;
        else
            ev.HasMorePages = false;
    }

    // Print the file.
    public async Task Printing(string content)
    {
        Stream stream = null;
        try
        {
            //WebClient client = new WebClient();
            //Stream stream = client.OpenRead("https://filesamples.com/samples/document/txt/sample3.txt");
            stream = await GenerateStreamFromString(content);
            streamToPrint = new StreamReader(stream);

            printFont = new Font("Arial", 23);
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler((s, e) => pd_PrintPage(content, s, e));

            // Print the document.
            pd.PrinterSettings.PrinterName = SelectedPrinter;
            pd.Print();
        }
        catch (Exception ex)
        {
            string test = ex.ToString();
        }
        finally
        {
            stream.Close();
            streamToPrint.Close();
        }
    }

    private async Task<Stream> GenerateStreamFromString(string input)
    {
        UTF8Encoding utf8 = new UTF8Encoding();
        MemoryStream stream = new MemoryStream();
        var streamWriter = new StreamWriter(stream);
        await streamWriter.WriteLineAsync(input);
        var encodedBytes = Encoding.ASCII.GetBytes(streamWriter.ToString());
        encodedBytes = utf8.GetPreamble();
        stream.Write(encodedBytes, 0, encodedBytes.Length);
        return stream;
    }
}
