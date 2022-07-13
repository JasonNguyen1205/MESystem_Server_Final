using System.Net.Http;

using DevExpress.Blazor;

using MESystem.Data.HR;
using MESystem.Service;

using Microsoft.AspNetCore.Components;

namespace MESystem.Pages.HR;

public partial class UpdateCheckInOut
{
    private IEnumerable<CheckInOut>? checkInOuts;

    [Inject]
    public HRService HRDataService { get; private set; }

    public string TokenStr { get; set; }
    public Attendance Attendance { get; set; } = new Attendance();

    public DateTime From { get; set; }
    public DateTime To { get => to; set { to=value; if(From<=To) { _=Task.Run(async () => { //await CheckAttendance(CheckInOuts); 
        await UpdateUI(); }); } } }

    public IGrid? AttendanceList { get; set; }
    private string? cssFieldClass;
    private DateTime to;

    public int Count { get; set; }

    private bool showDetail;
    private bool showAll;
    private bool isLoading;

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
            TokenStr="No Token";
            ShowDetail=false;
            ShowAll=false;
            await UpdateUI();
            //CheckInOuts=new List<CheckInOut>();
            //CheckInOuts=await HRService.LoadCheckInOutInformation();
            //CheckInOuts=await HRDataService.GetAllCheckInOut();
            //CheckInOuts=CheckInOuts.Where(_ => _.TimeStr>=From&&_.TimeStr<=To);
            //await CheckAttendance(CheckInOuts.ToList());
            IsLoading=false;
            await UpdateUI();
        }
    }

    public async Task CheckAttendance(IEnumerable<CheckInOut> checkInOuts)
    {
        IsLoading=true;
        ShowDetail=false;
        await UpdateUI();
        CheckInOuts=await HRDataService.GetAllCheckInOut();
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

    public async void GetAuthToken()
    {
        TokenStr=await HRDataService.GetAuthorizeToken();

        var rs = await HRDataService.GetCompanyInfo(TokenStr);

        TokenStr=rs.ToString();
        await UpdateUI();
    }

    public async void PutEmployeeInfo()
    {
        TokenStr=await HRDataService.GetAuthorizeToken();

        var emp1 = new Employee
        {
            CompanyCode="FRIWO",
            Code="700515",
            Payload=
        new Payload
        {
            AttendeeCode="240367109438181376-81050",
            BankAccount=new BankAccount
            {
                AccountNumber="123456789",
                BankCode="seabank"
            },
            Dob=DateTime.Now,
            Email="",
            FullName="A",
            Gender="",
            PhoneNumber="",
            Salary=new Salary
            {
                Value=10000000,
                Currency="VND"
            },
            SocialIdDocument=new SocialIdDocument
            {
                IssuedDate= DateTime.Parse("2011-11-09"),
                Number="079096015890",
                IssuedPlace=""

            },
            StartDate=DateTime.Now,
            Status=true,
            WorkplaceId=""
        }
        };

        var emp2 = new Employee
        {
            CompanyCode="FRIWO",
            Code="700516",
            Payload=
        new Payload
        {
            AttendeeCode="",
            BankAccount=new BankAccount
            {
                AccountNumber="",
                BankCode=""
            },
            Dob=DateTime.Now,
            Email="",
            FullName="B",
            Gender="",
            PhoneNumber="",
            Salary=new Salary
            {
                Value=10000000,
                Currency="VND"
            },
            SocialIdDocument=new SocialIdDocument
            {
                Date=DateTime.Now,
                Number="",
                Place=""

            },
            StartDate=DateTime.Now,
            Status=true,
            WorkplaceId=""
        }
         };

        var emp3 = new Employee
        {
            CompanyCode="FRIWO",
            Code="700517",
            Payload=
        new Payload
        {
            AttendeeCode="",
            BankAccount=new BankAccount
            {
                AccountNumber="",
                BankCode=""
            },
            Dob=DateTime.Now,
            Email="",
            FullName="C",
            Gender="",
            PhoneNumber="",
            Salary=new Salary
            {
                Value=10000000,
                Currency="VND"
            },
            SocialIdDocument=new SocialIdDocument
            {
                Date=DateTime.Now,
                Number="",
                Place=""

            },
            StartDate=DateTime.Now,
            Status=true,
            WorkplaceId=""
        }
         };

        List<Employee> empls = new List<Employee>();
        empls.Add(emp1);
        empls.Add(emp2);
        empls.Add(emp3);

        var rs = await HRDataService.GetCompanyInfo(TokenStr);
        TokenStr=await HRDataService.GetAuthorizeToken();
        var rs1 = await HRDataService.PutEmployeeInfo(empls, TokenStr);

        TokenStr=rs+rs1;
        await UpdateUI();
    }

}