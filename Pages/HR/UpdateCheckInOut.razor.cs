using DevExpress.Blazor;

using MESystem.Data.HR;

namespace MESystem.Pages.HR;

public partial class UpdateCheckInOut
{
    private IEnumerable<CheckInOut>? checkInOuts;
    private bool showDetail;
    private bool showAll;
    private bool isLoading;
    public Attendance Attendance { get; set; } = new Attendance();

    public DateTime From { get; set; }
    public DateTime To { get => to; set { to=value; if(From<=To) { _=Task.Run(async () => { await CheckAttendance(CheckInOuts); await UpdateUI(); }); } } }

    public IGrid? AttendanceList { get; set; }
    private string? cssFieldClass;
    private DateTime to;

    public int Count { get; set; }
    public string? Hidden { get; set; }
    public bool ShowDetail { get => showDetail; set { showDetail=value; Hidden=value ? "collapse" : ""; _=Task.Run(async () => { await UpdateUI(); }); } }
    public bool ShowAll
    {
        get => showAll; set
        {
            showAll=value;
            _=Task.Run(async () => { await UpdateUI(); });
        }
    }
    public bool IsLoading
    {
        get => isLoading; set
        {
            isLoading=value;
            _=Task.Run(async () => { await UpdateUI(); });
        }
    }
    public string? CssFieldClass { get => cssFieldClass; set { cssFieldClass=value; _=Task.Run(async () => await UpdateUI()); } }

    public string? LoadingText { get; set; }
    public IEnumerable<Attendance> FingerTime { get; set; }

    public IEnumerable<CheckInOut>? CheckInOuts { get => checkInOuts; private set => checkInOuts=value; }
    public bool ShouldUpdateUI { get; private set; }

    private async Task UpdateUI()
    {
        ShouldUpdateUI=true;
        //Update UI
        if(ShouldRender())
        {
            await Task.Delay(5);
            await InvokeAsync(StateHasChanged);
            await Task.Delay(5);
            //ShouldUpdateUI = false;
        }

        Console.WriteLine("UI is updated");
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if(firstRender)
        {
            IsLoading=true;
            From=DateTime.Now.AddDays(-2);
            To=DateTime.Now.AddDays(-1); 
            CssFieldClass="";
            LoadingText="Getting Data";
            ShowDetail=false;
            ShowAll=false;
            await UpdateUI();
            CheckInOuts=new List<CheckInOut>();
            //CheckInOuts=await HRService.LoadCheckInOutInformation();
            CheckInOuts=await HRService.GetAllCheckInOut();
            CheckInOuts=CheckInOuts.Where(_ => _.TimeStr>=From&&_.TimeStr<=To);
            await CheckAttendance(CheckInOuts.ToList());
            IsLoading=false;
            await UpdateUI();
        }
    }

    public async Task CheckAttendance(IEnumerable<CheckInOut> checkInOuts)
    {
        IsLoading=true;
        ShowDetail=false;
        await UpdateUI();
        CheckInOuts=await HRService.GetAllCheckInOut();
        CheckInOuts=CheckInOuts.Where(_ => _.TimeStr>=From&&_.TimeStr<=To);
        await UpdateUI();
        //DateTime date = DateTime.Now.AddDays(-2).AddHours(0).Date;
        //checkInOuts=checkInOuts.Where(_ => _.TimeStr>=From&&_.TimeStr<=To).ToList();
        checkInOuts=checkInOuts.Where(_ => _.TimeStr>=From&&_.TimeStr<=To);
        await UpdateUI();
        LoadingText="Calculating";
        await UpdateUI();
        var results = (from p in checkInOuts
                       group p.TimeStr by new { p.UserEnrollNumber, p.UserFullName, p.UserIDTitle, p.TimeDate, p.Desc } into g
                       select new Attendance { UserID=g.Key.UserEnrollNumber, UserFullName=g.Key.UserFullName, UserIDTitle=g.Key.UserIDTitle, TimeDate=g.Key.TimeDate, TimeIn=g.Min(), TimeOut=g.Max()==g.Min() ? null : g.Max(), FingerTime=g.Count(), Desc=g.Key.Desc });

        FingerTime=results;
        IsLoading=false;
        await UpdateUI();
    }

}