using MESystem.Data.HR;

namespace MESystem.Pages.HR;

public class Attendance
{
    public long UserID { get; set; }
    public DateTime? TimeDate { get; set; }
    public int FingerTime { get; set; }
}

public partial class UpdateCheckInOut
{
    private IEnumerable<CheckInOut>? checkInOuts;

    public int Count { get; set; }

    public IEnumerable<Attendance> FingerTime { get; set; }

    public IEnumerable<CheckInOut>? CheckInOuts { get => checkInOuts; private set => checkInOuts=value; }

    protected override async void OnAfterRender(bool firstRender)
    {
        if(firstRender)
        {
            CheckInOuts=new List<CheckInOut>();
            CheckInOuts=await HRService.LoadCheckInOutInformation();
            CheckInOuts=CheckInOuts.Where(_ => _.TimeDate>DateTime.Now.AddDays(-2).Date);
            await Task.Delay(1);
            await InvokeAsync(StateHasChanged);
            await CheckAttendance(CheckInOuts.ToList());
            await Task.Delay(1);
            await InvokeAsync(StateHasChanged);
            

            IEnumerable<Attendance> results = ((IEnumerable<Attendance>)(from p in CheckInOuts
                          group p by new { p.UserEnrollNumber, p.TimeDate } into g
                          where g.Count()>2
                          orderby g.Count()
                          select new Attendance { UserID = g.Key.UserEnrollNumber, TimeDate = g.Key.TimeDate, FingerTime = g.Count() }));
            FingerTime=results;
            await Task.Delay(1);
            await InvokeAsync(StateHasChanged);
            //var rs = CheckInOuts.Where(_ => _.UserEnrollNumber==results.FirstOrDefault().UserId;
            //results.ForEach((_) => { _.FingerTime=result.Count; _.Attendance=results.FirstOrDefault().Count>1; });
            //checks.Add(rs.FirstOrDefault());

        }
    }

    public async Task CheckAttendance(List<CheckInOut> checkInOuts)
    {
       
      
        //var results = from p in checkInOuts
        //              group p.TimeDate by p.UserEnrollNumber into g
        //              select new { UserId = g.Key, Count = g.Count()};

    }


}
