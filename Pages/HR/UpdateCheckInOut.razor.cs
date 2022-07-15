using System.Globalization;

using DevExpress.Blazor;
using DevExpress.Pdf.Native.BouncyCastle.Utilities;

using MESystem.Data.HR;
using MESystem.Service;

using Microsoft.AspNetCore.Components;

namespace MESystem.Pages.HR;

public partial class UpdateCheckInOut : ComponentBase
{
    private IEnumerable<CheckInOut>? checkInOuts;

    [Inject]
    public HRService HRDataService { get; private set; }

    public string TokenStr { get; set; }
    public Attendance Attendance { get; set; } = new Attendance();

    public DateTime From { get; set; }
    public DateTime To
    {
        get => to; set
        {
            to = value; if (From <= To)
            {
                _ = Task.Run(async () =>
                { //await CheckAttendance(CheckInOuts);
                    await UpdateUI();
                });
            }
        }
    }

    public IGrid? AttendanceList { get; set; }
    private string? cssFieldClass;
    private DateTime to;

    public int Count { get; set; }

    private bool showDetail;
    private bool showAll;
    private bool isLoading;

    public string? Hidden { get; set; }
    public bool ShowDetail { get => showDetail; set { showDetail = value; Hidden = value ? "collapse" : "Test"; _ = Task.Run(async () => { await UpdateUI(); }); } }
    public bool ShowAll
    {
        get => showAll; set
        {
            showAll = value;
            _ = Task.Run(async () => { await UpdateUI(); });
        }
    }
    public bool IsLoading
    {
        get => isLoading; set
        {
            isLoading = value;
            _ = Task.Run(async () => { await UpdateUI(); });
        }
    }
    public string? CssFieldClass { get => cssFieldClass; set { cssFieldClass = value; _ = Task.Run(async () => await UpdateUI()); } }

    public string? LoadingText { get; set; }
    public IEnumerable<Attendance> FingerTime { get; set; }

    public IEnumerable<CheckInOut>? CheckInOuts { get => checkInOuts; private set => checkInOuts = value; }
    public bool ShouldUpdateUI { get; private set; }

    private async Task UpdateUI()
    {
        ShouldUpdateUI = true;
        //Update UI
        if (ShouldRender())
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
        if (firstRender)
        {
            IsLoading = true;
            From = DateTime.Now.AddDays(-2);
            To = DateTime.Now.AddDays(-1);
            CssFieldClass = "Test";
            LoadingText = "Getting Data";
            TokenStr = "No Token";
            ShowDetail = false;
            ShowAll = false;
            await UpdateUI();
            CheckInOuts = new List<CheckInOut>();
            //CheckInOuts=await HRService.LoadCheckInOutInformation();
            CheckInOuts = await HRDataService.GetAllCheckInOut();
            CheckInOuts = CheckInOuts.Where(_ => _.TimeStr >= From && _.TimeStr <= To);
            await CheckAttendance(CheckInOuts.ToList());
            IsLoading = false;
            await UpdateUI();
        }
    }

    public async Task CheckAttendance(IEnumerable<CheckInOut> checkInOuts)
    {
        IsLoading = true;
        ShowDetail = false;
        await UpdateUI();
        CheckInOuts = await HRDataService.GetAllCheckInOut();
        CheckInOuts = CheckInOuts.Where(_ => _.TimeStr >= From && _.TimeStr <= To);
        await UpdateUI();
        DateTime date = DateTime.Now.AddDays(-2).AddHours(0).Date;
        //checkInOuts=checkInOuts.Where(_ => _.TimeStr>=From&&_.TimeStr<=To).ToList();
        checkInOuts = checkInOuts.Where(_ => _.TimeStr >= From && _.TimeStr <= To);
        await UpdateUI();
        LoadingText = "Calculating";
        await UpdateUI();
        var results = (from p in checkInOuts
                       group p.TimeStr by new { p.UserEnrollNumber, p.UserFullName, p.UserIDTitle, p.TimeDate, p.Desc } into g
                       select new Attendance { UserID = g.Key.UserEnrollNumber, UserFullName = g.Key.UserFullName, UserIDTitle = g.Key.UserIDTitle, TimeDate = g.Key.TimeDate, TimeIn = g.Min(), TimeOut = g.Max() == g.Min() ? null : g.Max(), FingerTime = g.Count(), Desc = g.Key.Desc });

        FingerTime = results;
        IsLoading = false;
        await UpdateUI();
    }

    public async void GetAuthToken()
    {
        TokenStr = await HRDataService.GetAuthorizeToken();

        var rs = await HRDataService.GetCompanyInfo(TokenStr);

        TokenStr = rs.ToString();
        await UpdateUI();
    }

