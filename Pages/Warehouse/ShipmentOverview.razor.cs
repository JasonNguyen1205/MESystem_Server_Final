using Blazored.Toast.Services;
using DevExpress.Blazor;
using DevExpress.Web;
using GLib;
using MESystem.Data;
using MESystem.Data.TRACE;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Globalization;
using System.IO;
using DateTime = System.DateTime;

namespace MESystem.Pages.Warehouse;

public partial class ShipmentOverview : ComponentBase
{
    [Inject]
    IJSRuntime? jSRuntime { get; set; }

    [Inject]
    TraceService? TraceDataService { get; set; }

    [Inject]
    UploadFileService? UploadFileService { get; set; }

    [Inject]
    IToastService? Toast { get; set; }
    public string? Title { get; set; }
    public bool Sound { get; set; } = true;

    public bool ShowPopUpFamily { get; set; } = true;
    public string SelectedFamily { get; set; } = "";

    public List<string>? Infofield { get; set; } = new();
    public List<string>? InfoCssColor { get; set; } = new();
    public List<string>? Result { get; set; } = new();
    public List<string>? HighlightMsg { get; set; } = new();
    public string CssUploadedList
    {
        get => cssUploadedList; set
        {
            cssUploadedList = value; Task.Run(async () =>
            {
                await UpdateUI();
            }
    );
        }
    }

    public string CssDataList { get => cssDataList; set => cssDataList = value; }

    public bool CollapseUploadedDetail
    {
        get => collapseUploadedDetail; set
        {
            collapseUploadedDetail = value; CssUploadedList = value ? "collapse" : ""; Task.Run(async () =>
            {
                await UpdateUI();
            }
    );
        }
    }

    public bool CollapseDataDetail
    {
        get => collapseDataDetail; set
        {
            collapseDataDetail = value; CssDataList = value ? "collapse" : ""; Task.Run(async () =>
            {
                await UpdateUI();
            }
    );
        }
    }
    public DateTime WeekValue { get; set; }

    public IEnumerable<Shipment> MasterList { get => masterList; set => masterList = value; }
    public class Family
    {
        public int id { get; set; }
        public int stock { get; set; }
        public string family { get; set; }

        public string partNo { get; set; }

        public Family(int id, int stock, string family, string partNo)
        {
            this.id = id;
            this.stock = stock;
            this.family = family;
            this.partNo = partNo;
        }
    }
    public IEnumerable<Shipment> Shipments { get; set; } = new List<Shipment>().AsEnumerable();
    public IEnumerable<Shipment> ShipmentsFromExcel { get; set; } = new List<Shipment>().AsEnumerable();
    public List<Shipment> ShipmentsFail { get; set; } = new List<Shipment>();
    public IEnumerable<Shipment> ShipmentsFailIEnum { get; set; } = new List<Shipment>();
    public List<Shipment> ShipmentsSuccess { get; set; } = new List<Shipment>();
    public IEnumerable<Shipment> ShipmentsSuccessIEnum { get; set; } = new List<Shipment>();

    public IEnumerable<Shipment> WarehouseInfos { get; set; } = new List<Shipment>();

    public string ShipmentType { get; set; }

