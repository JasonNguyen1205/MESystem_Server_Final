using MESystem.Data.HR;

namespace MESystem.Pages.HR;

public partial class UpdateCheckInOut
{
    private IEnumerable<CheckInOut>? checkInOuts;

    public int Count { get; set; }

    public IEnumerable<CheckInOut> FingerTime { get; set; }

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
            CheckAttendance(CheckInOuts.ToList());
            await Task.Delay(1);
            await InvokeAsync(StateHasChanged);
        }
    }

    public async void CheckAttendance(List<CheckInOut> checkInOuts)
    {
        var results = from p in checkInOuts
                      group p.TimeDate by p.UserEnrollNumber into g
                      select new { UserId = g.Key, Count = g.Count()};

    }


}
