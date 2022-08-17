using Blazored.Toast.Services;
using DevExpress.Blazor;
using DevExpress.Spreadsheet;
using MESystem.Data.TRACE;
using MESystem.Service;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.IO;
using System.Reflection;

namespace MESystem.Pages.Planning;

public partial class Efficiency : ComponentBase
{
    private int tabIndex;
    private static DateTime fromDateSearch;

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
    //public static DateTime FromDateSearch { get; set; }
    public static DateTime FromDateSearch
    {
        get => fromDateSearch;
        set
        {
            fromDateSearch = value;
        }
    }
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
    public static IEnumerable<EffPlan> DataFromSearch { get; set; }

    //Scan for making palette only
    //public IEnumerable<Shipment> Shipments { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await OnDateChanged(DateTime.Now);
            await UpdateUI();
            //DataFromSearch = await TraceDataService.LoadDataSearchByDate(await ChangeTime(FromDateSearch, 00, 00, 00, 0));

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
    public IEnumerable<UploadFileInfo> Files;

    protected async Task SelectedFilesChanged(IEnumerable<UploadFileInfo> files)
    {
        Files = files;
        ShowPopUpCheckUploadData = true;
        FileName = files.Last().Name;

        var path = Path.Combine(Environment.ContentRootPath, "wwwroot",
               "uploads",
                $"{FileName}");

        if (File.Exists(path))
        {
            string NewFileName = FileName.Replace(".xlsx", "") + "_" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss");
            var tempPath  = Path.Combine(Environment.ContentRootPath, "wwwroot",
               "uploads",
                $"{NewFileName}.xlsx");
            System.IO.File.Move(path, tempPath);

            PlanFromExcel = new List<EffPlan>();
            IsBB = false;
            IsSMD = false;
            IsMI = false;

            if (FileName.ToUpper().Contains("SMD"))
            {
                IsSMD = true;
                PlanFromExcel = await UploadFileService.UploadFileToArraySMD(tempPath);
            }

            if (FileName.ToUpper().Contains("MI"))
            {
                IsMI = true;
                PlanFromExcel = await UploadFileService.UploadFileToArrayMI(tempPath);
            }

            if (FileName.ToUpper().Contains("BB"))
            {
                IsBB = true;
                PlanFromExcel = await UploadFileService.UploadFileToArrayBB(tempPath);
            }

        } 


        await UpdateUI();
    }

