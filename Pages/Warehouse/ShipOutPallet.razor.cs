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
    IJSRuntime jSRuntime { get; set; }
    [Inject]
    IfsService? IfsService { get; set; }
    [Inject]
    TraceService? TraceDataService { get; set; }
    [Inject]
    IApiClientService? ApiClientService { get; set; }
    [Inject]
    PalleteLabel? PalleteLabel { get; set; }
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

    [Parameter]
    public CustomerOrder Value
    {
        get => valueprop;
        set { valueprop = value; Title = Value == null ? "Making palette" : "Link Box <--> PO No. & Making palette"; StateHasChanged(); }
    }

    [Parameter]
    public bool IsPopUp { get; set; }

    IEnumerable<CustomerOrder>? SelectedPoNumber { get; set; }
    IEnumerable<CustomerOrder>? CustomerOrderData { get; set; }

    //Scan for making palette only
    int QtyPerBox;
    int PaletteCapacity;
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

    //public string barcodeBase64Img{get;set;}
    //string content;
    //BarcodeWriter writer;

    IEnumerable<FinishedGood>? CheckBarcodeBox { get; set; }
    IEnumerable<ModelProperties>? ModelProperties { get; set; }
    public string? SelectedFamily { get; set; }
    public string? SelectedPartNo { get; set; }
    public string? SelectedSO { get; set; }
    public string? FirstRevisionOnPallete { get; set; }
    public string? FirstRevisionOnPO { get; set; }
    public string? CurrentIFSRevision { get; set; }

    public bool IsPhoenix { get; set; } = false;
    public bool? IsDuplicated { get; set; } = false;
    public bool? IsQlyPartBiggerThanQlyBox { get; set; } = false;
    public bool? NoShowPhoenix { get; set; } = true;
    public bool ForcePrint { get; set; }

    public string PalleteCode = "";


    protected override async Task OnInitializedAsync()
    {
        CustomerOrderData = await TraceDataService.GetCustomerOrders();
        Infofield = new();
        SelectedPoNumber = CustomerOrderData.Take(0);
        IsReady = false;
        withoutPOmode = false;
        TextBoxEnabled = false;
        CheckBarcodeBox = new List<FinishedGood>().AsEnumerable();
        await UpdateUI();
        ScannedBox = new List<FinishedGood>().AsEnumerable();
        TotalScannedBox = new List<FinishedGood>().AsEnumerable();
        Title = "Making pallete";
        CustomerRevisionsDetail = new List<CustomerRevision>().AsEnumerable();
        if (IsPopUp)
        {
            GetCustomerPo(Value);
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            CustomerOrderData = await TraceDataService.GetCustomerOrders();
            Infofield = new();
            SelectedPoNumber = CustomerOrderData.Take(0);
            IsReady = false;
            withoutPOmode = false;
            TextBoxEnabled = false;
            CheckBarcodeBox = new List<FinishedGood>().AsEnumerable();
            await UpdateUI();
            ScannedBox = new List<FinishedGood>().AsEnumerable();
            TotalScannedBox = new List<FinishedGood>().AsEnumerable();
            Title = "Making pallete";
            CustomerRevisionsDetail = new List<CustomerRevision>().AsEnumerable();
            if (IsPopUp)
            {
                GetCustomerPo(Value);
            }
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
            Scanfield = "";
            TextBoxEnabled = true;
            await UpdateUI();
            await jSRuntime.InvokeVoidAsync("focusEditorByID", "verifyScanField");
        }
    }

    //Edit quantity
    public async void EditShipmentQty(MouseEventArgs e)
    {
        await Task.Delay(5);
        CheckQtyPlanned = true;
        await UpdateUI();
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

    //Get CO info from PO number
    async void GetCustomerPo(CustomerOrder values)
    {

        Title = values == null ? "Making palette" : "Link Box <--> PO No. & Making palette";

        if (values is not null)
        {
            try
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
                CheckQtyPlanned = true;
                SelectedPartNo = PartNo;
                SelectedSO = values.OrderNo;

                //Using for cases making pallete without PO no, such as BOSCH
                withoutPOmode = false;

                //Get info for making pallete
                QtyPerBox = await TraceDataService.GetQtyFromTrace(3, SelectedPartNo);
                PaletteCapacity = await TraceDataService.GetQtyFromTrace(6, SelectedPartNo);
                await UpdateUI();

            }
            catch (Exception)
            {
                QtyPerBox = 0;
                Toast.ShowWarning($"Cannot find the number box/pallete for part no {SelectedPartNo}", "Missing information");
            }

            //Get family
            CustomerRevisionsDetail = await TraceDataService.GetCustomerRevision(0, $"{PoNumber}", "", "", "");
            if (CustomerRevisionsDetail != null)
                try
                {
                    SelectedFamily = CustomerRevisionsDetail.FirstOrDefault()?.ProductFamily;
                }
                catch (Exception)
                {
                    SelectedFamily = "Not found from IFS";
                    Toast.ShowWarning($"Cannot find the prod family for part no {SelectedPartNo}", "Missing information");
                }
            else
            {
                SelectedFamily = "Not found from IFS";
                Toast.ShowWarning($"Cannot find the prod family for part no {SelectedPartNo}", "Missing information");
            }

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
                CurrentIFSRevision = "null";
                Toast.ShowWarning($"Cannot find the revision for part no {SelectedPartNo}", "Missing information");
            }
            try
            {

                FirstRevisionOnPO = await TraceDataService.GetCustomerVersion(0, PoNumber);
                FirstRevisionOnPallete = "null";
            }
            catch
            {
                FirstRevisionOnPO = "null";
                FirstRevisionOnPallete = "null";
                //$"There is no FGs has been shipped for PO {SelectedPoNumber}";
                //Toast.ShowWarning($"Cannot find the  family for part no {SelectedPartNo}", "Missing information");
            }
        }
    }

    private void GetInputfield(string content)
    {
        Scanfield = content;
    }

    private void HandleInput(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
        }

    }
}
