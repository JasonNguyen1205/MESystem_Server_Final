﻿using MESystem.Data.SetupInstruction;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MESystem.Data
{
    public class SmtService
    {
        private readonly SiDbContext _context;
        public SmtService(SiDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<vProgramInformation>>
            LoadProgramInformation()
        {
            {
                return await _context.ProgramInformations
                                     .OrderBy(p => p.PartNo).ThenBy(p => p.Side)
                                     .AsNoTracking()
                                     .ToListAsync();
            }
        }

        public async Task<IEnumerable<TProductionPlanSMT>>
            GetProductionPlanSMT(string lineFilter)
        {
            if (lineFilter.Equals("all"))
            {
                return await _context.ProductionPlanSMT
                                 .AsNoTracking()
                                 .ToListAsync();
            }
            else if (lineFilter.Equals("unplanned"))
            {
                return await _context.ProductionPlanSMT
                                 .Where(w => w.LineDescription == "N/A")
                                 .AsNoTracking()
                                 .ToListAsync();
            }
            else
            {
                return await _context.ProductionPlanSMT
                                 .Where(w => w.LineDescription == lineFilter)
                                 .AsNoTracking()
                                 .ToListAsync();
            }
        }
    }
}