    public bool ShowPopUpCheckUploadData { get; set; } = false;
    public async void PopupClosingCheckUploadData()
    {
        ShowPopUpCheckUploadData = false;
        await SelectedFilesChanged(Files);
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
                if (PlanFromExcel.Count() > 0)
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

    public async Task OnDateChanged(DateTime newValue)
    {
        FromDateSearch = await ChangeTime(newValue, 00, 00, 00, 0);
        await OnStartDateChanged(FromDateSearch);
        await Task.Delay(100);
        await jSRuntime.InvokeVoidAsync("showLastPanel");
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


    DxSchedulerDataStorage DataStorageSMD = new();
    DxSchedulerDataStorage DataStorageMI = new();
    DxSchedulerDataStorage DataStorageBB = new();
    public IEnumerable<EffPlan> DataFromSearchSMD { get; set; }
    public IEnumerable<EffPlan> DataFromSearchMI { get; set; }
    public IEnumerable<EffPlan> DataFromSearchBB { get; set; }
    public async Task OnStartDateChanged(DateTime startDate)
    {
        FromDateSearch = await ChangeTime(startDate, 00, 00, 00, 0);
        DataFromSearch = await TraceDataService.LoadDataSearchByDate(FromDateSearch);
        await UpdateUI();
        IsSMD = false;
        IsBB = false;
        IsMI = false;

        if (DataFromSearch.Where(e => e.Area.Equals("SMD")).Any())
        {
            IsSMD = true;
            DataFromSearchSMD = DataFromSearch.Where(e => e.Area.Equals("SMD"));
            DataStorageSMD = new DxSchedulerDataStorage()
            {
                AppointmentsSource = ResourceAppointmentCollection.GetAppointments(FromDateSearch, DataFromSearchSMD),
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
                ResourcesSource = ResourceAppointmentCollection.GetResourcesForGrouping(DataFromSearchSMD),
                ResourceMappings = new DxSchedulerResourceMappings()
                {
                    Id = "Id",
                    Caption = "Name",
                    BackgroundCssClass = "BackgroundCss",
                    TextCssClass = "TextCss"
                }
            };
        }
        if (DataFromSearch.Where(e => e.Area.Equals("MI")).Any())
        {
            IsMI = true;
            DataFromSearchMI = DataFromSearch.Where(e => e.Area.Equals("MI"));
            DataStorageMI = new DxSchedulerDataStorage()
            {
                AppointmentsSource = ResourceAppointmentCollection.GetAppointments(FromDateSearch, DataFromSearchMI),
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
                ResourcesSource = ResourceAppointmentCollection.GetResourcesForGrouping(DataFromSearchMI),
                ResourceMappings = new DxSchedulerResourceMappings()
                {
                    Id = "Id",
                    Caption = "Name",
                    BackgroundCssClass = "BackgroundCss",
                    TextCssClass = "TextCss"

                }
            };
        }

        if (DataFromSearch.Where(e => e.Area.Equals("BB")).Any())
        {
            IsBB = true;
            DataFromSearchBB = DataFromSearch.Where(e => e.Area.Equals("BB"));
            DataStorageBB = new DxSchedulerDataStorage()
            {
                AppointmentsSource = ResourceAppointmentCollection.GetAppointments(FromDateSearch, DataFromSearchBB),
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
                ResourcesSource = ResourceAppointmentCollection.GetResourcesForGrouping(DataFromSearchBB),
                ResourceMappings = new DxSchedulerResourceMappings()
                {
                    Id = "Id",
                    Caption = "Name",
                    BackgroundCssClass = "BackgroundCss",
                    TextCssClass = "TextCss"
                }
            };
        }

    }

    async Task OnClickAppointment()
    {
        string title = "Shop Order Detail";
        await jSRuntime.InvokeVoidAsync("changeTitleAppointment", title);
        await OnClickAppointment();
    }
    public async void ShowChart()
    {
        TabIndex = 1;
        await Task.Delay(100);
        await jSRuntime.InvokeVoidAsync("showLastPanel");
        await UpdateUI();
    }

    protected string GetValidationMessage(EditContext editContext, string fieldName)
    {
        FieldIdentifier field = editContext.Field(fieldName);
        return string.Join("\n", editContext.GetValidationMessages(field));
    }

    void Grid_CustomizeEditModel(GridCustomizeEditModelEventArgs e)
    {
        if (e.IsNew)
        {
            EffPlan? newEffPlan = (EffPlan)e.EditModel;
            
        }
    }
    async Task Grid_EditModelSavingEff(GridEditModelSavingEventArgs e)
    {
        EffPlan? effPlan = (EffPlan)e.EditModel;
        if(effPlan.Idx == null && !string.IsNullOrEmpty(effPlan.Area))
        {
            
            if (await TraceDataService.InsertEff(effPlan))
            {
                Toast.ShowSuccess("Insert success", "SUCCESS");
            }
            else
            {
                Toast.ShowError("Insert fail", "FAIL");
            }
        } else if(effPlan.Idx != null)
        {
            if (await TraceDataService.UpdateEff(effPlan))
            {
                Toast.ShowSuccess("Update success", "SUCCESS");
            }
            else
            {
                Toast.ShowError("Update fail", "FAIL");
            }
        } else
        {
            Toast.ShowError("Update fail. Please fill info of area, plan date, from time, to time, so BB", "FAIL");
            return;
        }

        await OnStartDateChanged(FromDateSearch);
        await Task.Delay(100);
        await jSRuntime.InvokeVoidAsync("showLastPanel");
        await UpdateUI();
    }
    async Task Grid_DataItemDeleting(GridDataItemDeletingEventArgs e)
    {
        if(await TraceDataService.RemoveEff((EffPlan)e.DataItem))
        {
            await OnStartDateChanged(FromDateSearch);
            await Task.Delay(100);
            await jSRuntime.InvokeVoidAsync("showLastPanel");
            await UpdateUI();
            Toast.ShowSuccess("Delete success", "SUCCESS");
        } else
        {
            Toast.ShowError("Delete fail", "FAIL");
        }
       
    }

}