    public string TemplateShipmentId { get; set; } = "";


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            WeekValue = DateTime.UtcNow;
            MasterList = await TraceDataService.GetLogisticData() ?? new List<Shipment>();
            CollapseUploadedDetail = true;
            CollapseDataDetail = true;
            ShipmentType = "SEA";
            TemplateShipmentId = string.Concat(
                WeekValue.Year,
                CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(WeekValue, CalendarWeekRule.FirstDay, DayOfWeek.Monday).ToString(),
                '-',
                @ShipmentType,
                "-01");
            Shipments = MasterList.Where(s => s.TracePalletBarcode == null);
            await UpdateUI();
        }
    }



    async Task ResetInfo(bool backToStart)
    {
        if (backToStart)
        {

            await UpdateUI();
        }
        else
        {
            InfoCssColor = new();
            Result = new();
            Infofield = new();
            HighlightMsg = new();
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

    //File upload
    private List<IBrowserFile> loadedFiles = new();
    private long maxFileSize = 1024 * 1000000;
    private int maxAllowedFiles = 1;
    private IEnumerable<Shipment> masterList;

    private bool isLoading { get; set; } = false;
    DxSchedulerDataStorage DataStorage = new DxSchedulerDataStorage()
    {
        //AppointmentsSource = AppointmentCollection.GetAppointments(),
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
            RecurrenceInfo = "Recurrence"
        }
    };
    private bool collapseUploadedDetail;
    private bool collapseDataDetail;
    private string cssUploadedList;
    private string cssDataList;
    private string[] ShipmentTypeLabel { get; set; } = { "AIR", "SEA" };

    private bool IsShipmentExist { get; set; } = false;

    private async Task LoadFiles(InputFileChangeEventArgs e)
    {
        isLoading = true;
        loadedFiles.Clear();
        await UpdateUI();

        foreach (var file in e.GetMultipleFiles(maxAllowedFiles))
        {
            try
            {

                loadedFiles.Add(file);
                if (!Directory.Exists(Path.Combine(Environment.ContentRootPath, "wwwroot", "uploads")))
                {
                    // Try to create the directory.
                    DirectoryInfo di = Directory.CreateDirectory(Path.Combine(Environment.ContentRootPath, "wwwroot", "uploads"));
                }
                var trustedFileNameForFileStorage = $"packinglist{DateTime.Now.ToString("dd-MM-yyyy-hh-mm-ss")}.xlsx";
                var path = Path.Combine(Environment.ContentRootPath, "wwwroot",
                       "uploads",
                        trustedFileNameForFileStorage);

                await using FileStream fs = new(path, FileMode.Create);
                await file.OpenReadStream(maxFileSize).CopyToAsync(fs);

                ShipmentsFromExcel = await UploadFileService.GetShipments(path);
                await UpdateUI();
                var index = 0;

                foreach (Shipment shipment in ShipmentsFromExcel)
                {
                    // Insert Into Table
                    if (string.IsNullOrEmpty(shipment.CustomerPo) || string.IsNullOrEmpty(shipment.PoNo))
                    {
                        ShipmentsFail.Add(shipment);
                    }
                    else
                    {
                        // int temp = await CheckShipmentExist(shipment)).Count();
                        // Check PO, Customer Part NO, yearWeek
                        if ((await CheckShipmentExist(shipment)).Count() == 0)
                        {
                            shipment.ShipmentId = TemplateShipmentId;
                            // Check PO, Customer Part NO, Type
                            if (await TraceDataService.UpdatePackingList(shipment))
                            {
                                ShipmentsSuccess.Add(shipment);
                            }
                            else
                            {
                                ShipmentsFail.Add(shipment);
                            }
                        }
                        else
                        {

                        }

                    }
                }
                ShipmentsFailIEnum = ShipmentsFail.AsEnumerable();
                ShipmentsSuccessIEnum = ShipmentsSuccess.AsEnumerable();

            }
            catch (Exception ex)
            {
                Toast.ShowError(ex.ToString(), "Error");
                // Logger.LogError("File: {Filename} Error: {Error}",file.Name, ex.Message);
            }
        }

        await UpdateUI();

        //Calculation
        if (await TraceDataService.ShipmentInfoCalculation())
        { //Get Infos after calculating
            MasterList = await TraceDataService.GetLogisticData() ?? new List<Shipment>();
            isLoading = false;
            await UpdateUI();
            Toast.ShowSuccess("Upload & Calculate successfully", "Success");
        }

        else Toast.ShowError("Error occured!");
    }

    private async Task<string> GetYearWeek(string shipmentId)
    {
        // Check ShipmentId Unique
        string yearWeek = "";
        if (shipmentId.Contains("Sea"))
        {
            yearWeek = shipmentId.Replace("Sea", "");
            yearWeek = shipmentId.Split("-")[0];
        }

        if (shipmentId.Contains("Air"))
        {
            yearWeek = shipmentId.Replace("Air", "");
            yearWeek = shipmentId.Split("-")[0];
        }
        return yearWeek;
    }

    private async Task ExportExcelWarehouse()
    {
        byte[] fileContent = await UploadFileService.ExportExcelWarehouse(MasterList.ToList());

        await jSRuntime.InvokeVoidAsync("saveAsFile", $"Warehouse_{DateTime.Now}.xlsx", Convert.ToBase64String(fileContent));
    }

    private async Task ExportExcelScm()
    {
        byte[] fileContent = await UploadFileService.ExportExcelSCM(MasterList.ToList());

        await jSRuntime.InvokeVoidAsync("saveAsFile", $"SCM_{DateTime.Now}.xlsx", Convert.ToBase64String(fileContent));
    }

    string[] headersWarehouse = {
        "PO NO",
        "PART NO",
        "CUSTOMER PO",
        "CUSTOMER PART NO",
        "PART DESCRIPTION",
        "SHIP QTY",
        "SHIP MODE",
        "SCANNED QTY",
        "CARTON QTY",
        "PALLET",
        "PALLET CAPACITY",
    };
    string[] headersScm = {
        "PO NO",
        "PART NO",
        "CUSTOMER PO",
        "CUSTOMER PART NO",
        "PART DESCRIPTION",
        "SHIP QTY",
        "SHIP MODE",
        "PALLET CAPACITY",
        "SCANNED QTY",
        "PALLET",
        "NET",
        "GROSS",
        "DIMENTION",
        "CBM" };
    public List<Shipment> WarehouseList = new List<Shipment>();
    public List<Shipment> ScmList = new List<Shipment>();
    public async Task PrintPdfWarehouse()
    {
        WarehouseList = MasterList.ToList();
        await UpdateUI();
        await jSRuntime.InvokeVoidAsync("printHtml", "printWarehouse");
    }
    public async Task PrintPdfScm()
    {
        ScmList = MasterList.ToList();
        await UpdateUI();
        await jSRuntime.InvokeVoidAsync("printHtml", "printScm");
    }

    public async Task<IEnumerable<Shipment>> CheckShipmentExist(Shipment shipment)
    {
        string shipmentYearWeek = TemplateShipmentId.Split("-")[0];

        if (Shipments.Any(_ => _.ShipmentId == null) && Shipments.Count() > 0) return null;
        try
        {
            var rs = Shipments.Where(
                                s =>
                                s.ShipmentId.Split("-")[0] == shipmentYearWeek
                                && s.PoNo == shipment.PoNo
                                && s.CustomerPartNo == shipment.CustomerPartNo);
            if (rs.Any()) return Shipments;
            else return new List<Shipment>();
        }
        catch (Exception)
        {
            return new List<Shipment>();
        }

    }

    public async Task<bool> CheckShipmentStatus(Shipment shipment)
    {
        string shipmentYearWeek = TemplateShipmentId.Split("-")[0];
        return Shipments.Where(
                                 s =>
                                 s.ShipmentId.Split("-")[0] == shipmentYearWeek
                                 && s.PoNo == shipment.PoNo
                                 && s.CustomerPartNo == shipment.CustomerPartNo).Count() > 0;
    }



}
