using Blazored.Toast.Services;
using MESystem.Data;
using MESystem.Data.TRACE;
using MESystem.LabelComponents;
using MESystem.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MouseEventArgs = Microsoft.AspNetCore.Components.Web.MouseEventArgs;

namespace MESystem.Pages.Warehouse;

public partial class ShipOutPallet : ComponentBase
{
    [Inject]
    IJSRuntime? jSRuntime { get; set; }

    [Inject]
    IfsService? IfsService { get; set; }

    [Inject]
    TraceService? TraceDataService { get; set; }

    [Inject]
    IApiClientService? ApiClientService { get; set; }

    [Inject]
    PalleteLabel? PalleteLabel { get; set; }

    [Inject]
    IToastService? Toast { get; set; }

    bool FormJustRead = true;

    bool textBoxEnabled;

    bool verifyValue;
    [Parameter]
    public bool VerifyValue { get => verifyValue; set { if (verifyValue == value) return; verifyValue = value; VerifyValueChanged.InvokeAsync(value); } }

    [Parameter]
    public EventCallback<bool> VerifyValueChanged { get; set; }

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

    private CustomerOrder? valueprop;

    [Parameter]
    public CustomerOrder Value
    {
        get => valueprop;
        set
        {
            valueprop = value;
            Title = "Verify Pallet";
            StateHasChanged();
        }
    }

    [Parameter]
    public bool IsPopUp { get; set; }

    IEnumerable<CustomerOrder>? SelectedPoNumber { get; set; }

    IEnumerable<CustomerOrder>? CustomerOrderData { get; set; }

    //Scan for making palette only

    [Parameter]
    public int QtyPerBox { get; set; }

    [Parameter]
    public int PaletteCapacity { get; set; }


    public bool ShowScanBarcode { get; set; }


    public int TotalFgs { get; set; }

    public bool IsReady { get; set; }

    IEnumerable<FinishedGood>? ScannedBox;
    IEnumerable<FinishedGood>? TotalScannedBox;
    bool withoutPOmode;

    //Canvas for barcode                        
    public string? barcodeImg { get; set; }

    public string? LabelContent { get; set; }

    public string? BarcodePallete { get; set; }

    [Parameter]
    public IEnumerable<CustomerRevision>? CustomerRevisionsDetail { get; set; }

    IEnumerable<FinishedGood>? CheckBarcodeBox { get; set; }

    IEnumerable<ModelProperties>? ModelProperties { get; set; }

    public string? SelectedFamily { get; set; }

    public string? SelectedPartNo { get; set; }

    public string? SelectedSO { get; set; }

    public string? FirstRevisionOnPallete { get; set; }

    public string? DefaultUsedCV { get; set; }

    public string? CurrentIFSRevision { get; set; }

    public bool IsPhoenix { get; set; } = false;

    public bool? IsDuplicated { get; set; } = false;

    public bool? IsQlyPartBiggerThanQlyBox { get; set; } = false;

    public bool? NoShowPhoenix { get; set; } = true;

    public bool ForceDoNotPrint { get; set; }

    [Parameter]
    public string SelectedShipment { get; set; }
    [Parameter]
    public bool VerifyTextBoxEnabled { get; set; }

    public string PalleteCode = string.Empty;

    public int StandardFgs { get; set; } = 0;

    public int CurrentFgs { get; set; } = 0;

    public string? InfoColor { get; set; }

    public IEnumerable<FinishedGood>? ScannedBoxsInPallet { get; set; }

    public FinishedGood? FirstBoxInPallet { get; set; }

    protected override bool ShouldRender() { return true; }

    protected override Task OnParametersSetAsync() { return InvokeAsync(StateHasChanged); }


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            ForceDoNotPrint = false;
            CustomerOrderData = await TraceDataService.GetCustomerOrders();
            Infofield = new();
            SelectedPoNumber = CustomerOrderData.Take(0);
            IsReady = false;
            withoutPOmode = false;
            CheckBarcodeBox = new List<FinishedGood>().AsEnumerable();
            await UpdateUI();
            ScannedBox = new List<FinishedGood>().AsEnumerable();
            TotalScannedBox = new List<FinishedGood>().AsEnumerable();
            Title = "Making pallete";
            CustomerRevisionsDetail = new List<CustomerRevision>().AsEnumerable();
            if (IsPopUp)
            {
                //GetCustomerPo(Value);
            }
        }
    }

    async void ResetInfo(bool backToStart)
    {
        if (backToStart)
        {
            FormJustRead = true;
            VerifyValue = false;
            PoNumber = string.Empty;
            PartNo = string.Empty;
            PartDescription = string.Empty;
            RevisedQtyDue = 0;
            PoData = string.Empty;
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
            VerifyValue = false;
            await UpdateUI();
            await jSRuntime.InvokeVoidAsync("focusEditorByID", "VerifyScanField");
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
#if DEBUG
        Console.WriteLine("UI is updated");
#endif
    }

    private void GetInputfield(string content) { Scanfield = content; }

    private async void HandleInput(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            GetInputfield(Scanfield);
            if (string.IsNullOrEmpty(Scanfield))
                return;
            ScannedBoxsInPallet = await TraceDataService.GetPalletContentInformation(Scanfield);
            if (ScannedBoxsInPallet.Count() > 0)
            {
                FirstBoxInPallet = ScannedBoxsInPallet.FirstOrDefault();
                CurrentFgs = FirstBoxInPallet.QtyPallet;
                StandardFgs = PaletteCapacity * QtyPerBox;
                if (StandardFgs != CurrentFgs)
                {
                    InfoColor = "red";
                    await UpdateUI();
                    await TraceDataService.VerifyPallet(Scanfield, -1, SelectedShipment);
                    Scanfield = string.Empty;

                    await UpdateUI();
                    if (IsPopUp)
                    {
                        VerifyValue = false;
                        VerifyTextBoxEnabled = false;
                        await UpdateUI();
                        await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
                    }
                    else
                    {
                        VerifyValue = false;
                        VerifyTextBoxEnabled = false;
                        await UpdateUI();
                        await jSRuntime.InvokeVoidAsync("focusEditorByID", "VerifyScanField");
                    }
                }
                else
                {
                    InfoColor = "green";
                    await UpdateUI();
                    await TraceDataService.VerifyPallet(Scanfield, 1, SelectedShipment);
                    Scanfield = string.Empty;

                    if (IsPopUp)
                    {
                        VerifyValue = false;
                        VerifyTextBoxEnabled = false;
                        await UpdateUI();
                        await jSRuntime.InvokeVoidAsync("focusEditorByID", "ShippingScanField");
                    }
                    else
                    {
                        VerifyValue = false;
                        VerifyTextBoxEnabled = false;
                        await UpdateUI();
                        await jSRuntime.InvokeVoidAsync("focusEditorByID", "VerifyScanField");
                    }

                }
            }
            else
            {
                InfoColor = "red";
                Scanfield = string.Empty;
                //verifyValue = false;
                //VerifyTextBoxEnabled = false;
                await UpdateUI();
                await jSRuntime.InvokeVoidAsync("focusEditorByID", "VerifyScanField");
            }
        }
    }
}
