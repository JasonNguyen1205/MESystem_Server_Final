using Blazored.Toast.Services;
using DevExpress.Blazor;
using MESystem.Data.TRACE;
using MESystem.Service;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.IO;

namespace MESystem.Pages.Warehouse;

public partial class Efficiency : ComponentBase
{
    [Inject]
    IJSRuntime? jSRuntime { get; set; }

    [Inject]
    TraceService? TraceDataService { get; set; }

    [Inject]
    IToastService? Toast { get; set; }

    [Inject]
    UploadFileService UploadFileService { get; set; }

    public List<string>? Infofield { get; set; } = new();
    public List<string>? InfoCssColor { get; set; } = new();
    public List<string>? Result { get; set; } = new();
    public List<string>? HighlightMsg { get; set; } = new();

    bool UploadVisible { get; set; } = true;

    public IGrid? Grid { get; set; }
    public string? Title { get; set; }
    public bool Sound { get; set; } = true;

    public List<SMDPlan> SmdPlanFromExcel { get; set; } = new();
    public List<MIPLan> MiPlanFromExcel { get; set; } = new();
    public List<BBPlan> BbPlanFromExcel { get; set; } = new();

    //Scan for making palette only
    //public IEnumerable<Shipment> Shipments { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if(firstRender)
        {
            //Shipments = await TraceDataService.GetLogisticData(shipmentId: "ALL") ?? new List<Shipment>();
            //await jSRuntime.InvokeVoidAsync("focusEditorByID", "ComboBoxShipmentId");
        }
    }

    protected string GetUploadUrl(string url)
    {
        return NavigationManager.ToAbsoluteUri(url).AbsoluteUri;
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
            IsBB = false;
            IsSMD = false;
            IsMI = false;
           // Scanfield = "";
           // await jSRuntime.InvokeVoidAsync("focusEditorByID", "Barcode");
        }else
        {
            await Task.Run(() =>
            {
            
                Infofield = new();
                InfoCssColor = new();
                Result = new();
                HighlightMsg = new();
                //IsReadOnly = false;
            });
        }
           await UpdateUI();

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

    public string FileName { get; set; } = "";
    public bool IsSMD { get; set; } = false;
    public bool IsMI { get; set; } = false;
    public bool IsBB { get; set; } = false;



    protected async void SelectedFilesChanged(IEnumerable<UploadFileInfo> files)
   {
        //UploadVisible = files.ToList().Count > 0;
        ShowPopUpCheckUploadData = true;
        FileName = files.FirstOrDefault().Name;

        if (!Directory.Exists(Path.Combine(Environment.ContentRootPath, "wwwroot", "uploads")))
        {
            // Try to create the directory.
            _ = Directory.CreateDirectory(Path.Combine(Environment.ContentRootPath, "wwwroot", "uploads"));
        }

        var path = Path.Combine(Environment.ContentRootPath, "wwwroot",
               "uploads",
                $"{FileName}");

        await using FileStream fs = new(path, FileMode.Create);
        fs.Close();

        if (FileName.ToUpper().Contains("SMD"))
        {
            IsSMD = true;
            SmdPlanFromExcel = await UploadFileService.UploadFileToArraySMD(path);
            await UpdateUI();
            if(SmdPlanFromExcel.Count() > 0)
            {
                
            }
        }

        if (FileName.ToUpper().Contains("MI"))
        {
            IsMI = true;
        }

        if (FileName.ToUpper().Contains("BB"))
        {
            IsBB = true;
        }

        await UpdateUI();
   }

    public bool ShowPopUpCheckUploadData { get; set; } = false;
    public async void PopupClosingCheckUploadData()
    {
        ShowPopUpCheckUploadData = false;
        await UpdateUI();
    }

    public async void UploadDataFunc()
    {
        try
        {
            
            //foreach (Shipment s in Shipments)
            //{
            //    _ = await TraceDataService.UpdateRawDataByIdx(s.Idx, -2);
            //}
            //MasterList = await TraceDataService.GetLogisticData("ALL") ?? new List<Shipment>();
            //Shipments = await TraceDataService.GetLogisticData(SelectedShipmentId) ?? new List<Shipment>();
            //ShowPopUpFinishShipment = false;

            //Toast.ShowSuccess("Finished Shipment Success", "SUCCESS");
            //FinishEnable = false;

            // Run 2 function update packing list full and packing patial before finish.
            //await UpdatePackingList();

            //await EmailService.SendingEmailFinishShipment(SelectedShipmentId, "Warehouse");
            //await UpdateUI();

        }
        catch (Exception)
        {
            Toast.ShowError("Data Upload Error Error", "Error");
        //    FinishEnable = true;
        }

    }



}
