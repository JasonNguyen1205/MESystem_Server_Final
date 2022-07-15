using Blazored.Toast.Services;

using MESystem.Data.TRACE;
using MESystem.Service;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace MESystem.Pages.Warehouse;

public partial class SetRevisionByShopOrder : ComponentBase
{
    private CustomerRevision? selectedRevision;

    [Inject]
    IJSRuntime? jSRuntime { get; set; }

    [Inject]
    TraceService? TraceDataService { get; set; }

    [Inject]
    IToastService? Toast { get; set; }

    public IEnumerable<CustomerRevision>? OrderNoData { get; set; }
    public List<CustomerRevision> OrderNoDataList { get; set; } = new List<CustomerRevision>();

    public bool EnableButton { get; set; }
    public bool ShowRevCombox { get; set; }

    public bool SetRevisionSwitch { get; set; }
    public string? RemarkText { get; set; }

    // public string SelectedOrderNo { get; set; }
    public CustomerRevision SelectedRevision
    {
        get => selectedRevision;
        set
        {
            if(selectedRevision==value)
            {
                return;
            }

            selectedRevision=value;
            SaveRevision(value);
        }
    }

    public string SearchSelectedOrderNo { get; set; }



    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if(firstRender)
        {
            RemarkText="";
            EnableButton=false;
            ShowRevCombox=false;
            OrderNoData=new List<CustomerRevision>().AsEnumerable();
            OrderNoData=await TraceDataService.GetRevisionByShopOrder("");
            if(OrderNoData.Count()>0)
            {
                OrderNoDataList=OrderNoData.ToList();
                await UpdateUI();
                foreach(CustomerRevision revision in OrderNoDataList)
                {
                    await CheckOrRemoveCheck(revision);
                }

            }


            await UpdateUI();
        }
    }

    async void ResetInfo(bool backToStart)
    {
        if(backToStart)
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
        if(ShouldRender())
        {
            await Task.Delay(5);
            await InvokeAsync(StateHasChanged);
        }
#if DEBUG
        Console.WriteLine("UI is updated");
#endif
    }




    public async void SaveRevision(CustomerRevision revision)
    {
        if(await TraceDataService.UpdateRevision(revision))
        {
            Toast.ShowSuccess("Update Revision Success", "Success");
        }
        else
        {
            Toast.ShowError("Error Update Revision", "Error");
        };

    }

    public async Task LoadRevBySO(string orderNo)
    {

    }

    public async Task SearchText(string orderNo)
    {
        SearchSelectedOrderNo=orderNo;
    }

    public async Task SearchOrderNo(KeyboardEventArgs e)
    {
        if(e.Key=="Enter")
        {
            OrderNoData=new List<CustomerRevision>().AsEnumerable();
            OrderNoData=await TraceDataService.GetRevisionByShopOrder(SearchSelectedOrderNo);
            OrderNoDataList=new List<CustomerRevision>();
            await UpdateUI();
            if(OrderNoData.Count()>0)
            {
                OrderNoDataList=OrderNoData.ToList();

                foreach(CustomerRevision revision in OrderNoDataList)
                {
                    await CheckOrRemoveCheck(revision);
                }
            }
            //await jSRuntime.InvokeVoidAsync("ConsoleLog", "ShopOrderSearch: " + SearchSelectedOrderNo);
        }
    }

    public async Task CheckOrRemoveCheck(CustomerRevision revision)
    {
        string temp;
        switch(revision.Status)
        {
            case 0:
                temp=string.Concat(revision.OrderNo, "_", revision.Rev, "_", revision.LatestRev);
                await UpdateUI();
                await jSRuntime.InvokeVoidAsync("AddOrRemoveChecked", temp, 1);
                break;
            case 1:
                temp=string.Concat(revision.OrderNo, "_", revision.LatestRev);
                await UpdateUI();
                await jSRuntime.InvokeVoidAsync("AddOrRemoveChecked", temp, 1);
                break;
            case -1:
                temp=string.Concat(revision.OrderNo, "_", revision.Rev);
                await UpdateUI();
                await jSRuntime.InvokeVoidAsync("AddOrRemoveChecked", temp, 1);
                break;
        }
    }

    public async void SetRemark(string value, CustomerRevision revision)
    {

        selectedRevision=revision;
        RemarkText=value;
        if(value=="")
        {
            Toast.ShowWarning("Please fill the note", "Warning");
        }

    }

    public async Task UpdateRemark(KeyboardEventArgs e)
    {
        if(e.Key=="Enter")
        {
            if(selectedRevision!=null)
            {
                selectedRevision.Remark=RemarkText;
                // await jSRuntime.InvokeVoidAsync("ConsoleLog", SelectedRevision);
                await UpdateUI();
                if(await TraceDataService.UpdateRemarkDB(selectedRevision))
                {
                    Toast.ShowSuccess("Update Remark Success", "Success");
                }
                else
                {
                    Toast.ShowError("Error Remark Update", "Error");
                };
            }
            else
            {
                Toast.ShowWarning("Please fill the note", "Warning");
            }
        }
    }


}
