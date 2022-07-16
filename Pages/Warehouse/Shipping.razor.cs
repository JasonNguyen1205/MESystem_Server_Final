using System.Drawing.Printing;
using System.IO;
using System.Text;

using Blazored.Toast.Services;

using DevExpress.BarCodes;
using DevExpress.Blazor;
using DevExpress.Pdf;

using MESystem.Data.Location;
using MESystem.Data.TRACE;
using MESystem.LabelComponents;
using MESystem.Service;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

using MouseEventArgs = Microsoft.AspNetCore.Components.Web.MouseEventArgs;

namespace MESystem.Pages.Warehouse;

public partial class Shipping : ComponentBase
{
    [Inject]
    SessionValues SessionValues { get; set; }

    [Inject]
    IJSRuntime? jSRuntime { get; set; }

    [Inject]
    private TraceService? TraceDataService { get; set; }

    [Inject]
    IApiClientService? ApiClientService { get; set; }

    [Inject]
    PalleteLabel? PalletLabel { get; set; }

    [Inject]
    IToastService? Toast { get; set; }

    private string? tips;

    public string Tips { get => tips; set => tips = value; }

    public bool ComboBox1ReadOnly { get; set; }

    bool FormJustRead = true;

    public bool TextBoxEnabled { get; set; }

    /// <summary>
    /// string scanfield;
    /// </summary>
    string Scanfield { get => scanfield; set => scanfield = value; }
    string PoData { get; set; } = string.Empty;

    List<string>? Infofield { get; set; } = new();

    List<string>? InfoCssColor { get; set; } = new();

    string PoNumber { get; set; } = string.Empty;

    List<string?> PartNos { get; set; }

    string PartDescription { get; set; } = string.Empty;

    int RevisedQtyDue { get; set; } = 0;

    int QtyShipped { get; set; } = 0;

    public int QtyInShipQueue { get; set; }

    int QtyLeft { get; set; } = 0;

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

    private CustomerOrder? SelectedPoNumber { get; set; } = new();

    public IEnumerable<CustomerOrder>? CustomerOrderData { get; set; }

    public IEnumerable<CustomerRevision>? CustomerOrders { get; set; }

    //Scan for making palette only
    int QtyPerBox;
    int PaletteCapacity;

    public string? LoadingText { get; set; }

    public string CSSViewMode
    {
        get => cSSViewMode;
        set
        {
            cSSViewMode = value;
            _ = Task.Run(async () => await UpdateUI());
        }
    }

    public int TotalFgs { get; set; }

    public bool IsReady { get; set; }

    public bool IsWorking
    {
        get => isWorking;
        set
        {
            isWorking = value;
        }
    }

    //Just only one partial box
    public bool IsPartial { get; set; }

    IEnumerable<FinishedGood>? ScannedBox;

    public List<FinishedGood>? CurrentList { get; set; }

    public List<FinishedGood>? MasterList { get; set; }

    IEnumerable<FinishedGood>? TotalScannedBox;
    bool withoutPOmode;

    public bool OperationMode
    {
        get => operationMode;
        set
        {
            operationMode = value;

            CSSViewMode = value ? "collapse" : "";
            Tips = value ? "** Complete view mode" : "** Simple view mode";
        }
    }

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

    public IEnumerable<CustomerRevision>? StockRevision { get; private set; }

    public CustomerRevision? SelectedStockRevision { get; private set; }

    public string? CurrentIFSRevision { get; set; }

    public bool IsPhoenix { get; set; } = false;
    public bool IsBraun { get; set; } = false;

    public bool? IsDuplicated { get; set; } = false;

    public bool? IsQlyPartBiggerThanQlyBox { get; set; } = false;

    public bool? NoShowPhoenix { get; set; } = true;

    public bool ForceDoNotPrint { get; set; }

    public bool ConfirmPallet { get; set; }

    bool verifyPalletTextBoxEnabled;

    public bool VerifyPalletTextBoxEnabled
    {
        get => verifyPalletTextBoxEnabled;
        set => verifyPalletTextBoxEnabled = value;
    }

    bool verifyBoxTextBoxEnabled;

    public bool VerifyBoxTextBoxEnabled { get => verifyBoxTextBoxEnabled; set => verifyBoxTextBoxEnabled = value; }

    public List<string>? Result { get; set; }

    public List<string>? HighlightMsg { get; set; }

    public string? PhoenixPart { get; set; }

    public bool ShouldUpdateUI { get; private set; }

    public string PalletCode = string.Empty;
    private bool isWorking;

    public IEnumerable<string>? Printers { get; set; }

    public string? SelectedPrinter { get; set; }

    public bool Sound { get; set; }

    public List<string>? ShipmentIdList { get; set; }

    public IEnumerable<Shipment> Shipments { get; set; }

    public string? SelectedShipment { get => selectedShipment; set { selectedShipment = value; _ = Task.Run(async () => await UpdateUI()); } }

    public bool AllOpenedPOMode { get; set; }

    public string UserInput
    {
        get => _userInput;
        set => Task.Run(
                async () =>
                {
                    _userInput = value;
                    await Task.Delay(100);
                    await UpdateUI();
                });
    }

    public bool AllowInput { get; set; }

    private string? pORevision;
    private string? _userInput;
    private bool operationMode;
    private string? cSSViewMode;
    private string? selectedShipment;

