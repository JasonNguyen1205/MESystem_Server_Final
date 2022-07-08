using MESystem.Data.HR;
using MESystem.Data.SetupInstruction;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace MESystem.Service
{
    public class HRService
    {
        private readonly HRDbContext _context;
        public HRService(HRDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HRCheckInOut>>
            LoadCheckInOutInformation()
        {
            {
                return await _context.CheckInOuts
                                     .OrderBy(p => p).ThenBy(p => p.TimeStr)
                                     .AsNoTracking()
                                     .ToListAsync();
            }
        }

        public async Task<IEnumerable<HRCheckInOut>>
            GetCheckInOut(int iDFilter)
        {
           
                return await _context.CheckInOuts
                                 .Where(w => w.UserEnrollNumber == iDFilter)
                                 .AsNoTracking()
                                 .ToListAsync();
          
        }
    }
}
