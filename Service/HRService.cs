using System.Data;

using MESystem.Data.HR;

using Microsoft.EntityFrameworkCore;

namespace MESystem.Service;

public class HRService
{
    private readonly HRDbContext _context;
    public HRService(HRDbContext context)
    {
        _context=context;
    }

    public async Task<IEnumerable<CheckInOut>>
        LoadCheckInOutInformation()
    {
       
            return await _context.CheckInOuts
            .Where(_=>_.TimeStr>DateTime.Now.AddDays(-2))
                        .AsNoTracking()
                        .ToListAsync();
       
    }

    public async Task<IEnumerable<CheckInOut>>
        GetCheckInOut(int iDFilter)
    {

        return await _context.CheckInOuts
                         .Where(w => w.TimeStr>(DateTime.Now.AddDays(-2)))
                         .AsNoTracking()
                         .ToListAsync();

    }

    public async Task<IEnumerable<CheckInOut>>
       GetAllCheckInOut()
    { 
    var td = from s in (from s in _context.CheckInOuts
    join r in _context.UserInfos on s.UserEnrollNumber equals r.UserEnrollNumber
    select new CheckInOut { TimeStr=s.TimeStr,UserEnrollNumber=s.UserEnrollNumber,TimeDate=s.TimeDate,MachineNo=s.MachineNo, UserFullName=r.UserFullName, UserIDTitle=r.UserIDTitle,UserIDD=r.UserIDD }) join r in _context.RelationDepts on s.UserIDD equals r.ID select new CheckInOut { TimeStr=s.TimeStr, UserEnrollNumber=s.UserEnrollNumber, TimeDate=s.TimeDate, MachineNo=s.MachineNo, UserFullName=s.UserFullName, UserIDTitle=s.UserIDTitle, UserIDD=s.UserIDD, Desc=r.Description };

        return await td.ToListAsync();
    }
}
