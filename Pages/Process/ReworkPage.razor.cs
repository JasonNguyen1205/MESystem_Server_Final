
using MESystem.Service;
using MESystem.Data.TRACE;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using DevExpress.Blazor;
using Microsoft.JSInterop;

namespace MESystem.Pages.Process;

public partial class ReworkPage : ComponentBase
{
    private string selectedNgCode;

    [Inject]
    private TraceService? TraceDataService { get; set; }
 
    public string? FocusElement { get; set; }
    [Inject]
    IJSRuntime? jSRuntime { get; set; }

    public string? ReadOnlyElement { get; set; }
    bool ShowCheckboxes { get; set; } = true;
    public IEnumerable<Rework>? Data { get; set; }

    public string SelectedNgCode { get => selectedNgCode; set { selectedNgCode = value;  UpdateUI(); } }
    public List<string> NgCodeList { get; set; } = new List<string>();
    //DataGridFilteringMode FilteringMode { get; set; } = DataGridFilteringMode.Contains;
    //string? ng_Code { get; set; }
    //string? last_Code { get; set; }
    //string? first_Code { get; set; }
    string? remark { get; set; }
    public bool IsReady { get; set; }
    public string? ng_data { get; set; }
    public string? barcode { get; set; }
    int flag { get; set; }
    public Rework SelectedRework { get; set; } = new();
    protected override async Task OnInitializedAsync()
    {

        Data = await TraceDataService.GetNGCode();
        if (Data.Count() > 0)
        {

            foreach (Rework r in Data)
            {
                Rework temp = new();
                if (!string.IsNullOrEmpty(r.NG_Description_VN))
                {
                    NgCodeList.Add(r.NG_Description_VN);
                    //int? ngNo = int.Parse((r.NG_Description_VN).Split(".")[0]);
                    //temp.NG_Code = ngNo;
                    //temp.NG_Description_VN = (r.NG_Description_VN).Split(".")[1];
                    //if (temp != null)
                    //    NgCodeList.Add(temp);
                }

            }
            NgCodeList.Sort();
            await UpdateUI();
        }

        flag = 0;
    }
    async void GetData()
    {

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
    public async void Enter(KeyboardEventArgs e)
    {
        if (e.Code == "Enter")
        {
            SelectedNgCode = await jSRuntime.InvokeAsync<string>("getValueById", "ngCode");
            SelectedRework = Data.Where(e => e.NG_Description_VN.Contains(SelectedNgCode.ToUpper())).FirstOrDefault();
            if(SelectedRework != null)
            {
                SelectedNgCode = SelectedRework.NG_Description_VN;
            }
            await UpdateUI();
        }
    }

    public async void NgCodeChange(string ngCode)
    {
        if (ngCode != null)
            SelectedNgCode = ngCode;
        await UpdateUI();
        //if (string.IsNullOrEmpty(shipment))
        //{
        //    CustomerOrderData = new List<CustomerOrder>();
        //    await UpdateUI();
        //    return;
        //}

        //SelectedShipment = shipment;
        //SelectedPoNumber = new CustomerOrder();
        //await UpdateUI();
        //IEnumerable<Shipment>? pOs = from _ in Shipments where _.ShipmentId == SelectedShipment select _;
        //List<CustomerOrder>? list = new();
        //foreach (Shipment? item in pOs)
        //{
        //    list.Add(
        //        new CustomerOrder { CustomerPoNo = item.PoNo, PartNo = item.PartNo, RevisedQtyDue = item.PoTotalQty });
        //}
        //CustomerOrderData = new List<CustomerOrder>();
        //await UpdateUI();
        //CustomerOrderData = list.AsEnumerable();
        //await UpdateUI();
        //FocusElement = "ComboBox3";
        //await UpdateUI();
    }
    //private async void HandleCodeInput(KeyboardEventArgs e)
    //{
    //    if (e.Key == "Enter")
    //    {
    //        if (flag == 0)
    //        {
    //            FocusElement = "ng_Code2";
    //            ReadOnlyElement = "ng_Code1";
    //            flag = 1;
    //        }
    //        else
    //        {
    //            FocusElement = "barcode";
    //            ReadOnlyElement = "ng_Code2";
    //            flag = 0;
    //            ng_Code = first_Code + last_Code;
    //            ng_data = Data.Where(e => e.NG_Description_VN.Split(".")[0].ToString() == ng_Code).FirstOrDefault()?.NG_Description_VN ?? "Wrong Rework Code!";
    //        }
    //    }

    //}

    private async void HandleBarcodeInput(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            TraceDataService.GetBarcodeLink(barcode);
            // Rework input_data = new Rework(barcode,null,int.Parse(ng_Code),"","","","");
            // TraceDataService.InsertReworkData(input_data);
        }
    }

    private async void HandleRemarkInput(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            FocusElement = "barcode";
            ReadOnlyElement = "remark";
        }
    }
}
