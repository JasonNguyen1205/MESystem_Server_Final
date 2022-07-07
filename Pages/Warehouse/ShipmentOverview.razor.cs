using Blazored.Toast.Services;
using DevExpress.Blazor;
using MESystem.Data;
using MESystem.Data.TRACE;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Globalization;
using System.IO;
using System.Linq;
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
    UploadFileInfo BrowserFile { get; set; }
    public string? Title { get; set; }
    public bool Sound { get; set; } = true;

    public bool ShowPopUpFamily { get; set; } = true;
    public string SelectedFamily { get; set; } = "";
    public string? SelectedWeek { get => selectedWeek; set => selectedWeek=value; }
    public string? SelectedYear { get => selectedYear; set => selectedYear=value; }
    public List<string>? Infofield { get; set; } = new();
    public List<string>? InfoCssColor { get; set; } = new();
    public List<string>? Result { get; set; } = new();
    public List<string>? HighlightMsg { get; set; } = new();
    public string CssUploadedList
    {
        get => cssUploadedList; set
        {
            cssUploadedList=value; Task.Run(async () =>
            {
                await UpdateUI();
            });
        }
    }
    public string CssDataList { get => cssDataList; set => cssDataList=value; }

    public IGrid? Grid { get; set; }

    public bool CollapseUploadedDetail
    {
        get => collapseUploadedDetail; set
        {
            collapseUploadedDetail=value; CssUploadedList=value ? "collapse" : ""; Task.Run(async () =>
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
            collapseDataDetail=value; CssDataList=value ? "collapse" : ""; Task.Run(async () =>
            {
                await UpdateUI();
            }
    );
        }
    }
    public DateTime WeekValue
    {
        get => weekValue;
        set
        {
            weekValue=value;
            Task.Run(async () => await WeekChanged(value));
        }
    }
    public IEnumerable<Shipment> MasterList { get => masterList; set => masterList=value; }
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
    public IEnumerable<Shipment> Shipments { get; set; } = new List<Shipment>().AsEnumerable();
    public IEnumerable<Shipment> ShipmentsFromExcel { get; set; } = new List<Shipment>().AsEnumerable();
    public List<Shipment> ShipmentsFail { get; set; } = new List<Shipment>();
    public IEnumerable<Shipment> ShipmentsFailIEnum { get; set; } = new List<Shipment>();
    public List<Shipment> ShipmentsSuccess { get; set; } = new List<Shipment>();
    public IEnumerable<Shipment> ShipmentsSuccessIEnum { get; set; } = new List<Shipment>();

    public IEnumerable<Shipment> WarehouseInfos { get; set; } = new List<Shipment>();

    public string ShipmentType { get; set; }
    bool InvoiceStatus { get; set; }
    string? InvoiceNumber { get; set; }
    public bool IsOpen { get; set; }
    public async void GetInputfield(string content) { InvoiceNumber=content; await UpdateUI(); }
    public async void SetContainerNofield(string content) { SelectedContainerNo=content; await UpdateUI(); }
    public int ShipmentIdx { get; set; }
    public string SelectedShipmentId
    {
        get => templateShipmentId; set
        {
            templateShipmentId = value;
        }
    }
   
    public List<string> ShipmentIdList { get; set; } = new List<string>();

    bool UploadVisible { get; set; } = false;
   // public ValueTask<FileUploadEventArgs> SelectedFilesChanged;

    public bool ShowPopUpFinishShipment { get; set; } = false;

    public bool PlanningUpdateExistShipment { get; set; } = false;

    public IEnumerable<Shipment> OldShipmentToUpdate { get; set; } 

    protected async Task SelectedFiles(string FileName)
    {
        
        await WeekChanged(WeekValue); 
        await UpdateUI();
        isLoading = true;
        await UpdateUI();

        if (!Directory.Exists(Path.Combine(Environment.ContentRootPath, "wwwroot", "uploads")))
        {
            // Try to create the directory.
            DirectoryInfo di = Directory.CreateDirectory(Path.Combine(Environment.ContentRootPath, "wwwroot", "uploads"));
        }
        //var trustedFileNameForFileStorage = $"packinglist{DateTime.Now.ToString("dd-MM-yyyy-hh-mm-ss")}.xlsx";
        //var trustedFileNameForFileStorage = file.Name;
        var path = Path.Combine(Environment.ContentRootPath, "wwwroot",
               "uploads",
                $"{FileName}");
        try
        {

            await using FileStream fs = new(path, FileMode.Open);
            fs.Close();
            //e.FileInfo

            ShipmentsFromExcel=await UploadFileService.GetShipments(path);
            await UpdateUI();
            await InsertShipment();
           
        }
        catch(Exception ex)
        {
            Toast.ShowError(ex.ToString(), "Error");
            // Logger.LogError("File: {Filename} Error: {Error}",file.Name, ex.Message);
        }
        ShipmentsFailIEnum=ShipmentsFail.AsEnumerable();
        ShipmentsSuccessIEnum=ShipmentsSuccess.AsEnumerable();

        //}
        await UpdateUI();

        //Calculation
        if(ShipmentsSuccess.Count()>0)
        { //Get Infos after calculating
            MasterList=await TraceDataService.GetLogisticData("ALL")??new List<Shipment>();
            await TraceDataService.ShipmentInfoCalculation(SelectedShipmentId);
            //await TraceDataService.ShipmentInfoUpdate(SelectedShipmentId);
            isLoading=false;
            Toast.ShowSuccess("Upload & Calculate successfully", "Success");
            // Send Email
            await EmailService.SendingEmail(path, SelectedShipmentId);

        }

        if(ShipmentsFail.Count()>0) Toast.ShowError("Error occured!");

        ShipmentsFromExcel=new List<Shipment>();
        ShipmentsFail=new List<Shipment>();
        ShipmentsSuccess=new List<Shipment>();
        CollapseUploadedDetail=false;
        await WeekChanged(DateTime.Now);
        await UpdateUI();
    }

  public async Task InsertShipment()
    {
        foreach (Shipment shipment in ShipmentsFromExcel)
        {
            // Insert Into Table
            if (string.IsNullOrEmpty(shipment.CustomerPo) || string.IsNullOrEmpty(shipment.PoNo))
            {
                ShipmentsFail.Add(shipment);
            }
            else
            {
                shipment.ShipmentId = SelectedShipmentId;
                shipment.Week_ = SelectedWeek;
                shipment.Year_ = SelectedYear;

                var i = SelectedShipmentId.Contains("AIR") && !shipment.ShipMode.ToUpper().Contains("SEA") && !shipment.ShipMode.ToUpper().Contains("DHL");
                var j = SelectedShipmentId.Contains("SEA") && !shipment.ShipMode.ToUpper().Contains("AIR") && !shipment.ShipMode.ToUpper().Contains("DHL");
                var z = SelectedShipmentId.Contains("DHL") && !shipment.ShipMode.ToUpper().Contains("AIR") && !shipment.ShipMode.ToUpper().Contains("SEA");
                if (!string.IsNullOrEmpty(shipment.ShipMode) && (i || j || z))
                {
                    if (!await TraceDataService.InsertPackingList(shipment)) return;
                    ShipmentsSuccess.Add(shipment);
                }
                else
                {
                    ShipmentsFail.Add(shipment);
                }
            }
        }
    }

    protected string GetUploadUrl(string url)
    {
        return NavigationManager.ToAbsoluteUri(url).AbsoluteUri;
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if(firstRender)
        {
            WeekValue=DateTime.Now;
            CollapseUploadedDetail=false;
            CollapseDataDetail=false;
            ShipmentType="SEA";
            ShipmentIdx=1;
            SelectedShipmentId=string.Concat(
                SelectedYear,
                SelectedWeek,
                '-',
                ShipmentType,
                $"-{ShipmentIdx}");
            MasterList=await TraceDataService.GetLogisticData("ALL")??new List<Shipment>();
            Shipments=await TraceDataService.GetLogisticData(SelectedShipmentId)??new List<Shipment>();
            foreach(Shipment s in MasterList.Where(s => s.ShipmentId!=null).ToList())
            {
                if(!ShipmentIdList.Contains(s.ShipmentId)) ShipmentIdList.Add(s.ShipmentId);
            }
            if(Shipments.Count() > 0)
            ShippingDate = Shipments.FirstOrDefault().ShippingDate;


            PORevised = false;
            await UpdateUI();
        }
    }
    protected override async Task OnInitializedAsync()
    {
        //Data = await NwindDataService.GetEmployeesEditableAsync();

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
    private IEnumerable<Shipment> masterList;
    private bool isLoading { get; set; } = false;
    private bool collapseUploadedDetail;
    private bool collapseDataDetail;
    private string cssUploadedList;
    private string cssDataList;
    private string[] ShipmentTypeLabel { get; set; } = { "AIR", "SEA", "DHL" };
    private async Task ExportExcelWarehouse()
    {
        isLoading=true;
        await UpdateUI();

        byte[] fileContent = await UploadFileService.ExportExcelWarehouse(Shipments.ToList());
        await jSRuntime.InvokeVoidAsync("saveAsFile", $"Warehouse_{Shipments.First().ShipmentId}.xlsx", Convert.ToBase64String(fileContent));

        isLoading=false;
        await UpdateUI();
    }
    private async Task ExportExcelScm()
    {
        byte[] fileContent = await UploadFileService.ExportExcelSCM(MasterList.ToList());

        await jSRuntime.InvokeVoidAsync("saveAsFile", $"SCM_{DateTime.Now}.xlsx", Convert.ToBase64String(fileContent));
    }
    private async Task ExportTempShipmentData()
    {
        isLoading = true;
        await UpdateUI();

        if(await UploadFileService.ExportTempShipmentData(Shipments.ToList())) {

            Toast.ShowSuccess("Watermark Success", "SUCCESS");
        } else
        {
            Toast.ShowError("Watermark Fail", "FAIL");
        }

        isLoading = false;

        await UpdateUI();
        //await jSRuntime.InvokeVoidAsync("saveAsFile", $"SCM_{DateTime.Now}.xlsx", Convert.ToBase64String(fileContent));
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
    private DateTime weekValue;
    private string templateShipmentId;
    private string? selectedWeek;
    private string? selectedYear;

    public bool Checked { get; set; }
    void WindowShowing(FlyoutShowingEventArgs args)
    {
        //args.Cancel = !Checked;
    }
    void WindowShown(FlyoutShownEventArgs args)
    {
        Checked=false;
    }

    public int TabIndex { get; set; }
    public int TabChildrenIndex { get; set; }

    public DateTime? ShippingDate { get; set; } = DateTime.Now;
    public async Task PrintPdfWarehouse()
    {
        WarehouseList = Shipments.ToList();
        await UpdateUI();
        await jSRuntime.InvokeVoidAsync("printHtml", "printWarehouse");
    }
    public async Task PrintPdfScm()
    {
        ScmList = Shipments.ToList();
        await UpdateUI();
        await jSRuntime.InvokeVoidAsync("printHtml", "printScm");
    }

    public async Task<IEnumerable<Shipment>> CheckShipmentExist(Shipment shipment)
    {
        string shipmentYearWeek = SelectedShipmentId.Split("-")[0];

        if(Shipments.Any(_ => _.ShipmentId==null)&&Shipments.Count()>0) return null;
        try
        {
            var rs = Shipments.Where(
                                s =>
                                s.ShipmentId.Split("-")[0]==shipmentYearWeek
                                &&s.PoNo==shipment.PoNo
                                &&s.CustomerPartNo==shipment.CustomerPartNo);
            if(rs.Any()) return Shipments;
            else return new List<Shipment>();
        }
        catch(Exception)
        {
            return new List<Shipment>();
        }

    }

    public async Task<bool> CheckShipmentStatus(Shipment shipment)
    {
        string shipmentYearWeek = SelectedShipmentId.Split("-")[0];
        return MasterList.Where(
                                 s =>
                                 s.ShipmentId.Split("-")[0]==shipmentYearWeek
                                 &&s.PoNo==shipment.PoNo
                                 &&s.CustomerPartNo==shipment.CustomerPartNo).Count()>0;
    }

    public async void UpdateShippingDate()
    {
        if (await TraceDataService.UpdateShippingDateToShipment(SelectedShipmentId, ShippingDate))
        {
            Toast.ShowSuccess("Update Shipping Date Success", "Success");
        } else
        {
            Toast.ShowError("Update Shipping Date Fail", "Fail");
        }
        MasterList = await TraceDataService.GetLogisticData("ALL") ?? new List<Shipment>();
        Shipments = await TraceDataService.GetLogisticData(SelectedShipmentId) ?? new List<Shipment>();

        await UpdateUI();
    }

    public async Task<bool> WeekChanged(DateTime dt = default, string selectedShipmentId = null)
    {
        await Task.Run(() =>
        {
            SelectedYear=dt.Year.ToString();
            SelectedWeek=GetIso8601WeekOfYear(dt).ToString();
        });

        ShipmentIdx=1;
        if(string.IsNullOrEmpty(selectedShipmentId))
            SelectedShipmentId=string.Concat(
            SelectedYear,
            SelectedWeek);

        MasterList=await TraceDataService.GetLogisticData("ALL")??new List<Shipment>();
        Shipments=await TraceDataService.GetLogisticData(SelectedShipmentId)??new List<Shipment>();
        foreach(Shipment s in MasterList.Where(s => s.ShipmentId!=null).ToList())
        {
            if(!ShipmentIdList.Contains(s.ShipmentId)) ShipmentIdList.Add(s.ShipmentId);
        }
        var MasterListTemp = MasterList.Where(c => c.ShipmentId.StartsWith(SelectedShipmentId)).OrderBy(c => c.ShipmentId.Split('-')[2]);

        if(MasterListTemp.Any())
        {
            var e = int.Parse(MasterListTemp.Last().ShipmentId.Split('-')[2]);
            ShipmentIdx=e+1;

        }

        SelectedShipmentId=string.Concat(
          SelectedYear,
          SelectedWeek,
          '-',
          ShipmentType??"SEA",
          $"-{ShipmentIdx}");

        await UpdateUI();
        return true;
    }

    public static int GetIso8601WeekOfYear(DateTime time)
    {
        // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll
        // be the same week# as whatever Thursday, Friday or Saturday are,
        // and we always get those right
        DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
        // if(day>=DayOfWeek.Monday&&day<=DayOfWeek.Wednesday)
        // {
        //     time=time.AddDays(3);
        // }

        // Return the week of our adjusted day
        return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
    }

    public async Task LoadByShipmentId(string value)
    {
        CollapseUploadedDetail=true;
        CollapseDataDetail=true;
        SelectedShipmentId=value;
        InvoiceNumber="";
        InvoiceStatus=false;
        SelectedContainerNo=string.Empty;

        if(!string.IsNullOrEmpty(SelectedShipmentId))
        {
            Shipments=MasterList.Where(s => s.ShipmentId==SelectedShipmentId);

        }
        else
        {
            Shipments=await TraceDataService.GetLogisticData(SelectedShipmentId)??new List<Shipment>();

        }

        InvoiceNumber=Shipments.FirstOrDefault().PackingListId.ToString();

        if(!InvoiceNumber.Equals(""))
        {
            InvoiceStatus=true;
        }

        SelectedContainerNo=Shipments.FirstOrDefault().ContainerNo.ToString();
        CollapseUploadedDetail=false;
        CollapseDataDetail=false;
        if (Shipments.Count() > 0)
            ShippingDate = Shipments.ToList().FirstOrDefault().ShippingDate;
        await UpdateUI();

    }
    public string? SelectedContainerNo { get; set; }
    public bool PORevised { get; private set; }
    public bool SecondPORevised { get; private set; } = false;

    private async void HandleInputContainerNo(KeyboardEventArgs e)
    {
        if(e.Key=="Enter")
        {
            if(string.IsNullOrEmpty(SelectedContainerNo)) return;
            await TraceDataService.UpdateContainerNoToShipment(SelectedShipmentId, SelectedContainerNo);
            await ResetInfo(true, 2);
            MasterList=await TraceDataService.GetLogisticData("ALL")??new List<Shipment>();
            Shipments=await TraceDataService.GetLogisticData(SelectedShipmentId)??new List<Shipment>();
            await UpdateUI();
        }
    }
    private async void HandleInvoiceNumber(KeyboardEventArgs e)
    {
        if(e.Key=="Enter")
        {
            if(string.IsNullOrEmpty(InvoiceNumber)) return;
            await TraceDataService.UpdateInvoiceNumberToShipment(SelectedShipmentId, InvoiceNumber);
            await ResetInfo(true, 1);
            MasterList=await TraceDataService.GetLogisticData("ALL")??new List<Shipment>();
            Shipments=await TraceDataService.GetLogisticData(SelectedShipmentId)??new List<Shipment>();
            await UpdateUI();
        }
    }
    async Task ResetInfo(bool backToStart, int flag)
    {
        if(backToStart)
        {
            switch(flag)
            {
                case 1:
                    InvoiceNumber="";

                    await UpdateUI();
                    await jSRuntime.InvokeVoidAsync("focusEditorByID", "InvoiceNumber");
                    break;
                case 2:
                    SelectedContainerNo="";

                    await UpdateUI();
                    await jSRuntime.InvokeVoidAsync("focusEditorByID", "ContainerNumber");
                    break;
                default:
                    break;
            }


        }
    }

    //IEnumerable<S> DataSource { get; set; }

    //protected override async Task OnInitializedAsync()
    //{
    //    //DataSource = await NwindDataService.GetEmployeesEditableAsync();
    //}
    void Grid_CustomizeEditModel(GridCustomizeEditModelEventArgs e)
    {
        if(e.IsNew)
        {
            var newShipment = (Shipment)e.EditModel;
            newShipment.PackingListId="here";
        }
    }

    async Task Grid_EditModelSavingInvoice(GridEditModelSavingEventArgs e)
    {
        var shipment = (Shipment)e.EditModel;

        if(await TraceDataService.UpdateInvoiceByIdx(shipment.Idx, shipment.PackingListId))
        {
            Toast.ShowSuccess("Update invoice success", "SUCCESS");
            (Shipments.Where(s => s.Idx==shipment.Idx).FirstOrDefault()).PackingListId=shipment.PackingListId;
        }
        else
        {
            Toast.ShowError("Update invoice fail", "FAIL");
        }


        await UpdateUI();
    }
    async Task Grid_EditModelSavingContainerNo(GridEditModelSavingEventArgs e)
    {
        //await jSRuntime.InvokeVoidAsync("ConsoleLog",((Shipment)e.EditModel).Idx,);
        var shipment = (Shipment)e.EditModel;

        if(await TraceDataService.UpdateContainerByIdx(shipment.Idx, shipment.ContainerNo))
        {
            Toast.ShowSuccess("Update container success", "SUCCESS");
            (Shipments.Where(s => s.Idx==shipment.Idx).FirstOrDefault()).ContainerNo=shipment.ContainerNo;
        }
        else
        {
            Toast.ShowError("Update container fail", "FAIL");
        }


        await UpdateUI();
    }

    async Task Grid_EditModelSavingShipDate(GridEditModelSavingEventArgs e)
    {
        //await jSRuntime.InvokeVoidAsync("ConsoleLog",((Shipment)e.EditModel).Idx,);
        var shipment = (Shipment)e.EditModel;

        if (await TraceDataService.UpdateShippingDateByIdx(shipment.Idx, shipment.ShippingDate))
        {
            Toast.ShowSuccess("Update Shipping Date success", "SUCCESS");
            (Shipments.Where(s => s.Idx == shipment.Idx).FirstOrDefault()).ShippingDate = shipment.ShippingDate;
        }
        else
        {
            Toast.ShowError("Update Shipping Date fail", "FAIL");
        }

        await UpdateUI();
    }
    protected string GetValidationMessage(EditContext editContext, string fieldName)
    {
        var field = editContext.Field(fieldName);
        return string.Join("\n", editContext.GetValidationMessages(field));
    }


    void ExpandAllRows_Click()
    {
        Grid?.ExpandAllGroupRows();
    }
    void CollapseAllRows_Click()
    {
        Grid?.CollapseAllGroupRows();
    }

    public async Task FinishShipment()
    {
        // Show confirm box
        ShowPopUpFinishShipment = true;
        await UpdateUI();

        // Process change rawdata to -2


    }
    public async void PopupClosingFinishShipment()
    {

            ShowPopUpFinishShipment = false;

            foreach (Shipment s in Shipments)
            {
                await TraceDataService.UpdateRawDataByIdx(s.Idx, -2);
            }
            MasterList = await TraceDataService.GetLogisticData("ALL") ?? new List<Shipment>();
            Shipments = await TraceDataService.GetLogisticData(SelectedShipmentId) ?? new List<Shipment>();
            await UpdateUI();

    }

    public async Task PopUpUpdateShipment() 
    {

        try
        {
            PORevised = false;
            if(OldShipmentToUpdate.Count() > 0)
            {
                SecondPORevised = true;
            } else
            {
                await SelectedFiles(FileName);
            }

            await UpdateUI();
        }
        catch (Exception ex)
        {
        }
    }

    public async Task SecondPopUpUpdateShipment()
    {
        try {
            if (OldShipmentToUpdate.Count() > 0)
            {
                foreach (Shipment shipment in OldShipmentToUpdate)
                {
                    var i = SelectedShipmentId.Contains("AIR") && !shipment.ShipMode.ToUpper().Contains("SEA")&&!shipment.ShipMode.ToUpper().Contains("DHL");
                    var j = SelectedShipmentId.Contains("SEA") && !shipment.ShipMode.ToUpper().Contains("AIR")&&!shipment.ShipMode.ToUpper().Contains("DHL"); ;
                    var z = SelectedShipmentId.Contains("DHL") && !shipment.ShipMode.ToUpper().Contains("AIR") && !shipment.ShipMode.ToUpper().Contains("SEA");
                    if (!string.IsNullOrEmpty(shipment.ShipMode) && (i || j || z))
                    {
                        if (!await TraceDataService.UpdatePackingList(shipment)) return;
                    }else
                    {
                        Toast.ShowError("Error ShipMode", "Error");
                    }
                }
            }
            await SelectedFiles(FileName);
            await UpdateUI();
        } catch(Exception ex) { 
        }
    }

    string FileName = "";
    protected async void SelectedFilesChanged(IEnumerable<UploadFileInfo> files)
    {
        //UploadVisible = files.ToList().Count > 0;
        FileName = files.FirstOrDefault().Name;
        PORevised = true;
        OldShipmentToUpdate = await TraceDataService.CheckExistShipmentId(SelectedShipmentId);
        await UpdateUI();
    }

    public async void PopupClosingUpdateShipment(PopupClosingEventArgs args)
    {
        PORevised = false;
        await UpdateUI();
       
    }

    public async void SecondPopupClosingUpdateShipment(PopupClosingEventArgs args)
    {
        SecondPORevised = false;
        await UpdateUI();

    }


}