    public async void PutEmployeeInfo()
    {
        TokenStr = await HRDataService.GetAuthorizeToken();

        var emp1 = new Employee
        {

            companyCode = "FRIWO",
            code ="700476",
            payload =
        new Payload
        {
            attendeeCode = "700476",
            bankAccount = new BankAccount
            {
                account = "123456789",
                bankCode = "seabank"
            },
            dob = "2011-11-09",
            email = "nano@nanofin.tech",
            fullName = "BCD",
            gender = "Male",
            phoneNumber = "+84976058938",
            salary = new Salary
            {
                value = 10000000,
                currency = "VND"
            },
            socialIdDocument = new SocialIdDocument
            {
                issuedDate = "2011-11-09",
                number = "079096015890",
                issuedPlace = "Test"

            },
            startDate = "2011-11-09",
            status = "Active",
            workplaceId = "Test",
            workingDayPerMonth = 26,
            workplaceName = "Test",
            contractType = "FullTime",
            departmentName = "Hanh Chinh"
        }
        };

        //     var emp2 = new Employee
        //     {

        //         companyCode="FRIWO",
        //         code="700516",
        //         payload=
        //    new Payload
        //    {
        //        attendeeCode="240367109438181376-81050",
        //        bankAccount=new BankAccount
        //        {
        //            account="123456789",
        //            bankCode="seabank"
        //        },
        //        dob="2011-11-09",
        //        email="nano@nanofin.tech",
        //        fullName="B",
        //        gender="Male",
        //        phoneNumber="+84976058938",
        //        salary=new Salary
        //        {
        //            value=10000000,
        //            currency="VND"
        //        },
        //        socialIdDocument=new SocialIdDocument
        //        {
        //            issuedDate="2011-11-09",
        //            number="079096015890",
        //            issuedPlace="Test"

        //        },
        //        startDate="2011-11-09",
        //        status="Active",
        //        workplaceId="Test",
        //        workingDayPerMonth=22,
        //        workplaceName="Test",
        //        contractType="FullTime",
        //        departmentName="Hanh Chinh"
        //    }
        //     };


        //     var emp3 = new Employee
        //     {

        //         companyCode="FRIWO",
        //         code="700517",
        //         payload=
        //    new Payload
        //    {
        //        attendeeCode="240367109438181376-81050",
        //        bankAccount=new BankAccount
        //        {
        //            account="123456789",
        //            bankCode="seabank"
        //        },
        //        dob="2011-11-09",
        //        email="nano@nanofin.tech",
        //        fullName="C",
        //        gender="Male",
        //        phoneNumber="+84976058938",
        //        salary=new Salary
        //        {
        //            value=10000000,
        //            currency="VND"
        //        },
        //        socialIdDocument=new SocialIdDocument
        //        {
        //            issuedDate="2011-11-09",
        //            number="079096015890",
        //            issuedPlace="Test"

        //        },
        //        startDate="2011-11-09",
        //        status="Active",
        //        workplaceId="Test",
        //        workingDayPerMonth=22,
        //        workplaceName="Test",
        //        contractType="FullTime",
        //        departmentName="Hanh Chinh"
        //    }
        //     };


        List<Employee> empls = new List<Employee>();
        empls.Add(emp1);
        // empls.Add(emp2);
        // empls.Add(emp3);
        var rs = await HRDataService.GetCompanyInfo(TokenStr);
        TokenStr = await HRDataService.GetAuthorizeToken();
        var rs1 = await HRDataService.PutEmployeeInfo(empls, TokenStr);

        TokenStr = rs1;
        await UpdateUI();
    }

    public async void PutAttendeeInfo()
    {
        TokenStr = await HRDataService.GetAuthorizeToken();
        List<Attendee> attendees = new List<Attendee>();

        foreach (var item in FingerTime)
        {
            var timess = new List<string>();
            if (item != null && item.UserID == 700476 && item.Offset.HasValue)
            {
                timess.Add(item.TimeIn.Value.ToString("HH:mm"));
                timess.Add(item.TimeOut.Value.ToString("HH:mm"));

                attendees.Add(new Attendee
                {
                    attendeeCode = item.UserID.ToString(),
                    code = item.TimeDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    payload = new AttendancePayload
                    {
                        date = item.TimeDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                        shiftCount = item.Checked ? 1 : 0,

                        times = timess,

                        type = "Standard"
                    }
                });

            }
        }

        TokenStr = await HRDataService.GetAuthorizeToken();
        var rs1 = await HRDataService.PutAttendanceInfo(attendees, TokenStr);

        TokenStr = rs1;
        await UpdateUI();
    }


}