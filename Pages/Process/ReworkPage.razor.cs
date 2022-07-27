
using MESystem.Service;
using MESystem.Data.TRACE;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace MESystem.Pages.Process;

public partial class ReworkPage : ComponentBase
{
    [Inject]
    private TraceService? TraceDataService { get; set; }
    public string? FocusElement { get; set; }

    public string? ReadOnlyElement { get; set; }
    bool ShowCheckboxes { get; set; } = true;
    public IEnumerable<Rework>? Data { get; set; }
    string? ng_Code { get; set; }
    string? last_Code { get; set; }
    string? first_Code { get; set; }
    string? remark { get; set; }
    public bool IsReady { get; set; }
    public string? ng_data { get; set; }
    public string? barcode { get; set; }
    int flag { get; set; }
    protected override async Task OnInitializedAsync()
    {
        
        Data = await TraceDataService.GetNGCode();

        flag = 0;
    }
    async void GetData()
    {

    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {

        }

    }
    private async void HandleCodeInput(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            if (flag == 0)
            {
                FocusElement = "ng_Code2";
                ReadOnlyElement = "ng_Code1";
                flag = 1;
            }
            else
            {
                FocusElement = "barcode";
                ReadOnlyElement = "ng_Code2";
                flag = 0;
                ng_Code = first_Code + last_Code;
                ng_data = Data.Where(e => e.NG_Description_VN.Split(".")[0].ToString() == ng_Code).FirstOrDefault()?.NG_Description_VN ?? "Wrong Rework Code!";
            }
        }

    }

    private async void HandleBarcodeInput(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            TraceDataService.GetBarcodeLink(barcode);
            Rework input_data = new Rework(barcode,null,int.Parse(ng_Code),"","","","");
            TraceDataService.InsertReworkData(input_data);
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
