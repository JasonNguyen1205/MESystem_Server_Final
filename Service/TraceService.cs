﻿using DevExpress.Blazor.Internal;
using MESystem.Data.TRACE;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace MESystem.Data
{
    public class TraceService
    {
        private readonly TraceDbContext _context;
        public TraceService(TraceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductionLine>>
            LoadProductionLines(string departmentFilter)
        {
            if (departmentFilter.Equals("all"))
            {
                return await _context.ProductionLines
                                .AsNoTracking()
                                .ToListAsync();
            }
            else
            {
                return await _context.ProductionLines
                                .Where(s => s.DepartmentNo == departmentFilter)
                                .AsNoTracking()
                                .ToListAsync();
            }
        }

        public async Task<IEnumerable<vProductionPlan>>
            GetProductionPlan(string department, string workcenterFilter)
        {
            string[] stringsToSearchFor = workcenterFilter.Split(",");
            if (!stringsToSearchFor.Contains("T&J"))
            {
                return await _context.ProductionPlan
                                     .Where(s => s.DepartmentNo == department && stringsToSearchFor.Contains(s.WorkCenterNo))
                                     .OrderBy(o => o.LineDescription).ThenBy(o => o.PlannedStartTime).ThenBy(o => o.ProductionOrder)
                                     .AsNoTracking()
                                     .ToListAsync();
            }
            else
            {
                string[] departmentFilter = department.Split(",");
                return await _context.ProductionPlan
                                     .Where(s => departmentFilter.Contains(s.DepartmentNo))
                                     .OrderBy(o => o.LineDescription).ThenBy(o => o.PlannedStartTime).ThenBy(o => o.ProductionOrder)
                                     .AsNoTracking()
                                     .ToListAsync();
            }
        }

        public async Task<IEnumerable<vProductionPlanJigs>>
            GetProductionPlanToolsJigs(string department, string workcenterFilter)
        {
            string[] stringsToSearchFor = workcenterFilter.Split(",");
            if (!stringsToSearchFor.Contains("T&J"))
            {
                return await _context.ProductionPlanJigs
                                     //.Where(s => s.DepartmentNo == department && stringsToSearchFor.Contains(s.WorkCenterNo))
                                     .Where(s => s.DepartmentNo == department)
                                     .OrderBy(o => o.LineDescription).ThenBy(o => o.PlannedStartTime).ThenBy(o => o.PlannedStartTime)
                                     .AsNoTracking()
                                     .ToListAsync();
            }
            else
            {
                string[] departmentFilter = department.Split(",");
                return await _context.ProductionPlanJigs
                                     .Where(s => departmentFilter.Contains(s.DepartmentNo))
                                     .OrderBy(o => o.LineDescription).ThenBy(o => o.PlannedStartTime).ThenBy(o => o.PlannedStartTime)
                                     .AsNoTracking()
                                     .ToListAsync();
            }
        }

        public async Task<ProductionPlanJigs>
            GetToolsJigsDetailsOrderNo(string orderNo)
        {
            ProductionPlanJigs editFormData = null;

            var result = await _context.ProductionPlanJigs
                                       .Where(s => s.OrderNo == orderNo)
                                       .AsNoTracking()
                                       .ToListAsync();

            if (result.Count() != 0)
            {
                editFormData = new ProductionPlanJigs()
                {
                    OrderNo = result.FirstOrDefault().OrderNo,
                    PartNo = result.FirstOrDefault().PartNo,
                    PartDescription = result.FirstOrDefault().PartDescription,
                    LineDescription = result.FirstOrDefault().LineDescription,
                    Location = result.FirstOrDefault().Location
                };
            }
            else
            {
                editFormData = new ProductionPlanJigs()
                {
                    OrderNo = "",
                    PartNo = "NO DATA",
                    PartDescription = "",
                    LineDescription = "",
                    Location = ""
                };
            }

            return editFormData;
        }

        public async Task<IEnumerable<vProductionPlanIFS>>
            GetShopOrderStatus(string department)
        {
            if (department.Equals(""))
            {
                return await _context.ProductionPlanIfs
                                     .Where(pp => pp.PercentDone >= 90)
                                     .Distinct()
                                     .OrderBy(pp => pp.DepartmentNo).ThenByDescending(pp => pp.QtyNotBooked).ThenByDescending(pp => pp.PercentDone)
                                     .AsNoTracking()
                                     .ToListAsync();
            }
            else
            {
                return await _context.ProductionPlanIfs
                                     .Where(pp => pp.PercentDone >= 90 && pp.DepartmentNo == department)
                                     .Distinct()
                                     .OrderBy(pp => pp.DepartmentNo).ThenByDescending(pp => pp.QtyNotBooked).ThenByDescending(pp => pp.PercentDone)
                                     .AsNoTracking()
                                     .ToListAsync();
            }
        }

        //public async Task<IEnumerable<TempProductionPlanSMT>>
        //    GetFullProductionPlan(IEnumerable<vProgramInformation> programInformation, string departmentFilter)
        //{
        //    var prodPlanSMT = await _context.ProductionPlanSMT
        //                                         .Where(s => s.DepartmentNo == departmentFilter)
        //                                         //.OrderBy(o => o.LineDescription).ThenBy(o => o.PlannedStartTime).ThenBy(o => o.ProductionOrder).ThenBy(o => o.OrderNo).ThenBy(o => o.OperationNo)
        //                                         .AsNoTracking()
        //                                         .ToListAsync();

        //    return (from pp in prodPlanSMT
        //            join pi in programInformation
        //            on pp.PartNo + pp.Side equals pi.PartNo + pi.Side
        //            select new TempProductionPlanSMT()
        //            {
        //                Id = pp.Id,
        //                LineId = pp.LineId,
        //                LineDescription = pp.LineDescription,
        //                DepartmentNo = pp.DepartmentNo,
        //                Department = pp.Department,
        //                OrderNo = pp.OrderNo,
        //                OperationNo = pp.OperationNo,
        //                IfsVersion = pp.IfsVersion,
        //                PartNo = pp.PartNo,
        //                PartDescription = pp.PartDescription.Replace("PCB automatic SMD ", ""),
        //                Side = pp.Side,
        //                PcbPartNo = pp.PcbPartNo,
        //                PcbDescription = pp.PcbDescription,
        //                PcbGroup = pi.PcbGroup,
        //                QtyPlanned = pp.QtyPlanned,
        //                NeedDate = pp.NeedDate,
        //                TargetRuntime = pp.TargetRuntime,
        //                TargetPartsPerHourIFS = pp.TargetPartsPerHourIFS,
        //                PlannedStartTime = pp.PlannedStartTime,
        //                ProductionOrder = pp.ProductionOrder,
        //                ProductionNote = pp.ProductionNote,
        //                Line1 = pi.Line1,
        //                Line2 = pi.Line2,
        //                Line3 = pi.Line3,
        //                Line4 = pi.Line4,
        //                Aoi = pi.Aoi,
        //                Spi = pi.Spi,
        //                Stencil = pi.Stencil
        //            })
        //            .ToList()
        //            .OrderBy(o => o.LineDescription).ThenBy(o => o.PlannedStartTime).ThenBy(o => o.ProductionOrder).ThenBy(o => o.OrderNo).ThenBy(o => o.OperationNo);
        //}

        //public async Task<IEnumerable<TRACE.vProductionPlanSMTdef>>
        //        GetProductionPlanSMT(string departmentFilter)
        //{
        //    if (departmentFilter.Equals("all"))
        //    {
        //        return await _context.ProductionPlanSMT
        //                             .OrderBy(o => o.LineDescription).ThenBy(o => o.PlannedStartTime).ThenBy(o => o.ProductionOrder).ThenBy(o => o.OrderNo).ThenBy(o => o.OperationNo)
        //                             .AsNoTracking()
        //                             .ToListAsync();
        //    }
        //    else
        //    {
        //        return await _context.ProductionPlanSMT
        //                             .Where(s => s.DepartmentNo == departmentFilter)
        //                             .OrderBy(o => o.LineDescription).ThenBy(o => o.PlannedStartTime).ThenBy(o => o.ProductionOrder).ThenBy(o => o.OrderNo).ThenBy(o => o.OperationNo)
        //                             .AsNoTracking()
        //                             .ToListAsync();
        //    }
        //}

        public async Task<IEnumerable<ProductionPlanLine>>
            GetProductionPlanLinesAsync(string departmentFilter)
        {
            return await _context.ProductionPlanLines
                                 .Where(s => s.DepartmentNo == departmentFilter)
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<ProductionPlanLine>
            UpdateProductionValues(ProductionPlanLine dataItem, bool newInsert)
        {
            if (newInsert)
            {
                _context.ProductionPlanLines
                        .Add(dataItem);
                await _context.SaveChangesAsync();
            }
            else
            {
                var updateQuery = await _context.ProductionPlanLines
                .FirstAsync(c => c.DepartmentNo == dataItem.DepartmentNo && c.OrderNo == dataItem.OrderNo && c.OperationNo == dataItem.OperationNo);

                updateQuery.LineId = dataItem.LineId;
                updateQuery.PlannedStartTime = dataItem.PlannedStartTime;
                updateQuery.ProductionOrder = dataItem.ProductionOrder;
                updateQuery.ModificationTime = DateTime.Now;

                _context.ProductionPlanLines
                        .Update(updateQuery);
                await _context.SaveChangesAsync();
            }

            return null;
        }

        ProductionPlanLine[] RemoveInternal(ProductionPlanLine dataItem)
        {
            _context.ProductionPlanLines.Attach(dataItem);
            _context.ProductionPlanLines.Remove(dataItem);
            _context.SaveChanges();
            return _context.ProductionPlanLines.ToArray();
        }

        public Task<ProductionPlanLine[]> Remove(ProductionPlanLine dataItem)
        {
            return Task.FromResult(RemoveInternal(dataItem));
        }

        public int CountTotalQty(int? prepotting, string orderNo)
        {
            string query = "";

            if (prepotting != null)
            {
                query = "SELECT COUNT(DISTINCT BARCODE) AS TOTAL_QUANTITIY FROM data_ate_pre_potting WHERE ORDER_NO = '" + orderNo + "' ";
            }
            else
            {
                query = "SELECT COUNT(BARCODE) AS TOTAL_QUANTITIY FROM TRACE.finished_good_ps WHERE ORDER_NO = '" + orderNo + "' ";
            }

            var result = _context.TotalQuantity
                                 .FromSqlRaw(query);

            return result.FirstOrDefault().TOTAL_QUANTITIY;
        }

        public Task<vShopOrderStates[]> GetShopOrderState(string filter)
        {
            if (filter == "")
            {
                return _context.ShopOrderState
                               .AsNoTracking()
                               .ToArrayAsync();
            }
            else
            {
                return _context.ShopOrderState
                               .Where(s => s.Department == filter)
                               .AsNoTracking()
                               .ToArrayAsync();
            }
        }

        public Task<vShopOrderStateMI[]> GetShopOrderStateMI(string filter)
        {
            if (filter == "")
            {
                return _context.ShopOrderStateMI
                .AsNoTracking()
                .ToArrayAsync();
            }
            else
            {
                return _context.ShopOrderStateMI
                .Where(s => s.Department == filter)
                .AsNoTracking()
                .ToArrayAsync();
            }
        }

        public Task<vShopOrderLinks[]> GetLinkedShopOrders()
        {
            return _context.ShopOrderLink
            .AsNoTracking()
            .ToArrayAsync();
        }

        public async Task<IEnumerable<ModelProperties>>
            GetPartNoList()
        {
            return await _context.ModelProperties
                                 .OrderBy(o => o.Family).ThenBy(o => o.PartNo).ThenBy(o => o.Description)
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<IEnumerable<SiFamily>>
            GetFamilys()
        {
            return await _context.SiFamilys
                                 .OrderBy(o => o.Family)
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<IEnumerable<SiProduct>>
            GetProducts()
        {
            return await _context.SiProducts
                                 .Distinct()
                                 .OrderBy(o => o.PartNo)
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<IEnumerable<vSiProductFamily>>
            GetProductsFamilys()
        {
            return await _context.vSiProductFamilys
                                 .OrderByDescending(o => o.PartNo)
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<IEnumerable<SiManufacturingTool>>
            GetManufTools()
        {
            return await _context.SiManufacturingTools
                                 .OrderBy(o => o.DeviceNo).ThenBy(o => o.Description)
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<IEnumerable<SiManufacturingToolType>>
            GetManufToolTypes()
        {
            return await _context.SiManufacturingToolTypes
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<IEnumerable<vSiManufacturingToolPart>>
            GetManufToolParts()
        {
            return await _context.SiManufacturingToolParts
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<IEnumerable<vSiManufacturingToolPart>>
            GetManufToolPartsByPartNo(string partNo)
        {
            return await _context.SiManufacturingToolParts
                                 .Where(o => o.PartNo == partNo)
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<IEnumerable<vDepartmentStation>>
            GetDepartmentsStations()
        {
            return await _context.vDepartmentStations
                                 .OrderBy(o => o.Department).ThenBy(o => o.Station)
                                 .AsNoTracking()
                                 .ToListAsync();
        }


        public async Task<IEnumerable<vProductionLayout>>
            GetProductionLayouts(string partNo, string family)
        {
            return await _context.vProductionLayouts
                                 .Where(w => w.PartNo == partNo && w.Family == family)
                                 .OrderBy(o => o.PartNo).ThenBy(o => o.Id)
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task UpdateTargetPph(string productionLineId, int newTargetPPH)
        {
            var updateQuery = await _context.ProductionPlanLines.FirstOrDefaultAsync(c => c.Id == productionLineId);

            if (updateQuery != null)
            {
                updateQuery.TargetPPH = newTargetPPH;
                _context.ProductionPlanLines
                        .Update(updateQuery);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateBasePph(SiPartsPerHour newValues)
        {
            var updateQuery = await _context.SiPartPerhour.FirstOrDefaultAsync(c => c.partNo == newValues.partNo && c.workCenterNo == newValues.workCenterNo);

            if (updateQuery != null)
            {
                _context.SiPartPerhour
                        .Remove(updateQuery);
                await _context.SaveChangesAsync();
            }
            await _context.SiPartPerhour.AddAsync(newValues);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<vFinishedGoods>>
            GetOutputByOrderNoDay(string orderNo, string departmentNo)
        {
            IEnumerable<vFinishedGoods> result;

            if (departmentNo.Equals("V3100"))
            {
                result = await _context.vFinishedGoods
                                       .Where(f => f.OrderNo == orderNo)
                                       .GroupBy(r => new { r.OrderNo, r.PartNo, r.Day, r.Month, r.Year, r.PanelQty })
                                       .Select(g => new vFinishedGoods() { OrderNo = g.Key.OrderNo, PartNo = g.Key.PartNo, Day = g.Key.Day, Month = g.Key.Month, Year = g.Key.Year, QtyCounted = g.Count() * g.Key.PanelQty })
                                       .AsNoTracking()
                                       .ToListAsync();
            }
            else if (departmentNo.Equals("T3500"))
            {
                result = await _context.vFinishedGoods
                                        .Where(f => f.OrderNo == orderNo)
                                        .GroupBy(r => new { r.OrderNo, r.PartNo, r.Day, r.Month, r.Year })
                                        .Select(g => new vFinishedGoods() { OrderNo = g.Key.OrderNo, PartNo = g.Key.PartNo, Day = g.Key.Day, Month = g.Key.Month, Year = g.Key.Year, QtyCounted = g.Sum(s => s.BoxNumber) })
                                        .AsNoTracking()
                                        .ToListAsync();
            }
            else
            {
                result = await _context.vFinishedGoods
                                        .Where(f => f.OrderNo == orderNo)
                                        .GroupBy(r => new { r.OrderNo, r.PartNo, r.Day, r.Month, r.Year })
                                        .Select(g => new vFinishedGoods() { OrderNo = g.Key.OrderNo, PartNo = g.Key.PartNo, Day = g.Key.Day, Month = g.Key.Month, Year = g.Key.Year, QtyCounted = g.Count() })
                                        .AsNoTracking()
                                        .ToListAsync();
            }
            return result;
        }

        public async Task<IEnumerable<FinishedGood>>
            GetBoxContentInformation(string barcodeBox, string partNo)
        {
            var result = await _context.FinishedGood
                                       .Where(_ => _.BarcodeBox == barcodeBox && _.PartNo==partNo)
                                       .AsNoTracking()
                                       .ToListAsync();

            return result.Select(s => new FinishedGood() { Barcode = s.Barcode, OrderNo = s.OrderNo,PartNo = s.PartNo, BarcodeBox = s.BarcodeBox, DateOfPackingBox = s.DateOfPackingBox, QtyBox = result.Count(), InvoiceNumber = s.InvoiceNumber, Rev = result.FirstOrDefault().Barcode.Substring(7, 2) }).ToList().AsEnumerable();
        }

        public async Task<IEnumerable<FinishedGood>>
          GetPalletContentInformation(string barcodePallet)
        {
            var result = await _context.FinishedGood
                                       .Where(_ => _.BarcodePalette == barcodePallet)
                                       .AsNoTracking()
                                       .ToListAsync();

            return result.Select(s => new FinishedGood() { OrderNo = s.OrderNo, PartNo = s.PartNo, BarcodeBox = s.BarcodeBox, DateOfPackingBox = s.DateOfPackingBox, QtyPallet = result.Count(), InvoiceNumber = s.InvoiceNumber, Rev = result.FirstOrDefault().Barcode.Substring(7, 2) }).Take(1).ToList().AsEnumerable();
        }

        public async Task<IEnumerable<FinishedGood>>
            GetQtyOfAddedPoNumbers(string poNumber, string partNo)
        {
            int qty = 0;
            var p0 = new OracleParameter("p0",OracleDbType.Varchar2, 2000,poNumber,ParameterDirection.Input);
            var p1 = new OracleParameter("p1", OracleDbType.Varchar2, 2000, partNo, ParameterDirection.Input);
            var p3 = new OracleParameter("p3", OracleDbType.Int16, qty, ParameterDirection.Output);
            
            var rs = await _context.FinishedGood.FromSqlInterpolated($"select * from TRACE.FINISHED_GOOD_PS where invoice_number = {p0} and part_no = {p1}").ToListAsync();
            
            return rs;
        }

        public async Task<IEnumerable<CustomerOrder>>
            GetCustomerOrders()
        {
            var rs = await _context.CustomerOrders
                //.Where(f => f.QtyInvoiced > 0)
                //(f=>f.CustomerPoNo)
                .AsAsyncEnumerable().ToEnumerableAsync();
            return rs;
        }

        public async Task<bool>
            InsertPurchaseOrderNo(string barcodeBox, string poNumber)
        {
            var updateQuery = _context.FinishedGood
                                       .Where(c => c.BarcodeBox == barcodeBox).ToListAsync();

            if (updateQuery != null)
            {
                var p0 = new OracleParameter("p0",OracleDbType.Varchar2, 2000,poNumber,ParameterDirection.Input);
                var p1 = new OracleParameter("p1", OracleDbType.Varchar2, 2000, barcodeBox, ParameterDirection.Input);

                var rs = await _context.Database.ExecuteSqlInterpolatedAsync($"UPDATE TRACE.FINISHED_GOOD_PS SET INVOICE_NUMBER = {p0}, DATE_OF_SHIPPING = SYSDATE WHERE BARCODE_BOX = {p1}");
                if (rs > 0)
                {
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                };

            }
            else
            {
                return false;
            }
        }

        public async Task<IEnumerable<FinalResultFg>>
            GetFinalResult(string barcode)
        {
            return await _context.FinalResultFgs
                                 .Where(w => w.Barcode == barcode && w.Id == 28734334)
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task UpdateManufTool(SiManufacturingTool newValues)
        {
            var updateQuery = await _context.SiManufacturingTools.FirstOrDefaultAsync(mft => mft.Id == newValues.Id);

            if (updateQuery != null)
            {
                _context.SiManufacturingTools
                        .Remove(updateQuery);
                await _context.SaveChangesAsync();
            }
            await _context.SiManufacturingTools.AddAsync(newValues);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveManufTool(SiManufacturingTool newValues)
        {
            var updateQuery = await _context.SiManufacturingTools.FirstOrDefaultAsync(mft => mft.Id == newValues.Id);

            if (updateQuery != null)
            {
                _context.SiManufacturingTools
                        .Remove(updateQuery);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdatefToolType(SiManufacturingToolType newValues)
        {
            var updateQuery = await _context.SiManufacturingToolTypes.FirstOrDefaultAsync(mft => mft.Id == newValues.Id);

            if (updateQuery != null)
            {
                _context.SiManufacturingToolTypes
                        .Remove(updateQuery);
                await _context.SaveChangesAsync();
            }
            await _context.SiManufacturingToolTypes.AddAsync(newValues);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateManufToolLink(SiManufacturingToolLink newValues)
        {
            var updateQuery = await _context.SiManufacturingToolLinks.FirstOrDefaultAsync(mftl => mftl.Id == newValues.Id);

            if (updateQuery != null)
            {
                _context.SiManufacturingToolLinks
                        .Remove(updateQuery);
                await _context.SaveChangesAsync();
            }
            await _context.SiManufacturingToolLinks.AddAsync(newValues);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveManufToolLink(vSiManufacturingToolPart newValues)
        {
            var updateQuery = await _context.SiManufacturingToolLinks.FirstOrDefaultAsync(mftl => mftl.Id == newValues.Id);

            if (updateQuery != null)
            {
                _context.SiManufacturingToolLinks
                        .Remove(updateQuery);
                await _context.SaveChangesAsync();
            }
        }

        // Scan Pallete 

        //Check Box exist
        public async Task<IEnumerable<FinishedGood>?>
           CheckExistBarcodeBox(string barcodeBox, string pONo)
        {
            var query = await _context.FinishedGood
                                 .Where(f => f.BarcodeBox == barcodeBox && f.InvoiceNumber == pONo).ToListAsync();
            return query.AsEnumerable();
        }

        //Check box is linked to pallete
        public async Task<IEnumerable<FinishedGood>?>
          CheckBoxInAnyPallete(string barcodeBox)
        {
            var query = await _context.FinishedGood
                                 .Where(f => f.BarcodeBox == barcodeBox && f.BarcodePalette != null).ToListAsync();
            if (query == null) return null;
            return query.AsEnumerable();
        }

        //Check box is linked to PO
        public async Task<IEnumerable<FinishedGood>?>
         CheckBoxLinkedToPO(string barcodeBox)
        {
            var query = await _context.FinishedGood
                                 .Where(f => f.BarcodeBox == barcodeBox && f.InvoiceNumber == null).ToListAsync();
            return query.AsEnumerable();
        }

        public async Task<int> GetMaxPaletteNumber(string part_no)
        {
            var Flag = new OracleParameter("P_FLAG", OracleDbType.Decimal, 100, 1, ParameterDirection.Input);
            var Part_No = new OracleParameter("P_PART_NO", OracleDbType.NVarchar2, 200, part_no, ParameterDirection.Input);
            var output = new OracleParameter("P_MAX_BOX_OR_PALETTE", OracleDbType.Decimal, 100, ParameterDirection.Output);
            var res = await _context.Database.ExecuteSqlInterpolatedAsync($"BEGIN TRACE.TRS_FINISHED_GOOD_PS_PKG.GET_MAX_BOX_OR_PALETTE_PRC({Flag},{Part_No},{output}); END;", default);
            int max_number = 0;
            Console.WriteLine(output.Value);
            max_number = int.Parse(output.Value.ToString());
            return max_number;
        }

        public async Task UpdateFinishedGood(string barcode_box, string barcode_palette, int palette_number)
        {

            var parameters = new OracleParameter[0];
            var Flag = new OracleParameter("P_FLAG", OracleDbType.Decimal, 100, 1, ParameterDirection.Input);
            var Barcode_Box = new OracleParameter("P_INPUT", OracleDbType.NVarchar2, barcode_box, ParameterDirection.Input);
            var Barcode_Palette = new OracleParameter("P_BARCODE_PALETTE", OracleDbType.NVarchar2, barcode_palette, ParameterDirection.Input);
            var Palette_number = new OracleParameter("P_PALETTE_NUMBER", OracleDbType.Decimal, palette_number, ParameterDirection.Input);
            var Dates = new OracleParameter("P_DATE_OF_PACKING_PALETTE", OracleDbType.Date, DateTime.Now, ParameterDirection.Input);

            var res = await _context.Database.ExecuteSqlInterpolatedAsync($"BEGIN TRACE.TRS_FINISHED_GOOD_PS_PKG.UPDATE_FINISHED_GOOD_PRC({Flag},{Barcode_Box},{Barcode_Palette},{Palette_number},{Dates}); END;", default);
            await _context.SaveChangesAsync();
            //}
        }


        public async Task<bool> VerifyPallet(string barcode_palette, 
            int state)
        {
            var p0 = new OracleParameter("p0", OracleDbType.Int32, 2000, state, ParameterDirection.Input);
            var p1 = new OracleParameter("p1", OracleDbType.Varchar2, 2000, barcode_palette, ParameterDirection.Input);

            var rs = await _context.Database.ExecuteSqlInterpolatedAsync($"UPDATE TRACE.FINISHED_GOOD_PS SET VERIFIED_PALLET = {p0} WHERE BARCODE_PALETTE = {p1}");
            
            if (rs > 0)
            {
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }


        public async Task<IEnumerable<ModelProperties>>
           GetFamily(string FamilyInput, string partNoInput)
        {
            return await _context.ModelProperties
                                 .Where(j => j.Routing_station == FamilyInput && j.PartNo == partNoInput)
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        // public async Task<string> GetCustomerVersion(int flag, string po_number)
        // {
        //     string max_number = "";
        //     var p1 = new OracleParameter("P_FLAG", OracleDbType.Int16, 100, flag, ParameterDirection.Input);
        //     var p2 = new OracleParameter("P_PART_NO", OracleDbType.NVarchar2, 200, po_number, ParameterDirection.Input);
        //     var p3 = new OracleParameter("P_OUT", OracleDbType.NVarchar2, 200, max_number, ParameterDirection.Output);
        //     var res = await _context.Database.ExecuteSqlInterpolatedAsync($"BEGIN TRACE.TRS_PLANNING_PKG.GET_CUSTOMER_VERSION_PRC({p1},{p2},{p3}); END;");
        //
        //     Console.WriteLine(p3.Value);
        //     max_number = p3.Value.ToString()??"0";
        //     return max_number;
        // }

        public async Task<int> GetQtyFromTrace(int flag, string part_number)
        {
            var flagParam = new OracleParameter("P_FLAG", OracleDbType.Decimal, 100, flag, ParameterDirection.Input);
            var partNo = new OracleParameter("P_PART_NO", OracleDbType.NVarchar2, 200, part_number, ParameterDirection.Input);
            var output = new OracleParameter("P_RESULT", OracleDbType.Decimal, 100, ParameterDirection.Output);
            var res = await _context.Database.ExecuteSqlInterpolatedAsync($"BEGIN TRACE.TRS_MODEL_PROPERTIES_PKG.GET_MODEL_PRC({flagParam},{partNo},{output}); END;");
            //if (res < 1) return 0;
            var rs = 0;
            if(output.Value!=null)
                rs = int.Parse(output.Value.ToString() ?? throw new InvalidOperationException());
            return rs;
        }

        public async Task<IEnumerable<CustomerRevision>> GetCustomerRevision(int flag, string poNo, string family, string partNo, string orderNo)
        {
            List<CustomerRevision> revisions = new List<CustomerRevision>();
            var flagParam = new OracleParameter("P_FLAG", OracleDbType.Decimal, 8, flag, ParameterDirection.Input);
            var poNoParam = new OracleParameter("P_CO_NO", OracleDbType.NVarchar2, 100, poNo, ParameterDirection.Input);
            var familyParam = new OracleParameter("P_FAMILY", OracleDbType.NVarchar2, 100, family, ParameterDirection.Input);
            var partNoParam = new OracleParameter("P_PART_NO", OracleDbType.NVarchar2, 100, partNo, ParameterDirection.Input);
            var orderNoParam = new OracleParameter("P_ORDER_NO", OracleDbType.NVarchar2, 100, orderNo, ParameterDirection.Input);
            var outputParam = new OracleParameter("P_REF_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
            //var res = await _context.Database.ExecuteSqlInterpolatedAsync($"BEGIN TRS_PLANNING_PKG.GET_CV_BY_FAMILY_PRC({familyParam},{Part_No}); END;");
            await using var context = _context;
            var conn = new OracleConnection(context.Database.GetConnectionString());
            var query = "TRS_PLANNING_PKG.GET_CV_BY_FAMILY_PRC";
            conn.Open();
            if (conn.State == ConnectionState.Open)
            {
                await using var command = conn.CreateCommand();
                command.CommandText = query;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(flagParam);
                command.Parameters.Add(poNoParam);
                command.Parameters.Add(familyParam);
                command.Parameters.Add(partNoParam);
                command.Parameters.Add(orderNoParam);
                command.Parameters.Add(outputParam);
                command.Connection = conn;
                OracleDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    revisions.Add(new CustomerRevision(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString()));
                }

                command.Parameters.Clear();
                reader.Dispose();
                command.Dispose();
            }

            conn.Dispose();
            return revisions.AsEnumerable();
        }

        public async Task<IEnumerable<CustomerRevision>> GetCustomerRevisionByPartNo(string partNo)
        {
            List<CustomerRevision> revisions = new List<CustomerRevision>();
            var flagParam = new OracleParameter("P_FLAG", OracleDbType.Int16, 3
                , ParameterDirection.Input);
            var poNoParam = new OracleParameter("P_CO_NO", OracleDbType.NVarchar2, 100, "", ParameterDirection.Input);
            var familyParam = new OracleParameter("P_FAMILY", OracleDbType.NVarchar2, 100, "", ParameterDirection.Input); ;
            var partNoParam = new OracleParameter("P_PART_NO", OracleDbType.NVarchar2, 100, partNo, ParameterDirection.Input);
            var orderNoParam = new OracleParameter("P_ORDER_NO", OracleDbType.NVarchar2, 100, "", ParameterDirection.Input);
            var outputParam = new OracleParameter("P_REF_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
            await using var context = _context;
            var conn = new OracleConnection(context.Database.GetConnectionString());
            var query = "TRS_PLANNING_PKG.GET_CV_BY_FAMILY_PRC";
            conn.Open();
            if (conn.State == ConnectionState.Open)
            {
                await using var command = conn.CreateCommand();
                command.CommandText = query;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(flagParam);
                command.Parameters.Add(poNoParam);
                command.Parameters.Add(familyParam);
                command.Parameters.Add(partNoParam);
                command.Parameters.Add(orderNoParam);
                command.Parameters.Add(outputParam);
                command.Connection = conn;
                OracleDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    revisions.Add(new CustomerRevision(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString()));
                }
                reader.Dispose();
                command.Dispose();
            }

            conn.Dispose();
            return revisions;
        }

        //Check BarcodeBox exist
        public async Task<IEnumerable<FinishedGood>?>
           CheckBarcodeBoxExist(string barcodeBox)
        {
            var query = await _context.FinishedGood
                                 .Where(f => f.BarcodeBox == barcodeBox && f.BarcodePalette == null).ToListAsync();
            return query.AsEnumerable();
        }

        //Check PartNo of BarcodeBox exist
        public async Task<IEnumerable<FinishedGood>?>
           CheckPartNoBarcodeBox(string barcodeBox, string partNo)
        {
            var query = await _context.FinishedGood
                                 .Where(_=> _.BarcodeBox == barcodeBox &&  _.PartNo == partNo).ToListAsync();
            return query.AsEnumerable();
        }

        public async Task<bool> UpdateBarcodeBox(string barcode_box, string barcode_box2)
        {
            var p0 = new OracleParameter("p0", OracleDbType.Varchar2, 2000, barcode_box, ParameterDirection.Input);
            var p1 = new OracleParameter("p1", OracleDbType.Varchar2, 2000, barcode_box2, ParameterDirection.Input);

            var status = await _context.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO TRACE.FINISHED_GOOD_PS_HISTORY(CONTRACT,RELEASE_NO,SEQUENCE_NO,BARCODE_ID,BARCODE,BARCODE_BOX,BOX_NUMBER,DATE_OF_PACKING,BARCODE_PALETTE,PALETTE_NUMBER,DATE_OF_PACKING_PALETTE,PART_NO,SHIFT,LINE,INVOICE_NUMBER,DATE_OF_SHIPPING,INTERNAL_BARCODE,ORDER_NO,EMPLOYEE_ID,DAYY,WEEK,MONTHH,YEARR,SERIAL_NUMBER,PRODUCT_INFO,DATE_OF_DELETION) SELECT CONTRACT, RELEASE_NO, SEQUENCE_NO, BARCODE_ID, BARCODE, BARCODE_BOX, BOX_NUMBER, DATE_OF_PACKING, BARCODE_PALETTE, PALETTE_NUMBER, DATE_OF_PACKING_PALETTE, PART_NO, SHIFT_ID, LINE_ID, INVOICE_NUMBER, DATE_OF_SHIPPING, INTERNAL_BARCODE, ORDER_NO, EMPLOYEE_ID, DAYY, WEEK, MONTHH, YEARR, SERIAL_NUMBER, PRODUCT_INFO, TO_CHAR(SYSDATE,'DD-MON-YY HH24:MI:SS') FROM FINISHED_GOOD_PS WHERE FINISHED_GOOD_PS.BARCODE_BOX = {p0}");

            if (status<=0)
            {
                return false;
            }

            await _context.SaveChangesAsync();

            var rs = await _context.Database.ExecuteSqlInterpolatedAsync($"UPDATE TRACE.FINISHED_GOOD_PS SET BARCODE_BOX = {p1} WHERE BARCODE_BOX = {p0}");
            if (rs > 0)
            {
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<string> GetFamilyFromPartNo(string part_number)
        {
            var resultString = "";
            var Part_No = new OracleParameter("P_PART_NO", OracleDbType.NVarchar2, 200, part_number, ParameterDirection.Input);
            var output = new OracleParameter("P_RESULT", OracleDbType.NVarchar2, 200,resultString,  ParameterDirection.Output);
         
            using (var context = _context)
            {
                var conn = new OracleConnection(context.Database.GetConnectionString());
                var query = "TRS_MODEL_PROPERTIES_PKG.GET_FAMILY_OF_PARTNO";
                conn.Open();
                if (conn.State == ConnectionState.Open)
                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = query;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(Part_No);
                        command.Parameters.Add(output);
                        command.Connection = conn;
                        OracleDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                          resultString = reader.GetString(0);
                        }
                        reader.Dispose();
                        command.Dispose();
                    }
                conn.Dispose();
            }
            resultString = output.Value.ToString();
            return resultString;
        }

        //public async Task<IEnumerable<RevisionOrder>> GetRevisionByShopOrder(string order_no)
        //{
        //    IEnumerable<object> result = null;
        //    var Order_no = new OracleParameter("P_ORDER_NO", OracleDbType.NVarchar2, 200, order_no, ParameterDirection.Input);
        //    var output = new OracleParameter("P_RESULT", OracleDbType.RefCursor, 200, result, ParameterDirection.Output);
        //    await _context.Database.ExecuteSqlInterpolatedAsync($"BEGIN TRS_CUSTOMER_VERION_PKG.GET_DIFF_V_ORDER_PRC({order_no},{output}); END;");
        //    result = output.Value;
        //    return result;
        //}

        public async Task<IEnumerable<CustomerRevision>> GetRevisionByShopOrder(string orderNo)
        {
            List<CustomerRevision> revisions = new List<CustomerRevision>();
            var orderNoParam = new OracleParameter("P_ORDER_NO", OracleDbType.NVarchar2, 100, orderNo, ParameterDirection.Input);
            var outputParam = new OracleParameter("P_REF_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
            //var res = await _context.Database.ExecuteSqlInterpolatedAsync($"BEGIN TRS_PLANNING_PKG.GET_CV_BY_FAMILY_PRC({familyParam},{Part_No}); END;");
            using (var context = _context)
            {
                var conn = new OracleConnection(context.Database.GetConnectionString());
                var query = "TRS_CUSTOMER_VERION_PKG.GET_DIFF_V_ORDER_PRC";
                conn.Open();
                if (conn.State == ConnectionState.Open)
                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = query;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(orderNoParam);
                        command.Parameters.Add(outputParam);
                        command.Connection = conn;
                        OracleDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var qty = 0;
                            qty = int.TryParse(reader[8].ToString(), out qty)?qty:0;
                            DateTime date = default;
                            if (reader[7] != null)
                                DateTime.TryParse(reader[7].ToString(),out date);
                            revisions.Add(new CustomerRevision("", reader[1].ToString(), reader[0].ToString(), reader[2].ToString(), "", reader[3].ToString(), reader[4].ToString(), int.Parse(reader[5].ToString()), reader[6].ToString(),qty, reader[9].ToString(), date, reader[10].ToString()));
                        }
                        reader.Dispose();
                        command.Dispose();
                    }
                conn.Dispose();
                return revisions;

            }

        }

        public async Task<bool> UpdateRevision(CustomerRevision revision)
        {
            var p0 = new OracleParameter("p0", OracleDbType.Int16, revision.Status, ParameterDirection.Input);
            var p1 = new OracleParameter("p1", OracleDbType.Varchar2, 2000, revision.OrderNo, ParameterDirection.Input);
            var p2 = new OracleParameter("p2", OracleDbType.Varchar2, 2000, revision.PartNo, ParameterDirection.Input);
            var rs = await _context.Database.ExecuteSqlInterpolatedAsync($"UPDATE CUSTOMER_VERSION_MASTER_DATA SET STATUS = {p0}, CONFIRM_DATE=SYSDATE WHERE ORDER_NO = {p1} AND PART_NO={p2}");
            if (rs > 0)
            {
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> UpdateRemarkDB(CustomerRevision revision)
        {
            var p0 = new OracleParameter("p0", OracleDbType.Varchar2,2000, revision.Remark, ParameterDirection.Input);
            var p1 = new OracleParameter("p1", OracleDbType.Varchar2, 2000, revision.OrderNo, ParameterDirection.Input);
            var p2 = new OracleParameter("p2", OracleDbType.Varchar2, 2000, revision.PartNo, ParameterDirection.Input);
            var rs = await _context.Database.ExecuteSqlInterpolatedAsync($"UPDATE CUSTOMER_VERSION_MASTER_DATA SET REMARK = {p0}, CONFIRM_DATE=SYSDATE WHERE ORDER_NO = {p1} AND PART_NO={p2}");
            if (rs > 0)
            {
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<IEnumerable<StockByFamily>> GetStockByFamily(string family)
        {
            List<StockByFamily> stocks = new List<StockByFamily>();
            var orderNoParam = new OracleParameter("P_FAMILY", OracleDbType.NVarchar2, 100, family, ParameterDirection.Input);
            var outputParam = new OracleParameter("P_REF_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
            //var res = await _context.Database.ExecuteSqlInterpolatedAsync($"BEGIN TRS_PLANNING_PKG.GET_CV_BY_FAMILY_PRC({familyParam},{Part_No}); END;");
            using (var context = _context)
            {
                var conn = new OracleConnection(context.Database.GetConnectionString());
                var query = "TRS_CUSTOMER_VERION_PKG.GET_STOCK_BY_FAMILY_PRC";
                conn.Open();
                if (conn.State == ConnectionState.Open)
                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = query;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(orderNoParam);
                        command.Parameters.Add(outputParam);
                        command.Connection = conn;
                        OracleDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {

                            stocks.Add(new StockByFamily(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), int.Parse(reader[3].ToString()), ""));
                        }
                        reader.Dispose();
                        command.Dispose();
                    }
                conn.Dispose();
                return stocks;

            }
        }


        //TRS_CUSTOMER_VERION_PKG.GET_STOCK_BY_FAMILY_PRC(P_FAMILY IN STRING, P_REF_CURSOR OUT SYS_REFCURSOR);

    }
}
