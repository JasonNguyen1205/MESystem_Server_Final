using MESystem.Data.TRACE;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace MESystem.Pages.Warehouse;

public partial class ShipOutPallet : ComponentBase
{

    bool verifyValue;

    [Parameter]
    public bool VerifyValue { get => verifyValue; set { if(verifyValue==value) { return; } verifyValue=value; _=VerifyValueChanged.InvokeAsync(value); } }

    [Parameter]
    public EventCallback<bool> VerifyValueChanged { get; set; }

    string? PalletScanfield { get; set; }
    string? BoxScanfield { get; set; }

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


    [Parameter]
    public IEnumerable<CustomerRevision>? CustomerRevisionsDetail { get; set; }

    IEnumerable<FinishedGood>? CheckBarcodeBox { get; set; }

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
    string? barcodePallete;
    string? barcodeBox;
    public string? BarcodePallete { get => barcodePallete; set { if(barcodePallete==value) { return; } barcodePallete=value; _=InputVerifyField.InvokeAsync(value); } }
    public string? BarcodeBox { get => barcodeBox; set { if(barcodeBox==value) { return; } barcodeBox=value; _=InputBoxVerifyField.InvokeAsync(value); } }

    [Parameter]
    public EventCallback<string> InputVerifyField { get; set; }

    [Parameter]
    public EventCallback<KeyboardEventArgs> VerifyHandleInput { get; set; }

    [Parameter]
    public bool VerifyBoxTextBoxEnabled { get; set; }

    [Parameter]
    public EventCallback<string> InputBoxVerifyField { get; set; }

    [Parameter]
    public EventCallback<KeyboardEventArgs> VerifyBoxHandleInput { get; set; }

    KeyboardEventArgs keyboardEventArgs;
    [Parameter]
    public KeyboardEventArgs KeyboardEventArgs { get => keyboardEventArgs; set { keyboardEventArgs=value; _=VerifyBoxHandleInput.InvokeAsync(value); _=VerifyHandleInput.InvokeAsync(value); } }

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
        if(firstRender)
        {


        }
    }

    async Task UpdateUI()
    {
        //Update UI
        if(ShouldRender())
        {
            await Task.Delay(5);
            await InvokeAsync(StateHasChanged);
        }
#if DEBUG
        Console.WriteLine("UI is updated");
#endif
    }


}