    public string PORevision
    {
        get => pORevision;
        set => Task.Run(
                async () =>
                {
                    pORevision = value;
                    await UpdateUI();
                });
    }

    public string? FocusElement { get; set; }

    public string? ReadOnlyElement { get; set; }

    public string? PalletScanField { get; private set; }

    public string? BoxScanField { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        IsReady = false;
        ComboBox1ReadOnly = true;
        ShowScanBarcode = false;
        ShouldUpdateUI = true;
        await Task.CompletedTask;
        LoadingText = "Getting data...";
        CSSViewMode = "";
        OperationMode = false;
        SelectedShipment = "";
        SelectedPoNumber = new CustomerOrder { CustomerPoNo = "" };
        withoutPOmode = false;
    }

    async void BindPartNo(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            _ = await SetPartNo(SelectedPartNo);
        }
    }

    async void BindPoNo(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            CustomerOrderData = await TraceDataService.GetCustomerOrders();
            GetCustomerPo(CustomerOrderData
                .Where(_ => _.CustomerPoNo == PoNumber).Take(1).FirstOrDefault());
        }
    }

    public async void LoadOpenedPO()
    {
        CustomerOrderData = await TraceDataService.GetCustomerOrders();
        AllOpenedPOMode = true;
        await UpdateUI();
    }

    private Task OnError(string message)
    {
        Toast.ShowError(message, "Barcode Reader");
        StateHasChanged();
        return Task.CompletedTask;
    }

    protected override bool ShouldRender() { return ShouldUpdateUI; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (jSRuntime == null)
            {
                return;
            }

            LoadingText = "Please choose the PO...";

            AllowInput = false;
            AllOpenedPOMode = false;

            CustomerOrders = await TraceDataService.GetCustomerRevision(2, "", "", "", "");

            Shipments = await TraceDataService.GetLogisticData(shipmentId: "ALL") ?? new List<Shipment>();
            ShipmentIdList = new();
            foreach (Shipment s in Shipments.Where(s => s.ShipmentId != null).ToList())
            {
                if (!ShipmentIdList.Contains(s.ShipmentId))
                {
                    ShipmentIdList.Add(s.ShipmentId);
                }
            }


            ForceDoNotPrint = false;
            ComboBox1ReadOnly = false;
            InfoCssColor = new();
            Infofield = new();
            Result = new();
            HighlightMsg = new();

            PartNos = Shipments.Where(_ => _.ShipmentId == SelectedShipment).Select(_ => _.PartNo).ToList() ??
                new List<string?>();
            IsWorking = false;
            withoutPOmode = false;
            TextBoxEnabled = false;
            CheckBarcodeBox = new List<FinishedGood>().AsEnumerable();
            ScannedBox = new List<FinishedGood>().AsEnumerable();
            TotalScannedBox = new List<FinishedGood>().AsEnumerable();
            Title = "Making pallet";
            CustomerRevisionsDetail = new List<CustomerRevision>().AsEnumerable();
            SelectedPrinter = string.Empty;
            await UpdateUI();
            List<string>? printers = new();
            for (var i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
            {
                var pName = PrinterSettings.InstalledPrinters[i];
                printers.Add(pName);
                if (pName.Contains("SHARED_PRINTER"))
                {
                    SelectedPrinter = pName;
                }
            }
            Printers = printers.AsEnumerable();
            Sound = true;
            ShowScanBarcode = false;
            CSSViewMode = "";

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
        _ = ResetInfo(true);
        IsReady = false;
        ForceDoNotPrint = true;
        withoutPOmode = true;
        IsWorking = false;
        TextBoxEnabled = true;
        ConfirmPallet = true;
        await UpdateUI();
        FocusElement = "PartNoField";
        await UpdateUI();
    }

    //Set pallet capacity by PartNos
    private async Task<bool> SetQuantityPerPalette(string value)
    {
        if (int.TryParse(value, out var qty))
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
            QtyPerBox = await TraceDataService.GetQtyFromTrace(3, SelectedPartNo);
            if (QtyPerBox == 0)
            {
                UpdateInfoField("red", "ERROR", $"Invalid Part number");
                FocusElement = "PartNoField";
                SelectedPartNo = "";
                return false;
            }

            await UpdateUI();

            SelectedFamily = await TraceDataService.GetFamilyFromPartNo(SelectedFamily);

            PaletteCapacity = await TraceDataService.GetQtyFromTrace(6, SelectedPartNo);

            if (withoutPOmode)
            {
                IsReady = true;
                IsWorking = false;
                ForceDoNotPrint = true;
            }
            await UpdateUI();
            ReadOnlyElement = "PartNoField";
            FocusElement = "ShippingScanField";
            await UpdateUI();
            return true;
        }
        else
        {
            UpdateInfoField("red", "ERROR", $"Invalid Part number");
            return false;
        }
    }

    //Set pallet capacity by PartNos
    private async Task<bool> SetQuantityPerBox(string value)
    {
        if (int.TryParse(value, out var qty))
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
                PartDescription = string.Empty;
                RevisedQtyDue = 0;
                PoData = string.Empty;
                InfoCssColor = new();
                Infofield = new();
                withoutPOmode = false;
                PoNumber = string.Empty;
                PartDescription = string.Empty;
                Scanfield = string.Empty;
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

                SelectedShipment = string.Empty;
                SelectedPoNumber = new CustomerOrder { CustomerPoNo = string.Empty };                //Update UI
                await UpdateUI();
            }
            else
            {
                VerifyPalletTextBoxEnabled = false;
                VerifyBoxTextBoxEnabled = false;
                IsWorking = false;

                //CheckBarcodeBox = new List<FinishedGood>().AsEnumerable();
                await UpdateUI();
                await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
            }
        }
        catch (Exception)
        {

            VerifyPalletTextBoxEnabled = false;
            VerifyBoxTextBoxEnabled = false;
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
    public EventCallback<string> tSelectPO;
    private string scanfield = "";

    public async void ShipmentChanged(string shipment)
    {
        if (string.IsNullOrEmpty(shipment))
        {
            CustomerOrderData = new List<CustomerOrder>();
            await UpdateUI();
            return;
        }

        SelectedShipment = shipment;
        SelectedPoNumber = new CustomerOrder();
        await UpdateUI();
        IEnumerable<Shipment>? pOs = from _ in Shipments where _.ShipmentId == SelectedShipment select _;
        List<CustomerOrder>? list = new();
        foreach (Shipment? item in pOs)
        {
            list.Add(
                new CustomerOrder { CustomerPoNo = item.PoNo, PartNo = item.PartNo, RevisedQtyDue = item.PoTotalQty });
        }
        CustomerOrderData = new List<CustomerOrder>();
        await UpdateUI();
        CustomerOrderData = list.AsEnumerable();
        await UpdateUI();
        FocusElement = "ComboBox3";
        await UpdateUI();
    }
    public async void GetCustomerPoInShipment(CustomerOrder e)
    {
        if (e == null)
        {
            await ResetInfo(true);
            return;
        }
        //SelectedShipment=e.ShipmentId;
        await UpdateUI();
        CheckQtyPlanned = true;
        PoNumber = string.Empty;
        PartDescription = string.Empty;

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


        Title = e == null ? "Making pallet" : "Link carton <--> PO & Making pallet";
        IsReady = false;
        await UpdateUI();
        if (e != null)
        {
            SelectedPoNumber = e;
            ComboBox1ReadOnly = true;
            CustomerRevisionsDetail = new List<CustomerRevision>();
            CustomerOrder? values = SelectedPoNumber;
            Value = values;
            FormJustRead = false;
            TextBoxEnabled = true;
            PoNumber = e.CustomerPoNo;
            SelectedPartNo = e.PartNo;
            PartDescription = e.PartDescription;
            PoData = "FRIWO PN: " + SelectedPartNo + " - " + PartDescription;

            SelectedSO = values.OrderNo;

            try
            {

                if (string.IsNullOrEmpty(SelectedShipment))
                {
                    RevisedQtyDue = CustomerOrderData.Where(_ => _.CustomerPoNo == SelectedPoNumber?.CustomerPoNo).Sum(_ => _.RevisedQtyDue);

                    QtyInShipQueue = (await TraceDataService.GetQtyOfAddedPoNumbers(SelectedPoNumber.CustomerPoNo, SelectedPartNo, SelectedShipment))
                         .Count();
                    QtyLeft = RevisedQtyDue - QtyInShipQueue;

                }
                else
                {
                    RevisedQtyDue = Shipments.Where(_ => _.ShipmentId == SelectedShipment && _.PoNo == SelectedPoNumber.CustomerPoNo).FirstOrDefault().PoTotalQty;
                    QtyInShipQueue = (await TraceDataService.GetQtyOfAddedPoNumbers(SelectedPoNumber.CustomerPoNo, SelectedPartNo, SelectedShipment))
                        .Count();

                    QtyLeft = RevisedQtyDue - QtyInShipQueue;

                }




            }
            catch (Exception)
            {
                RevisedQtyDue = 10000;
                Toast.ShowError(
               $"Cannot get the information for this PO {SelectedPoNumber.CustomerPoNo}",
               "Missing information");

            }

            PoData = "FRIWO PN: " + SelectedPartNo + " - " + values.PartDescription ?? Shipments.Where(_ => _.PartNo == SelectedPartNo).FirstOrDefault().PartDesc;

            SelectedSO = values.OrderNo;

            //Enable verify pallet code
            ConfirmPallet = true;

            //Using for cases making pallet without PO no, such as BOSCH
            withoutPOmode = false;
            IsReady = true;


            await UpdateUI();


            //Get family
            CustomerRevisionsDetail = await TraceDataService.GetCustomerRevisionByPartNo(SelectedPoNumber.PartNo);
            await UpdateUI();
            if (CustomerRevisionsDetail.Count() > 0)
            {
                SelectedFamily = CustomerRevisionsDetail.First().ProductFamily;
            }
            else
            {
                if (CustomerOrders.Where(f => f.PartNo == SelectedPartNo).FirstOrDefault() != null)
                {
                    SelectedFamily = CustomerOrders.Where(f => f.PartNo == SelectedPartNo).FirstOrDefault()?.ProductFamily ??
                        "No family";
                }
                else
                {
                    SelectedFamily = await TraceDataService.GetFamilyFromPartNo(SelectedPartNo);
                }
            }
            //Smith
            VerifyPalletTextBoxEnabled = false;
            VerifyBoxTextBoxEnabled = false;
            TextBoxEnabled = true;

            if (IsPhoenix)
            {
                FocusElement = "ComboBox3";
                ReadOnlyElement = "ComboBox3";
            }
            else
            {
                FocusElement = "ShippingScanField";
                ReadOnlyElement = "ComboBox3";
            }

            //IsReady = false;
            await UpdateUI();
            _ = await GetNeededInfoByFamily(SelectedFamily);
            //Get info for making pallet
            QtyPerBox = await TraceDataService.GetQtyFromTrace(3, SelectedPartNo);
            PaletteCapacity = await TraceDataService.GetQtyFromTrace(6, SelectedPartNo);

            PartNos = new List<string?>();
            var temp = "";
            foreach (Shipment? item in Shipments)
            {
                if ((temp != item.PartNo || !PartNos.Any(_ => _ == item.PartNo)) &&
                    item.PoNo == SelectedPoNumber.CustomerPoNo)
                {
                    temp = item.PartNo;
                    PartNos.Add(item.PartNo);
                }
            }

            await UpdateUI();

        }
    }
    //Get CO info from PO number
    public async void GetCustomerPo(CustomerOrder values)
    {
        if (values == null)
        {
            return;
        }

        PartDescription = string.Empty;

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
        IsBraun = false;
        NoShowPhoenix = true;


        Title = values == null ? "Making pallet" : "Link carton <--> PO & Making pallet";
        IsReady = false;
        await UpdateUI();
        if (values != null)
        {

            SelectedPoNumber = values;
            ComboBox1ReadOnly = true;
            CustomerRevisionsDetail = new List<CustomerRevision>();
            Value = values;
            FormJustRead = false;
            TextBoxEnabled = true;
            SelectedPartNo = values.PartNo;
            PartDescription = values.PartDescription;
            PoData = "FRIWO PN: " + SelectedPartNo + " - " + PartDescription;

            SelectedSO = values.OrderNo;

            try
            {

                if (string.IsNullOrEmpty(SelectedShipment))
                {
                    RevisedQtyDue = CustomerOrderData.Where(_ => _.CustomerPoNo == SelectedPoNumber?.CustomerPoNo).Sum(_ => _.RevisedQtyDue);

                    QtyInShipQueue = (await TraceDataService.GetQtyOfAddedPoNumbers(SelectedPoNumber.CustomerPoNo, SelectedPartNo, null))
                         .Count();
                    QtyLeft = RevisedQtyDue - QtyInShipQueue;

                }
                else
                {
                    RevisedQtyDue = Shipments.Where(_ => _.PoNo == values.CustomerPoNo).FirstOrDefault().PoTotalQty;
                    QtyInShipQueue = (await TraceDataService.GetQtyOfAddedPoNumbers(SelectedPoNumber.CustomerPoNo, SelectedPartNo, SelectedShipment))
                        .Count();

                    QtyLeft = RevisedQtyDue - QtyInShipQueue;

                }




            }
            catch (Exception)
            {
                RevisedQtyDue = 99999;

                QtyLeft = RevisedQtyDue - QtyInShipQueue;
                Toast.ShowError(
               $"Cannot get the information for this PO {SelectedPoNumber.CustomerPoNo}",
               "Missing information");

            }

            PoData = "FRIWO PN: " + SelectedPartNo + " - " + values.PartDescription ?? Shipments.Where(_ => _.PartNo == SelectedPartNo).FirstOrDefault().PartDesc;

            SelectedSO = values.OrderNo;

            //Enable verify pallet code
            ConfirmPallet = true;

            //Using for cases making pallet without PO no, such as BOSCH
            withoutPOmode = false;
            IsReady = true;


            await UpdateUI();

            //Get family
            CustomerRevisionsDetail = await TraceDataService.GetCustomerRevisionByPartNo(SelectedPoNumber.PartNo);
            await UpdateUI();
            if (CustomerRevisionsDetail.Count() > 0)
            {
                SelectedFamily = CustomerRevisionsDetail.First().ProductFamily;
            }
            else
            {
                if (CustomerOrders.Where(f => f.PartNo == SelectedPartNo).FirstOrDefault() != null)
                {
                    SelectedFamily = CustomerOrders.Where(f => f.PartNo == SelectedPartNo).FirstOrDefault()?.ProductFamily ??
                        "No family";
                }
                else
                {
                    SelectedFamily = await TraceDataService.GetFamilyFromPartNo(SelectedPartNo);
                }
            }

            //Smith
            VerifyPalletTextBoxEnabled = false;
            VerifyBoxTextBoxEnabled = false;
            TextBoxEnabled = true;

            if (IsPhoenix)
            {
                FocusElement = "ComboBox3";
                ReadOnlyElement = "ComboBox3";
            }
            else
            {
                FocusElement = "ShippingScanField";
                ReadOnlyElement = "ComboBox3";
            }

            //IsReady = false;
            await UpdateUI();
            _ = await GetNeededInfoByFamily(SelectedFamily);
            //Get info for making pallet
            QtyPerBox = await TraceDataService.GetQtyFromTrace(3, SelectedPartNo);
            PaletteCapacity = await TraceDataService.GetQtyFromTrace(6, SelectedPartNo);

            PartNos = new List<string?>();
            var temp = "";
            foreach (Shipment? item in Shipments)
            {
                if ((temp != item.PartNo || !PartNos.Any(_ => _ == item.PartNo)) &&
                    item.PoNo == SelectedPoNumber.CustomerPoNo)
                {
                    temp = item.PartNo;
                    PartNos.Add(item.PartNo);
                }
            }
            await UpdateUI();
        }
    }

    //Check additional information by family
    private async Task<bool> GetNeededInfoByFamily(string? family = null)
    {
        IsPhoenix = false;
        IsBraun = false;
        //PORevision = "";
        if (family is null)
        {
            ////Toast.ShowError("Cannot find family for this PO", "Missing info");
            return false;
        }
        //
        // Check Phoenix
        if (family == "Phoenix")
        {
            IsPhoenix = true;
            NoShowPhoenix = false;

            try
            {
                StockRevision = TraceDataService.GetCustomerRevisionByPartNo(SelectedPartNo, SelectedFamily).Result;
                CurrentIFSRevision = CustomerRevisionsDetail.FirstOrDefault()?.Rev;
            }
            catch
            {
                CurrentIFSRevision = "";
                ////Toast.ShowWarning($"Cannot find the revision for part no {SelectedPartNo}", "Missing information");
            }
            try
            {
                if (StockRevision.Count() > 0)
                {
                    AllowInput = false;
                    PORevision = StockRevision.First().Rev;
                    Console.WriteLine(PORevision);
                    SelectedStockRevision = StockRevision.First();
                    await UpdateUI();
                }
                else
                {
                    AllowInput = true;
                    PORevision = "";
                    SelectedStockRevision = new CustomerRevision("00", 0);
                    await UpdateUI();
                }

                FirstRevisionOnPallet = "";
            }
            catch
            {
                PORevision = "";
                FirstRevisionOnPallet = "";
            }
        }

        if (family.Contains("Braun"))
        {
            IsBraun = true;
        }


        return true;
    }

    private async void OnValueChanged(string newValue)
    {
        //QtyLeft=int.Parse(new string(newValue.Where(c => char.IsDigit(c)).ToArray()));
        // //QtyOfTotalDevices = await TraceDataService.GetQtyOfAddedPoNumbers(PoNumber, PartNos);
        //QtyInShipQueue=TraceDataService.GetQtyOfAddedPoNumbers(SelectedPoNumber.CustomerPoNo, SelectedPartNo, SelectedShipment).Result
        //    .Count();
        //QtyLeft=QtyLeft-QtyInShipQueue;
        //CheckQtyPlanned=false;
        //await UpdateUI();
    }

    async void PopupClosing(PopupClosingEventArgs args)
    {
        if (IsPhoenix)
        {

            if (PORevision != "")
            {
                VerifyPalletTextBoxEnabled = false;
                IsWorking = false;
                IsReady = true;
                await UpdateUI();
                await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
                return;
            }
            IsReady = false;
            VerifyPalletTextBoxEnabled = false;
            await UpdateUI();
            await jSRuntime.InvokeVoidAsync("focusEditorByID", "RevCbx");
            return;
        }
        VerifyPalletTextBoxEnabled = false;
        IsWorking = false;
        IsReady = true;
        await UpdateUI();
        await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
    }

    private void GetInputfield(string content)
    {
        Scanfield = content;
        //await UpdateUI(); 1899692 B0000006895-5
    }

    private async void HandleInput(KeyboardEventArgs e)
    {
        //if(e.Key == "Enter")
        //{
        //    FocusElement = "PalletScanField";
        //}

        if (e.Key == "Enter")
        {
            var Scanfield = this.Scanfield;
            this.Scanfield = string.Empty;
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
            if (!string.IsNullOrEmpty(Scanfield))
                if (Scanfield.Contains("PartialPallet"))
                {
                    //var tempBarcodeBox = CheckBarcodeBox.First();
                    var maxPalletNo = await TraceDataService.GetMaxPaletteNumber(CheckBarcodeBox.FirstOrDefault().PartNo);
                    var PalletCode = CreatePalletBarcode(CheckBarcodeBox.FirstOrDefault().PartNo, maxPalletNo);

                    foreach (FinishedGood? item in ScannedBox)
                    {
                        if (item.BarcodeBox == null)
                        {
                            UpdateInfoField("red", "ERROR", $"Fail on update at ", $"{ScannedBox.ToList().IndexOf(item)}");
                            return;
                        }
                        if (item.BarcodeBox != null)
                        {
                            await TraceDataService.UpdateFinishedGood(item.BarcodeBox, PalletCode, maxPalletNo);
                        }
                    }

                    //Print Barcode
                    //PrintLabel(PalletCode, "barcodepallet", "Microsoft Print to PDF");

                    PrintLabel(PalletCode, "barcodepallet", SelectedPrinter);

                    BarcodePallet = "images/barcodepallet.pdf";

                    UpdateInfoField("green", "SUCCESS", "The pallet is created. Barcode is shown below");
                    if (Sound)
                    {
                        _ = jSRuntime.InvokeVoidAsync("playSound", "/sounds/palletbuilt.mp3");
                    }

                    if (IsPhoenix)
                    {
                        //Print Rev
                        Printing($"{CheckBarcodeBox.FirstOrDefault().Rev}");
                    }

                    // Print Po
                    foreach(var scanbox in ScannedBox)
                    {
                        // Print PO
                        Printing(SelectedPoNumber.CustomerPoNo);
                        // Insert Po or Invoice
                        _ = await InsertPoNumber(scanbox.BarcodeBox, SelectedPoNumber.CustomerPoNo, SelectedShipment);

                    }


                    ScannedBox = new List<FinishedGood>().AsEnumerable();

                    if (ConfirmPallet)
                    {
                        VerifyPalletTextBoxEnabled = true;
                        //Goto verify
                        await UpdateUI();
                        await jSRuntime.InvokeVoidAsync("focusEditorByID", "PalletScanField");
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
            if (ScannedBox != null && ScannedBox.Count() > 0 && ScannedBox.Any(_ => _.QtyBox < QtyPerBox))
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

                if (IsPartial && !IsBraun)
                {
                    UpdateInfoField("red", "ERROR", "More than one partial carton on this pallet");


                    TextBoxEnabled = true;
                    await UpdateUI();
                    await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
                    FlashQtyColor(true);
                    await ResetInfo();
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
                var samePo = false;
                //Check Invoice No
                if (CheckBarcodeBox.FirstOrDefault().InvoiceNumber is not null)
                {
                    //UpdateInfoField("orange", "WARNING", $"This carton already linked to PO: {CheckBarcodeBox.FirstOrDefault().InvoiceNumber}");
                    if (CheckBarcodeBox.FirstOrDefault().InvoiceNumber != SelectedPoNumber.CustomerPoNo)
                    {
                        UpdateInfoField(
                            "red",
                            "ERROR",
                            $"The carton is already linked to another PO {CheckBarcodeBox.FirstOrDefault().InvoiceNumber}");
                        await ResetInfo(false);
                        return;
                    }
                    else
                    {
                        UpdateInfoField(
                            "green",
                            "INFO",
                            $"{Scanfield} is re-printed label",
                            $"{SelectedPoNumber.CustomerPoNo}");
                        samePo = true;
                    }
                }
                else
                {
                    UpdateInfoField("green", "INFO", $"{Scanfield} is added to", $"{SelectedPoNumber.CustomerPoNo}");
                }


                if (CheckBarcodeBox.Count() > QtyLeft&&samePo == false)
                {
                    UpdateInfoField("red", "ERROR", $"Quantity check fail", $"{CheckBarcodeBox.Count()} > {QtyLeft}");
                    await ResetInfo(false);
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
                    foreach (FinishedGood? item in CheckBarcodeBox)
                    {
                        if (item.Rev == null || item.Rev != ver)
                        {
                            UpdateInfoField("red", "ERROR", "The customer version is not unique");
                            await ResetInfo(false);

                            return;
                        }
                    }

                    //Check cv is same as PO cv
                    var checkRevisionPO = PORevision == tempRevision;
                    if (!checkRevisionPO)
                    {
                        //Toast.ShowError("Different Phoenix Rev", "Wrong Rev");
                        IsWorking = false;

                        TextBoxEnabled = true;
                        UpdateInfoField(
                            "red",
                            "ERROR",
                            "The carton's customer version is not as same as pallet's customer version",
                            $"{tempRevision} <> {PORevision}");
                        await Task.Delay(5);
                        await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
                        await UpdateUI();
                        FlashQtyColor(false);
                        await ResetInfo(false);
                        return;
                    }
                    else
                    {
                        UpdateInfoField("green", "SUCCESS", "The carton's C/V is similar to PO");
                    }
                }
                #endregion

                // Insert Po or Invoice
                //_ = await InsertPoNumber(CheckBarcodeBox.FirstOrDefault().BarcodeBox, SelectedPoNumber.CustomerPoNo, SelectedShipment);

                // Print PO
                //Printing(SelectedPoNumber.CustomerPoNo);

                if (string.IsNullOrEmpty(SelectedShipment))
                {
                    RevisedQtyDue = CustomerOrderData.Where(_ => _.CustomerPoNo == SelectedPoNumber?.CustomerPoNo).Sum(_ => _.RevisedQtyDue);

                    QtyInShipQueue = (await TraceDataService.GetQtyOfAddedPoNumbers(SelectedPoNumber.CustomerPoNo, SelectedPartNo, SelectedShipment))
                         .Count();
                    QtyLeft = RevisedQtyDue - QtyInShipQueue;

                }
                else
                {
                    RevisedQtyDue = Shipments.Where(_ => _.ShipmentId == SelectedShipment && _.PoNo == SelectedPoNumber.CustomerPoNo).FirstOrDefault().PoTotalQty;
                    QtyInShipQueue = (await TraceDataService.GetQtyOfAddedPoNumbers(SelectedPoNumber.CustomerPoNo, SelectedPartNo, SelectedShipment))
                        .Count();

                    QtyLeft = RevisedQtyDue - QtyInShipQueue;

                }
                await UpdateUI();

            }

            #region Box is made pallet check
            IEnumerable<FinishedGood>? isUsed = await TraceDataService.CheckBoxInAnyPallete(Scanfield);
            if (isUsed != null && isUsed.Any())
            {
                UpdateInfoField(
                    "red",
                    "ERROR",
                    $"Carton is already packaged in pallet",
                    $"{isUsed.FirstOrDefault().BarcodePalette}");

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
            //    PORevision = CheckBarcodeBox.First().Barcode.Substring(7, 2);
            //Add box to list for making pallet
            List<FinishedGood>? t = ScannedBox.ToList();
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

            List<FinishedGood>? t1 = TotalScannedBox.ToList();
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

          

            #region Build Pallet when it is full
            //Check pallet is full
            if (ScannedBox.Count() >= PaletteCapacity)
            //if (ScannedBox.Count() >= 2)
            {
                //var tempBarcodeBox = CheckBarcodeBox.First();
                var maxPalletNo = await TraceDataService.GetMaxPaletteNumber(CheckBarcodeBox.FirstOrDefault().PartNo);
                var PalletCode = CreatePalletBarcode(CheckBarcodeBox.FirstOrDefault().PartNo, maxPalletNo);

                // Update Barcode Pallete
                foreach (FinishedGood? item in ScannedBox)
                {
                    if (item.BarcodeBox == null)
                    {
                        UpdateInfoField("red", "ERROR", $"Fail on update at ", $"{ScannedBox.ToList().IndexOf(item)}");
                        return;
                    }
                    if (item.BarcodeBox != null)
                    {
                        await TraceDataService.UpdateFinishedGood(item.BarcodeBox, PalletCode, maxPalletNo);
                    }
                }

                //Print Barcode
                //PrintLabel(PalletCode, "barcodepallet", "Microsoft Print to PDF");

                PrintLabel(PalletCode, "barcodepallet", SelectedPrinter);

                BarcodePallet = "images/barcodepallet.pdf";

                UpdateInfoField("green", "SUCCESS", "The pallet is created. Barcode is shown below");
                if (Sound)
                {
                    _ = jSRuntime.InvokeVoidAsync("playSound", "/sounds/palletbuilt.mp3");
                }

                if (IsPhoenix)
                {
                    //Print Rev
                    Printing($"{CheckBarcodeBox.FirstOrDefault().Rev}");
                }

                foreach(var scanbox in ScannedBox)
                {
                    // Print PO
                    Printing(SelectedPoNumber.CustomerPoNo);
                    // Insert Po or Invoice
                    _ = await InsertPoNumber(scanbox.BarcodeBox, SelectedPoNumber.CustomerPoNo, SelectedShipment);

                }

                if (IsPhoenix)
                {
                    //Print Rev
                    Printing($"{ScannedBox.Last().Rev}");
                }

                ScannedBox = new List<FinishedGood>().AsEnumerable();

                if (ConfirmPallet)
                {
                    VerifyPalletTextBoxEnabled = true;
                    //Goto verify
                    ReadOnlyElement = "ShippingScanField";
                    FocusElement = "PalletScanField";
                    FlashQtyColor(true);
                    return;
                }
                await ResetInfo(false);
                await UpdateUI();
                await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
                return;
            }
            #endregion

            await ResetInfo();
            FlashQtyColor(true);
        }

    }

    private async void VersionChange(CustomerRevision value)
    {
        if (value == null)
        {
            return;
        }

        if (value.Rev == null)
        {
            return;
        }

        if (value.Rev != StockRevision.First().Rev)
        {
            UpdateInfoField("red", "ERROR", "The lower CV is must be choosen");
            CheckQtyPlanned = true;
            IsReady = false;
            await UpdateUI();
            Toast.ShowError("The lower CV is must be choosen", "Please pick CV again");
            return;
        }


        PORevision = value.Rev;
        await UpdateUI();


        if (PORevision != SelectedStockRevision.Rev)
        {
            UpdateInfoField("red", "ERROR", "The lower CV is must be choosen");
            //CheckQtyPlanned = true;
            IsReady = false;
            FocusElement = "RevCbx";
            await UpdateUI();
            Toast.ShowWarning("The lower CV is must be choosen", "Please pick CV again");
            return;
        }

        IsWorking = false;
        IsReady = true;
        await UpdateUI();
        await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
    }

    public string CreatePalletBarcode(string partNo, int maxPallet)
    {
        var myString = maxPallet.ToString();
        //int countString = myString.Length;
        var loop = 10 - myString.Length;
        var resultString = string.Empty;
        resultString += partNo;
        resultString += " P";
        for (var i = 0; i < loop; i++)
        {
            resultString += '0';
        }
        resultString += myString;
        _ = jSRuntime.InvokeAsync<string>("ConsoleLog", resultString);
        return resultString;
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
            DirectoryInfo info = new($"wwwroot/images/{labelType}.bmp");
            if (info.Exists)
            {
                info.Delete();
            }
            barCode.Save($"wwwroot/images/{labelType}.bmp", System.Drawing.Imaging.ImageFormat.MemoryBmp);
            barCode.Dispose();

            if (labelType == "barcodepallet")
            {
                using PdfDocumentProcessor processor = new();
                //Find a printer containing 'PDF' in its name.
                var printerName = SelectedPrinter;
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
                BarcodePallet = string.Empty;
                //PalletLabel.Content = DateTime.Now.ToString();
                //await PalletLabel.SetParametersAsync(new ParameterView());
                LabelContent = PalletCode;
                PalletCode = string.Empty;
                processor.Dispose();
                await UpdateUI();
            }
        }
        catch (Exception)
        {
        }
    }

    public static void SaveStreamAsFile(string filePath, Stream inputStream, string fileName)
    {
        DirectoryInfo info = new(filePath);
        if (!info.Exists)
        {
            info.Create();
        }

        var path = Path.Combine(filePath, fileName);
        using FileStream outputFileStream = new(path, FileMode.Create);
        inputStream.CopyTo(outputFileStream);
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
            await ResetInfo(false);
            if (Sound)
            {
                await jSRuntime.InvokeVoidAsync("playSound", "/sounds/alert.wav");
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

    private async Task<bool> InsertPoNumber(string scanfield, string po, string selectedShipment)
    {
        var rs = await TraceDataService.InsertPurchaseOrderNo(scanfield, po, selectedShipment);

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
        float yPos = 5; //9
                        //float leftMargin = ev.MarginBounds.Left;
                        //float topMargin = ev.MarginBounds.Top;
        float leftMargin = 0; //79

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
        if (ForceDoNotPrint)
        {
            return;
        }
        //Stream stream = null;
        var timeout = 10000;
        Task? task = Task.Run(
            () =>
            {
                try
                {
                    //WebClient client = new WebClient();
                    //Stream stream = client.OpenRead("https://filesamples.com/samples/document/txt/sample3.txt");
                    //stream = await GenerateStreamFromString(content);
                    //streamToPrint = new StreamReader(stream);

                    printFont = new Font("Arial", 23);
                    PrintDocument pd = new();
                    pd.PrintPage += new PrintPageEventHandler((s, e) => pd_PrintPage(content, s, e));

                    // Print the document.
                    pd.PrinterSettings.PrinterName = SelectedPrinter;
                    pd.Print();
                }
                catch (Exception ex)
                {
                    var test = ex.ToString();
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


    private void HandleVerifyInput(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            if (string.IsNullOrEmpty(PalletScanField))
            {
                return;
            }

            ReadOnlyElement = "PalletScanField";
            FocusElement = "BoxScanField";
        }
    }

    private async void HandleBoxVerifyInput(KeyboardEventArgs e)
    {
        //if(e.Key == "Enter")
        //{
        //    FocusElement = "ShippingScanField";
        //}
        if (e.Key == "Enter")
        {

            IsWorking = true;
            if (string.IsNullOrEmpty(BoxScanField))
            {
                return;
            }

            IEnumerable<FinishedGood>? ScannedBoxsInPallet = await TraceDataService?.GetPalletContentInformation(PalletScanField) ??
                new List<FinishedGood>();
            if (ScannedBoxsInPallet.Count() > 0)
            {
                FinishedGood? FirstBoxInPallet = ScannedBoxsInPallet.FirstOrDefault();
                var CurrentFgs = FirstBoxInPallet?.QtyPallet;
                var StandardFgs = PaletteCapacity * QtyPerBox;
                if (StandardFgs != CurrentFgs)
                {

                    var i = await TraceDataService.VerifyPallet(PalletScanField, -1, SelectedShipment);
                    var j = await TraceDataService.VerifyBoxPallet(PalletScanField, -1, SelectedShipment, BoxScanField);
                    PalletScanField = string.Empty;
                    BoxScanField = string.Empty;

                    if (!i)
                    {
                        FocusElement = "PalletScanField";
                        UpdateInfoField("red", "FAIL", "Invalid code", "Scanned code is not pallet");
                        await UpdateUI();
                        return;
                    }

                    if (!j)
                    {
                        FocusElement = "PalletScanField";
                        UpdateInfoField("red", "FAIL", "Invalid code", "Box and pallet missmatch ");
                        await UpdateUI();
                        return;
                    }

                    await UpdateUI();

                    IsWorking = false;
                    VerifyPalletTextBoxEnabled = false;
                    VerifyBoxTextBoxEnabled = false;
                    UpdateInfoField("orange", "WARNING", "Verifying Pallet", "Quantity is less than standard");
                    await UpdateUI();
                    ReadOnlyElement = "BoxScanField";
                    FocusElement = "ShippingScanField";
                    await UpdateUI();
                    return;
                }
                else
                {
                    var i = await TraceDataService.VerifyPallet(PalletScanField, -1, SelectedShipment);
                    var j = await TraceDataService.VerifyBoxPallet(PalletScanField, -1, SelectedShipment, BoxScanField);
                    PalletScanField = string.Empty;
                    BoxScanField = string.Empty;


                    if (!i)
                    {
                        FocusElement = "PalletScanField";
                        UpdateInfoField("red", "FAIL", "Invalid code", "Scanned code is not pallet");
                        await UpdateUI();
                        return;
                    }

                    if (!j)
                    {
                        FocusElement = "PalletScanField";
                        UpdateInfoField("red", "FAIL", "Invalid code", "Box and pallet missmatch ");
                        await UpdateUI();
                        return;
                    }

                    await UpdateUI();

                    IsWorking = false;
                    UpdateInfoField("green", "SUCCESS", "Verifying Pallet", "Pallet is verified");
                    await UpdateUI();
                    ReadOnlyElement = "BoxScanField";
                    FocusElement = "ShippingScanField";
                    await UpdateUI();
                    return;
                }
            }
            else
            {
                BoxScanField = string.Empty;
                PalletScanField = string.Empty;
                ReadOnlyElement = "BoxScanField";
                FocusElement = "PalletScanField";
                UpdateInfoField("red", "FAIL", "Invalid code", "Verify pallet failed");
                await UpdateUI();
                return;
            }
        }
    }
}
