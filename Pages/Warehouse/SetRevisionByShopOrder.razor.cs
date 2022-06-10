using Blazored.Toast.Services;
using MESystem.Data;
using MESystem.Data.TRACE;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace MESystem.Pages.Warehouse;

public partial class SetRevisionByShopOrder : ComponentBase
{
    [Inject]
    IJSRuntime jSRuntime { get; set; }

    [Inject]
    TraceService? TraceDataService { get; set; }

    [Inject]
    IToastService Toast { get; set; }

    public IEnumerable<RevisionOrder> OrderNoData { get; set; } = new List<RevisionOrder>().AsEnumerable();
    public List<RevisionOrder> OrderNoDataList { get; set; } = new List<RevisionOrder>();
    //public List<Revision> RevisionDataList { get; set; }
    public bool EnableButton { get; set; }
    public bool ShowRevCombox { get; set; }

    public bool SetRevisionSwitch { get; set; }

   public string SelectedOrderNo { get; set; }
    public string SelectedRevision { get; set; }

    public string SearchSelectedOrderNo { get; set; }

  

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            EnableButton = false;
            ShowRevCombox = false;
            OrderNoData = new List<RevisionOrder>().AsEnumerable();
            OrderNoData = await TraceDataService.GetRevisionByShopOrder("");
            if (OrderNoData.Count() > 0)
                OrderNoDataList = OrderNoData.ToList();

            await UpdateUI();
        }
    }

    async void ResetInfo(bool backToStart)
    {
        if (backToStart)
        {
            //Scanfield = "";
            //Scanfield2 = "";
            //Update UI
            await UpdateUI();
        }
        else
        {
            //Scanfield = string.Empty;
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
#if DEBUG
        Console.WriteLine("UI is updated");
#endif
    }


   

    public async Task SaveRevision(string revision, string orderNo)
    {

        await jSRuntime.InvokeVoidAsync("ConsoleLog", "ShopOrder: " + orderNo);
        await jSRuntime.InvokeVoidAsync("ConsoleLog", "Revision: " + revision);
    }

    public async Task LoadRevBySO(string orderNo)
    {

    }
    
    public async Task SearchText(string orderNo)
    {
        if(orderNo != "")
        {
            SearchSelectedOrderNo = orderNo;
        }
    }

    public async Task SearchOrderNo(KeyboardEventArgs e)
    {
        if(e.Key == "Enter")
        {
            if (SearchSelectedOrderNo != null)
            {
                OrderNoData = new List<RevisionOrder>().AsEnumerable();
                OrderNoData = await TraceDataService.GetRevisionByShopOrder(SearchSelectedOrderNo);
                if (OrderNoData.Count() > 0)
                {
                    OrderNoDataList = new List<RevisionOrder>();
                    OrderNoDataList = OrderNoData.ToList();
                }
                
            }
            await UpdateUI();
            await jSRuntime.InvokeVoidAsync("ConsoleLog", "ShopOrderSearch: " + SearchSelectedOrderNo);
        }
    }
}
