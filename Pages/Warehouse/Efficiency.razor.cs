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
    private int tabIndex;
    private static DateTime fromDateSearch;

    [Inject]
    IJSRuntime? jSRuntime { get; set; }

    [Inject]
    static TraceService? TraceDataService { get; set; }

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
    public static DateTime FromDateSearch { get; set; } = DateTime.Now;
    //public static DateTime FromDateSearch
    //{
    //    get => fromDateSearch;
    //    set
    //    {
    //        fromDateSearch = value;
    //        //Task.Run(async () =>
    //        //{
    //        //    ToDateSearch = await ChangeTime(fromDateSearch.AddDays(1), 06, 00, 00, 0);
    //        //    await UpdateUI();
    //        //});

    //    }
    //}
    public static DateTime ToDateSearch { get; set; }
    public int TabIndex
    {
        get => tabIndex;
        set
        {
            tabIndex = value;
            if (tabIndex == 1)
            {
                FromDateSearch = DateTime.Now;
                ToDateSearch = DateTime.Now;
                //LoadDataSearch(FromDateSearch, ToDateSearch);

            }
        }
    }
    public List<EffPlan> PlanFromExcel { get; set; } = new();
    public IEnumerable<EffPlan> DataFromSearch { get; set; }

    //Scan for making palette only
    //public IEnumerable<Shipment> Shipments { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            FromDateSearch = DateTime.Now;
            
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
        if (reset)
        {
            InfoCssColor = new();
            Result = new();
            Infofield = new();
            HighlightMsg = new();
        }

        if (result == "ERROR")
        {
            //await ResetInfo(false);
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

    async Task ResetInfo(bool backToStart)
    {
        if (backToStart)
        {
            IsBB = false;
            IsSMD = false;
            IsMI = false;
            // Scanfield = "";
            // await jSRuntime.InvokeVoidAsync("focusEditorByID", "Barcode");
        }
        else
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
        if (ShouldRender())
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
        FileName = files.Last().Name;

        var path = Path.Combine(Environment.ContentRootPath, "wwwroot",
               "uploads",
                $"{FileName}");

        //await using FileStream fs = (path, FileMode.Open);
        //fs.Close();
        PlanFromExcel = new List<EffPlan>();
        IsBB = false;
        IsSMD = false;
        IsMI = false;

        if (FileName.ToUpper().Contains("SMD"))
        {
            IsSMD = true;
            PlanFromExcel = await UploadFileService.UploadFileToArraySMD(path);
        }

        if (FileName.ToUpper().Contains("MI"))
        {
            IsMI = true;
            PlanFromExcel = await UploadFileService.UploadFileToArrayMI(path);
        }

        if (FileName.ToUpper().Contains("BB"))
        {
            IsBB = true;
            PlanFromExcel = await UploadFileService.UploadFileToArrayBB(path);
        }

        await UpdateUI();
    }

    public bool ShowPopUpCheckUploadData { get; set; } = false;
    public async void PopupClosingCheckUploadData()
    {
        ShowPopUpCheckUploadData = false;
        await UpdateUI();
    }

    public bool ShowPopUpFileSave { get; set; } = false;

    public async void PopupClosingFileSave()
    {
        ShowPopUpFileSave = false;
        await UpdateUI();
    }

    private async Task FileSave()
    {
        ShowPopUpFileSave = true;
        await UpdateUI();
    }


    public async Task PopUpFileSave()
    {
        try
        {
            if (IsSMD || IsMI || IsBB)
            {
                if (PlanFromExcel.Count > 0)
                {
                    foreach (EffPlan eff in PlanFromExcel)
                    {
                        if (IsSMD)
                        {
                            eff.Area = "SMD";
                        }

                        if (IsMI)
                        {
                            eff.Area = "MI";
                        }
                        if (IsBB)
                        {
                            eff.Area = "BB";
                        }

                        if (!await TraceDataService.InsertEff(eff))
                        {
                            Toast.ShowError("File Error Input", "ERROR");
                            return;
                        }
                    }

                }

            }
            Toast.ShowSuccess("Upload Success", "SUCCESS");
            ShowPopUpFileSave = false;
            await UpdateUI();
        }
        catch (Exception)
        {
            Toast.ShowError("Data Upload Error Error", "ERROR");
            ShowPopUpFileSave = false;
        }
    }

    public async Task LoadDataSearch(DateTime fromDate, DateTime toDate)
    {
        //DataFromSearch = await TraceDataService.LoadDataSearchByDate(fromDate);
        await UpdateUI();
    }
    public List<string> MiLines { get; set; } = new();
    public async Task Search()
    {
        FromDateSearch = await ChangeTime(FromDateSearch, 00, 00, 00, 0);

        DataFromSearch = await TraceDataService.LoadDataSearchByDate(FromDateSearch);
        foreach (EffPlan e in DataFromSearch)
        {
            if (!MiLines.Contains(e.RealLine))
            {
                MiLines.Add(e.RealLine);
            }
        }
        await UpdateUI();
    }

    public async Task<DateTime> ChangeTime(DateTime dateTime, int hours, int minutes, int seconds, int milliseconds)
    {
        return new DateTime(
            dateTime.Year,
            dateTime.Month,
            dateTime.Day,
            hours,
            minutes,
            seconds,
            milliseconds,
            dateTime.Kind);
    }

    DxSchedulerDataStorage DataStorage = new DxSchedulerDataStorage()
    {
        AppointmentsSource = ResourceAppointmentCollection.GetAppointments(),
        AppointmentMappings = new DxSchedulerAppointmentMappings()
        {
            Type = "AppointmentType",
            Start = "StartDate",
            End = "EndDate",
            Subject = "Caption",
            AllDay = "AllDay",
            Location = "Location",
            Description = "Description",
            LabelId = "Label",
            StatusId = "Status",
            RecurrenceInfo = "Recurrence",
            ResourceId = "ResourceId"
        },
        ResourcesSource = ResourceAppointmentCollection.GetResourcesForGrouping(),
        ResourceMappings = new DxSchedulerResourceMappings()
        {
            Id = "Id",
            Caption = "Name",
            BackgroundCssClass = "BackgroundCss",
            TextCssClass = "TextCss"
        }
    };
}
