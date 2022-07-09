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
        {
            return await _context.CheckInOuts
                        .ToListAsync();
        }
    }

    public async Task<IEnumerable<CheckInOut>>
        GetCheckInOut(int iDFilter)
    {

        return await _context.CheckInOuts
                         .Where(w => w.TimeStr>(DateTime.Now.AddDays(-2)))
                         .AsNoTracking()
                         .ToListAsync();

    }
}
