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
using System.ComponentModel.DataAnnotations;
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
    private TraceService TraceDataService { get; set; }

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

    public int QtyInShipQueue { get; set; }

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
            Title = Value == null ? "Making pallet" : "Link carton and PO & Making pallet";
            StateHasChanged();
        }
    }

    public bool ShowScanBarcode { get; set; }

    private Font? printFont;
    private StreamReader? streamToPrint;

    private CustomerOrder SelectedPoNumber { get; set; }

    public IEnumerable<CustomerOrder> CustomerOrderData { get; set; }

    public IEnumerable<CustomerRevision> CustomerOrders { get; set; }
    //Scan for making palette only
    int QtyPerBox;
    int PaletteCapacity;

    public int TotalFgs { get; set; }

    public bool IsReady { get; set; }
    public bool IsWorking { get => isWorking; set { isWorking = value; if (!isWorking) { Scanfield = String.Empty; } } }


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

    bool verifyTextBoxEnabled;
    public bool VerifyTextBoxEnabled { get => verifyTextBoxEnabled; set { verifyTextBoxEnabled = value; IsWorking = !value; } }

    public List<string> Result { get; set; }

    public List<string> HighlightMsg { get; set; }

    public string? PhoenixPart { get; set; }
    public bool ShouldUpdateUI { get; private set; }

    public string PalletCode = string.Empty;
    private bool isWorking;

    public IEnumerable<string> Printers { get; set; }

    public string SelectedPrinter { get; set; }
    public bool Sound { get; set; }

    private string pORevision;

    public string PORevision
    {
        get => pORevision; set
        {
            Task.Run(async () =>
            {
                pORevision = value;
                await UpdateUI();
            });
        }
    }

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
            CustomerOrders = await TraceDataService.GetCustomerRevision(2, "", "", "", "");
            ForceDoNotPrint = false;
            ComboBox1ReadOnly = false;
            InfoCssColor = new();
            Infofield = new();
            Result = new();
            HighlightMsg = new();
            SelectedPoNumber = new CustomerOrder();
            IsWorking = false;
            withoutPOmode = false;
            TextBoxEnabled = false;
            CheckBarcodeBox = new List<FinishedGood>().AsEnumerable();
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
                if (pName.Contains("SHARED_PRINTER"))
                    SelectedPrinter = pName;
            }
            Printers = printers.AsEnumerable();
            Sound = true;
            ShowScanBarcode = false;
            await UpdateUI();
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
        withoutPOmode = true;
        IsWorking = false;
        TextBoxEnabled = true;
        ConfirmPallet = true;
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

            SelectedFamily = await TraceDataService.GetFamilyFromPartNo(SelectedFamily);
            QtyPerBox = await TraceDataService.GetQtyFromTrace(3, SelectedPartNo);
            PaletteCapacity = await TraceDataService.GetQtyFromTrace(6, SelectedPartNo);
            if (withoutPOmode)
            {
                IsReady = true;
                IsWorking = false;
                ForceDoNotPrint = true;

            }
            await UpdateUI();
            await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
            return true;
        }
        else
        {
            UpdateInfoField("orange", "ERROR", $"Invalid Part number");
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

    async Task ResetInfo(bool backToStart = false)
    {
        try
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
                SelectedPartNo = string.Empty;
                SelectedSO = string.Empty;
                ScannedBox = new List<FinishedGood>().AsEnumerable();
                QtyPerBox = 0;
                PaletteCapacity = 0;
                IsWorking = true;
                IsReady = false;
                //Phoenix info will be show for phoenix product
                IsPhoenix = false;
                NoShowPhoenix = true;

                //Update UI
                await UpdateUI();
            }
            else
            {
                VerifyTextBoxEnabled = false;
                IsWorking = false;
                //CheckBarcodeBox = new List<FinishedGood>().AsEnumerable();
                await UpdateUI();
                await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
            }
        }
        catch (Exception)
        {
            Scanfield = String.Empty;
            VerifyTextBoxEnabled = false;
            IsWorking = false;
            //CheckBarcodeBox = new List<FinishedGood>().AsEnumerable();
            await UpdateUI();
            await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
        }

    }

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

    //Get CO info from PO number
    async void GetCustomerPo(CustomerOrder values)
    {
        if (values == null)
        {
            await ResetInfo(true);
            return;
        }
        CheckQtyPlanned = true;
        await UpdateUI();
        Title = values == null ? "Making pallet" : "Link carton <--> PO & Making pallet";

        if (values is not null)
        {
            try
            {
                SelectedPoNumber = values;
                ComboBox1ReadOnly = true;
                Value = values;
                FormJustRead = false;
                TextBoxEnabled = true;
                PoNumber = values.CustomerPoNo;
                PartNo = values.PartNo;
                PartDescription = values.PartDescription;
                RevisedQtyDue = values.RevisedQtyDue;
                QtyInShipQueue = await TraceDataService.GetQtyOfAddedPoNumbers(SelectedPoNumber.CustomerPoNo, SelectedPoNumber.PartNo);
                QtyShipped = values.QtyShipped;
                QtyLeft = (RevisedQtyDue - QtyShipped) - QtyInShipQueue;
                PoData = "FRIWO PN: " + PartNo + " - " + PartDescription;

                SelectedPartNo = PartNo;
                SelectedSO = values.OrderNo;
                PORevision = "0";

                //Enable verify pallet code
                ConfirmPallet = true;

                //Using for cases making pallet without PO no, such as BOSCH
                withoutPOmode = false;


                //Get family
                CustomerRevisionsDetail = await TraceDataService.GetCustomerRevision(
                    0,
                    $"{SelectedPoNumber.CustomerPoNo}",
                    string.Empty,
                    string.Empty,
                    string.Empty);

                if (CustomerRevisionsDetail != null)
                {
                    try
                    {
                        SelectedFamily = CustomerRevisionsDetail.First().ProductFamily;
                    }
                    catch (Exception)
                    {
                        SelectedFamily = "Not found from IFS";
                        ////Toast.ShowWarning(
                        //    $"Cannot find the prod family for part no {SelectedPartNo}",
                        //    "Missing information");1858353 B0000005355-11
                    }
                }
                else
                {
                    SelectedFamily = "Not found from IFS";
                    ////Toast.ShowWarning($"Cannot find the prod family for part no {SelectedPartNo}", "Missing information");
                }

                VerifyTextBoxEnabled = false;
                TextBoxEnabled = true;
                //IsReady = true;
                await UpdateUI();
            }
            catch (Exception)
            {
                QtyPerBox = 0;
                Toast.ShowWarning(
                $"Cannot get the number box/pallet for part no {SelectedPartNo}",
                    "Missing information");
            }


            await GetNeededInfoByFamily(SelectedFamily);
            //Get info for making pallet
            QtyPerBox = await TraceDataService.GetQtyFromTrace(3, SelectedPartNo);
            PaletteCapacity = await TraceDataService.GetQtyFromTrace(6, SelectedPartNo);
            await UpdateUI();
            await jSRuntime.InvokeVoidAsync("focusEditorByID", "POField");
        }

    }

    //Check additional information by family
    async Task GetNeededInfoByFamily(string? family = null)
    {
        IsPhoenix = false;
        PORevision = "";
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

            try
            {
                CurrentIFSRevision = CustomerRevisionsDetail.FirstOrDefault()?.Rev;
            }
            catch
            {
                CurrentIFSRevision = "";
                ////Toast.ShowWarning($"Cannot find the revision for part no {SelectedPartNo}", "Missing information");
            }
            try
            {
                FirstRevisionOnPO = await TraceDataService.GetCustomerVersion(0, SelectedPoNumber.CustomerPoNo);
                if (FirstRevisionOnPO == "null") FirstRevisionOnPO = "";
                FirstRevisionOnPallet = "";
            }
            catch
            {
                FirstRevisionOnPO = "";
                FirstRevisionOnPallet = "";
            }
        }

    }

    private async void OnValueChanged(string newValue)
    {
        QtyLeft = int.Parse(new string(newValue.Where(c => char.IsDigit(c)).ToArray()));
        // //QtyOfTotalDevices = await TraceDataService.GetQtyOfAddedPoNumbers(PoNumber, PartNo);
        QtyInShipQueue = await TraceDataService.GetQtyOfAddedPoNumbers(SelectedPoNumber.CustomerPoNo, SelectedPartNo);
        QtyLeft = QtyLeft - QtyInShipQueue;
        CheckQtyPlanned = false;
        await UpdateUI();
    }

    async void PopupClosing(PopupClosingEventArgs args)
    {
       
        if (IsPhoenix)
        {
            IsReady = false;
            VerifyTextBoxEnabled = false;
            await UpdateUI();
            await jSRuntime.InvokeVoidAsync("focusEditorByID", "POField");
            return;
        }
        VerifyTextBoxEnabled = false;
        IsWorking = false;
        IsReady = true;
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
        Printing(Scanfield);
        PrintLabel(Scanfield, "barcodepallet", SelectedPrinter);
        IsWorking = false;
        Scanfield = String.Empty;
        await UpdateUI();
    }

    private async Task HandlePOInput(KeyboardEventArgs e)
    {
        if (e.Key != "Enter") return;
        if (PORevision != FirstRevisionOnPO) { PORevision = FirstRevisionOnPO; }
        IsWorking = false;
        IsReady = true;
        await UpdateUI();
        await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");

    }
    private async Task HandleInput(KeyboardEventArgs e)
    {
        if (e.Key != "Enter") return;
        
        if (Scanfield is null || Scanfield == string.Empty)
        {
            //UpdateInfoField("red", "ERROR", "Empty Input", "", true);
            //await ResetInfo();
            return;
        }
        
        GetInputfield(Scanfield);

        IsWorking = true;
        IsPartial = false;
        InfoCssColor = new();
        Infofield = new();
        Result = new();
        HighlightMsg = new();
        TextBoxEnabled = false;
        await UpdateUI();

        //Find any record follows scanned and check part no
        #region Make Partial Pallet by scanning barcode

        if (Scanfield.Contains("PartialPallet"))
        {
            //var tempBarcodeBox = CheckBarcodeBox.First();
            int maxPalletNo = await TraceDataService.GetMaxPaletteNumber(
                CheckBarcodeBox.FirstOrDefault().PartNo);
            string PalletCode = CreatePalletBarcode(
                CheckBarcodeBox.FirstOrDefault().PartNo, maxPalletNo);

            foreach (var item in ScannedBox)
            {
                if (item.BarcodeBox == null)
                {
                    UpdateInfoField("red", "ERROR", $"Fail on update at ", $"{ScannedBox.ToList().IndexOf(item)}");
                    return;
                }
                if (item.BarcodeBox != null)
                    await TraceDataService.UpdateFinishedGood(item.BarcodeBox, PalletCode, maxPalletNo);
            }

            //Print Barcode
            //PrintLabel(PalletCode, "barcodepallet", "Microsoft Print to PDF");

            PrintLabel(PalletCode, "barcodepallet", SelectedPrinter);

            BarcodePallet = "images/barcodepallet.pdf";

            UpdateInfoField("green", "SUCCESS", "The pallet is created. Barcode is shown below");
            if (Sound)
                jSRuntime.InvokeVoidAsync("playSound", "/sounds/palletbuilt.mp3");

            if (IsPhoenix)
            {
                //Print Rev
                Printing($"{CheckBarcodeBox.FirstOrDefault().Rev}");
            }

            ScannedBox = new List<FinishedGood>().AsEnumerable();

            if (ConfirmPallet)
            {
                VerifyTextBoxEnabled = true;
                //Goto verify
                await UpdateUI();
                await jSRuntime.InvokeVoidAsync("focusEditorByID", "VerifyScanField");
                FlashQtyColor(true);
                return;
            }

            await UpdateUI();
            await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
            return;
        }


        #endregion
        CheckBarcodeBox = await TraceDataService.GetBoxContentInformation(Scanfield, SelectedPartNo);
        #region Check is there partial box in scanned list; set IsPartial true if any
        if (ScannedBox !=null&&ScannedBox.Count() > 0 && ScannedBox.Any(_=>_.QtyBox<QtyPerBox))
        {
            IsPartial = true;
        }
        #endregion

        #region Check duplication in scanned
        // Check Duplication
        IsDuplicated = ScannedBox.Any(j => j.BarcodeBox == Scanfield);
        if (IsDuplicated == true)
        {
            UpdateInfoField("red", "ERROR", "This carton is already scanned into the list for making pallet");
            await ResetInfo(false);
            return;
        }

        #endregion

        CheckBarcodeBox = await TraceDataService.GetBoxContentInformation(Scanfield, SelectedPartNo);

        //Check barcode and partno
        if (!CheckBarcodeBox.Any() || CheckBarcodeBox == null)
        {
            UpdateInfoField("red", "ERROR", $"{Scanfield} is invalid");
            await ResetInfo(false);
            return;
        }

        //Check is full box or partial
        IsQlyPartBiggerThanQlyBox = CheckBarcodeBox.Count() != QtyPerBox;
        if (IsQlyPartBiggerThanQlyBox == true)
        {

            UpdateInfoField("orange", "WARNING: Partial box");

            if (IsPartial)
            {
                UpdateInfoField("red", "ERROR", "More than one partial carton on this pallet");

                Scanfield = string.Empty;
                TextBoxEnabled = true;
                await UpdateUI();
                await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
                FlashQtyColor(true);
                ResetInfo();
                return;
            }
            else
            {
                //IsPartial = true;
                UpdateInfoField("orange", "WARNING", "Partial carton");
            }
        }
        else
        {
            UpdateInfoField("green", "SUCCESS", "Carton is full");
        }

        //Scan with PO
        if (!withoutPOmode)
        {
            if (CheckBarcodeBox.Count() >= QtyLeft)
            {

                UpdateInfoField("red", "ERROR", $"Quantity check fail", $"{CheckBarcodeBox.Count()} > {QtyLeft}");

                return;
            }

            #region Check Customer Version
            //Check customer version
            if (IsPhoenix == true)
            {
                UpdateInfoField("green", "INFO", "Phoenix product");

                var tempRevision = CheckBarcodeBox.FirstOrDefault().Barcode.Substring(7, 2);

                //Check unique CV in carton box
                var ver = CheckBarcodeBox.FirstOrDefault().Barcode.Substring(7, 2);
                foreach (var item in CheckBarcodeBox)
                {
                    if (item.Rev == null || item.Rev != ver)
                    {
                        UpdateInfoField("red", "ERROR", "The customer version is not unique");
                        await ResetInfo(false);

                        return;
                    }
                }


                //Check cv is same as PO cv
                bool checkRevisionPO = PORevision == tempRevision;
                if (!checkRevisionPO)
                {
                    //Toast.ShowError("Different Phoenix Rev", "Wrong Rev");
                    IsWorking = false;
                    Scanfield = string.Empty;
                    TextBoxEnabled = true;
                    UpdateInfoField("red", "ERROR", "The carton's customer version is not as same as pallet's customer version", $"{tempRevision} <> {PORevision}");
                    await Task.Delay(5);
                    await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
                    await UpdateUI();
                    FlashQtyColor(false);
                    return;
                }
                else
                {
                    UpdateInfoField("green", "SUCCESS", "The carton's C/V is similar to PO");
                }
            }

            #endregion
            //Check Invoice No
            if (CheckBarcodeBox.FirstOrDefault().InvoiceNumber is not null)
            {
                //UpdateInfoField("orange", "WARNING", $"This carton already linked to PO: {CheckBarcodeBox.FirstOrDefault().InvoiceNumber}");
                if (CheckBarcodeBox.FirstOrDefault().InvoiceNumber != SelectedPoNumber.CustomerPoNo)
                {
                    UpdateInfoField("red", "ERROR", $"The carton is already linked to another PO {CheckBarcodeBox.FirstOrDefault().InvoiceNumber}");
                    await ResetInfo(false);
                    return;
                }
                else
                {
                    UpdateInfoField("green", "INFO", $"{Scanfield} is re-printed label", $"{SelectedPoNumber.CustomerPoNo}");
                }

            }
            else
            {
               
                UpdateInfoField("green", "INFO", $"{Scanfield} is added to", $"{SelectedPoNumber.CustomerPoNo}");
            }

            await InsertPoNumber(CheckBarcodeBox.FirstOrDefault().BarcodeBox, SelectedPoNumber.CustomerPoNo);

            var temp = await TraceDataService.GetQtyOfAddedPoNumbers(SelectedPoNumber.CustomerPoNo, SelectedPartNo);

            QtyLeft = QtyLeft - (temp-QtyInShipQueue);
            QtyInShipQueue = temp;
            Printing(SelectedPoNumber.CustomerPoNo);

        }

        #region Box is made pallet check

        var isUsed = await TraceDataService.CheckBoxInAnyPallete(Scanfield);
        if (isUsed != null && isUsed.Any())
        {
            UpdateInfoField("red", "ERROR", $"Carton is already packaged in pallet", $"{isUsed.FirstOrDefault().BarcodePalette}");

            TextBoxEnabled = true;

            await Task.Delay(5);
            await InvokeAsync(() => StateHasChanged());
            await Task.Delay(5);
            await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
            await Task.Delay(5);
            FlashQtyColor(false);
            return;
        }
        else
        {
            UpdateInfoField("green", "SUCCESS", "This carton is available for making pallet");

        }

        #endregion
        CheckBarcodeBox = await TraceDataService.GetBoxContentInformation(Scanfield, SelectedPartNo);
        #region Add box to list and calculate quantity
        //if (ScannedBox == null || !ScannedBox.Any())
        //    FirstRevisionOnPO = CheckBarcodeBox.First().Barcode.Substring(7, 2);
        //Add box to list for making pallet
        var t = ScannedBox.ToList();
        t.Add(
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
        ScannedBox = t.AsEnumerable();

        var t1 = TotalScannedBox.ToList();
        t1.Add(
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
        TotalScannedBox = t1.AsEnumerable();
        TotalFgs += CheckBarcodeBox.Count();

        UpdateInfoField("green", "SUCCESS", "The carton now is in queue for making pallet");

        #endregion

        if (IsPhoenix)
        {
            //Print Rev
            Printing($"{ScannedBox.Last().Rev}");
        }

        #region Build Pallet when it is full
        //Check pallet is full
        //if (ScannedBox.Count() >= PaletteCapacity)
        if (ScannedBox.Count() >= 2)
        {
            //var tempBarcodeBox = CheckBarcodeBox.First();
            int maxPalletNo = await TraceDataService.GetMaxPaletteNumber(
                CheckBarcodeBox.FirstOrDefault().PartNo);
            string PalletCode = CreatePalletBarcode(
                CheckBarcodeBox.FirstOrDefault().PartNo, maxPalletNo);

            foreach (var item in ScannedBox)
            {
                if (item.BarcodeBox == null)
                {
                    UpdateInfoField("red", "ERROR", $"Fail on update at ", $"{ScannedBox.ToList().IndexOf(item)}");
                    return;
                }
                if (item.BarcodeBox != null)
                    await TraceDataService.UpdateFinishedGood(item.BarcodeBox, PalletCode, maxPalletNo);
            }

            //Print Barcode
            //PrintLabel(PalletCode, "barcodepallet", "Microsoft Print to PDF");

            PrintLabel(PalletCode, "barcodepallet", SelectedPrinter);

            BarcodePallet = "images/barcodepallet.pdf";

            UpdateInfoField("green", "SUCCESS", "The pallet is created. Barcode is shown below");
            if (Sound)
                jSRuntime.InvokeVoidAsync("playSound", "/sounds/palletbuilt.mp3");

            if (IsPhoenix)
            {
                //Print Rev
                Printing($"{CheckBarcodeBox.FirstOrDefault().Rev}");
            }

            ScannedBox = new List<FinishedGood>().AsEnumerable();

            if (ConfirmPallet)
            {
                VerifyTextBoxEnabled = true;
                //Goto verify
                await UpdateUI();
                await jSRuntime.InvokeVoidAsync("focusEditorByID", "VerifyScanField");
                FlashQtyColor(true);
                return;
            }

            await UpdateUI();
            await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
            return;
        }

        #endregion

        await ResetInfo();
        FlashQtyColor(true);
    }

    async void VersionChange(string value)
    {
        PORevision = value;
        if (PORevision != FirstRevisionOnPO) { PORevision = FirstRevisionOnPO; }
        IsWorking = false;
        IsReady = true;
        await UpdateUI();
        await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
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

    public async void PrintLabel(string content, string labelType, string selectedPrinter)
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
            barCode.DpiX = 72;
            barCode.DpiY = 72;
            barCode.Module = 0.7f;
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
                    {
                        // Draw a rectangle.
                        //using (var pen = new Pen(Color.Black, 1))
                        // graphics.DrawRectangle(pen, new RectangleF(2, 2, 68, 68));
                        //barCode.Save("wwwroot/images/barcodepallet.png", System.Drawing.Imaging.ImageFormat.Png);
                        //await Task.Delay(500);
                        //barCode.Dispose();
                        var img = Image.FromFile($"wwwroot/images/{labelType}.bmp");
                        graphics.DrawImage(img, new PointF(0, 2));

                        // Add graphics content to the document page.
                        graphics.AddToPageForeground(pdfPage, 72, 72);
                        img.Dispose();
                        graphics.Dispose();
                    }

                    //processor.SaveDocument($"wwwroot/images/{labelType}.pdf");
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

    public async void FlashQtyColor(bool good)
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

    async void UpdateInfoField(string cssTextColor, string? result = null, string? content = null, string? highlightMsg = null, bool reset = false)
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
            await ResetInfo(false);
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

    private async Task<bool> InsertPoNumber(string scanfield, string po)
    {

        var rs = await TraceDataService.InsertPurchaseOrderNo(scanfield, po);

        if (rs)
        {

            UpdateInfoField("green", "SUCCESS", "Database is updated. ", po);
        }
        else
        {

            UpdateInfoField("orange", "WARNING", "Carton is not updated");
        }

        return rs;
    }

    // The PrintPage event is raised for each page to be printed.
    private void pd_PrintPage(string content, object sender, PrintPageEventArgs ev)
    {
        float linesPerPage = 1;
        float yPos = 5; //9
        int count = 0;
        //float leftMargin = ev.MarginBounds.Left;
        //float topMargin = ev.MarginBounds.Top;
        float leftMargin = 0; //79
        float topMargin = 0;
        string? line = null;

        // Calculate the number of lines per page.
        //linesPerPage = ev.MarginBounds.Height / printFont.GetHeight(ev.Graphics);

        //// Iterate over the file, printing each line.
        //while (count < linesPerPage && ((line = content) != null))
        //{
        //    yPos = topMargin + (count * printFont.GetHeight(ev.Graphics));
        //    ev.Graphics.DrawString(line, printFont, Brushes.Black, leftMargin, yPos, new StringFormat());
        //    count++;
        //}
        ev.Graphics.DrawString(content, printFont, Brushes.Black, leftMargin, yPos, new StringFormat());
        // If more lines exist, print another page.
        //if (line != null)
        //    ev.HasMorePages = true;
        //else
        ev.HasMorePages = false;
    }

    // Print the file.
    public async void Printing(string content)
    {
        if (ForceDoNotPrint) return;
        //Stream stream = null;
        int timeout = 10000;
        var task = Task.Run(() =>
        {
            try
            {
                //WebClient client = new WebClient();
                //Stream stream = client.OpenRead("https://filesamples.com/samples/document/txt/sample3.txt");
                //stream = await GenerateStreamFromString(content);
                //streamToPrint = new StreamReader(stream);

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
        });


        if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
        {
            // task completed within timeout
            UpdateInfoField("green", "SUCCESS", "Print job finished");
        }
        else
        {

            UpdateInfoField("red", "ERROR", "Print job timeout");
            // timeout logic
        }
    }

    //private async Task<Stream> GenerateStreamFromString(string input)
    //{
    //    UTF8Encoding utf8 = new UTF8Encoding();
    //    MemoryStream stream = new MemoryStream();
    //    var streamWriter = new StreamWriter(stream);
    //    await streamWriter.WriteLineAsync(input);
    //    var encodedBytes = Encoding.ASCII.GetBytes(streamWriter.ToString());
    //    encodedBytes = utf8.GetPreamble();
    //    stream.Write(encodedBytes, 0, encodedBytes.Length);
    //    return stream;
    //}
}
