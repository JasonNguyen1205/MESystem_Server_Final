using Blazored.Toast.Services;

using DevExpress.Blazor;

using MESystem.Data.TRACE;
using MESystem.Service;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

using ComponentBase = Microsoft.AspNetCore.Components.ComponentBase;

namespace MESystem.Pages.Warehouse;

public partial class CheckStockByFamily : ComponentBase
{
    [Inject]
    IJSRuntime? jSRuntime { get; set; }

    [Inject]
    TraceService? TraceDataService { get; set; }

    [Inject]
    IToastService? Toast { get; set; }

    [Inject]
    UploadFileService UploadFileService { get; set; }

    public IGrid? Grid { get; set; }

    public List<string>? Infofield { get; set; } = new();
    public List<string>? InfoCssColor { get; set; } = new();
    public List<string>? Result { get; set; } = new();
    public List<string>? HighlightMsg { get; set; } = new();

    public string? Title { get; set; }
    public bool Sound { get; set; } = true;

    public bool ShowPopUpFamily { get; set; } = true;
    public string SelectedFamily { get; set; } = "";

    public class Family
    {
        public int id { get; set; }
        public int stock { get; set; }
        public string family { get; set; }

        public string partNo { get; set; }

        public Family(int id, int stock, string family, string partNo)
        {
            this.id=id;
            this.stock=stock;
            this.family=family;
            this.partNo=partNo;
        }
    }

    public List<Family> FamilyList { get; set; } = new List<Family>();
    public IEnumerable<StockByFamily> ListStockByFamily { get; set; } = new List<StockByFamily>().AsEnumerable();


    IGridDataColumn? PD { get; set; }
    //Scan for making palette only
    // public int QtyPerBox;

    //string? Scanfield { get; set; }
    //string? Scanfield2 { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if(firstRender)
        {
            FamilyList.Add(new Family(1, 10, "Phoenix", "1893678"));

            //  PartNoByFamily.Add(new Family(1, 100, "Phoenix", "1897478"));
            // PartNoByFamily.Add(new Family(2, 200, "Phoenix", "1893678"));
            await UpdateUI();
            //FamilyList.AddRange();
        }
    }

    async void UpdateInfoField(string cssTextColor, string? result = null, string? content = null, string? highlightMsg = null, bool reset = false)
    {
        if(reset)
        {
            InfoCssColor=new();
            Result=new();
            Infofield=new();
            HighlightMsg=new();
        }

        if(result=="ERROR")
        {
            //await ResetInfo(false);
            if(Sound)
            {
                await jSRuntime.InvokeVoidAsync("playSound", "/sounds/alert.wav");
            }

        }

        if(string.IsNullOrEmpty(cssTextColor)||string.IsNullOrEmpty(content))
        {
            return;
        }

        InfoCssColor.Add(cssTextColor);

        if(result!=null)
        {
            Result.Add(result);
        }
        else
        {
            Result.Add("INFO");
        }

        Infofield.Add(content);

        if(highlightMsg!=null)
        {
            HighlightMsg.Add(highlightMsg);
        }
        else
        {
            HighlightMsg.Add(string.Empty);
        }

        await UpdateUI();
    }

    async Task ResetInfo(bool backToStart)
    {
        if(backToStart)
        {

            await UpdateUI();
        }
        else
        {
            Infofield=new();
            InfoCssColor=new();
            Result=new();
            HighlightMsg=new();
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

    //----------------------------------------------------------------
    public async void GetFamily(Family family)
    {
        SelectedFamily=family.family;
        await jSRuntime.InvokeVoidAsync("ConsoleLog", family);
    }

    public async void PopupClosingStock(PopupClosingEventArgs args)
    {
        ShowPopUpFamily=false;
        await UpdateUI();
        // await jSRuntime.InvokeVoidAsync("ConsoleLog", "Closing");
    }

    public async Task LoadStock()
    {
        ListStockByFamily=await TraceDataService.GetStockByFamily(SelectedFamily);
        if(PD!=null)
        {
            PD.DisplayFormat=string.Format(@"MMM\/yyyy");
        }
        ShowPopUpFamily=false;
        await UpdateUI();
    }

    private async Task ExportExcel()
    {
        var fileContent = await UploadFileService.ExportExcelStock(ListStockByFamily.ToList());

        await jSRuntime.InvokeVoidAsync("saveAsFile", $"Stock_{DateTime.Now}.xlsx", Convert.ToBase64String(fileContent));
    }

    void ExpandAllRows_Click()
    {
        Grid.ExpandAllGroupRows();
    }
    void CollapseAllRows_Click()
    {
        Grid.CollapseAllGroupRows();
    }
}
