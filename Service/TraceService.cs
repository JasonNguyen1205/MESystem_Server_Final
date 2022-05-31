using DevExpress.Blazor.Internal;
using MESystem.Data.SetupInstruction;
using MESystem.Data.TRACE;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

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

                updateQuery.LineId = (int)dataItem.LineId;
                updateQuery.PlannedStartTime = (DateTime)dataItem.PlannedStartTime;
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
                query = "SELECT COUNT(BARCODE) AS TOTAL_QUANTITIY FROM finished_good_ps WHERE ORDER_NO = '" + orderNo + "' ";
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
                                       .Where(f => f.BarcodeBox == barcodeBox && f.PartNo == partNo)
                                       .AsNoTracking()
                                       .ToListAsync();

            return result.Select(s => new FinishedGood() { OrderNo = s.OrderNo, PartNo = s.PartNo, DateOfPackingBox = s.DateOfPackingBox, QtyBox = result.Count() }).Take(1).ToList();
        }

        public async Task<int>
            GetQtyOfAddedPoNumbers(string poNumber, string partNo)
        {
            return await _context.FinishedGood
                                 //.Where(f => f.InvoiceNumber == poNumber && f.PartNo == partNo)
                                 .Where(fg => fg.InvoiceNumber == poNumber && fg.PartNo == partNo && fg.DateofShipping.Value.Date == DateTime.Today.Date)
                                 .AsNoTracking().CountAsync();
        }

        public async Task<IEnumerable<CustomerOrder>>
            GetCustomerOrders()
        {
            return await _context.CustomerOrders
                                 .Where(f=>f.CustomerPoNo==f.CustomerPoNo)
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<bool> 
            InsertPurchaseOrderNo(string barcodeBox, string poNumber)
        {
            //var updateQuery = _context.FinishedGoods
            //                           .SingleOrDefault(c => c.BarcodeBox == barcodeBox);
            var updateQuery = _context.FinishedGood
                                       .Where(c => c.BarcodeBox == barcodeBox).ToList();

            if (updateQuery != null)
            {
                //updateQuery.InvoiceNumber = poNumber;
                //updateQuery.DateofShipping = DateTime.Now;
                //updateQuery.ForEach(f =>
                //{
                //    f.InvoiceNumber = poNumber;
                //    f.DateofShipping = DateTime.Now;
                //});
                //foreach (var f in updateQuery)
                //{
                //    f.InvoiceNumber = poNumber;
                //    f.DateofShipping = DateTime.Now;
                //}


                //foreach (var invoice in _context.FinishedGoods.Where(c => c.BarcodeBox == barcodeBox))
                //{
                //    invoice.InvoiceNumber = poNumber;
                //}
                //_context.SaveChanges();

                //_context.FinishedGoods
                //        .Update(updateQuery);
                //await _context.SaveChangesAsync();

                if(await _context.Database.ExecuteSqlRawAsync("UPDATE TRACE.FINISHED_GOOD_PS SET INVOICE_NUMBER = '" + poNumber + "', DATE_OF_SHIPPING = sysdate WHERE BARCODE_BOX = '" + barcodeBox + "' ") > 0)
                {
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
    

        public async Task<IEnumerable<FinishedGood>?>
           CheckExistBarcodeBox(string barcodeBox)
        {
            var query = await _context.FinishedGood
                                 .Where(f => f.BarcodeBox == barcodeBox && f.BarcodePalette == null)
                                 .AsNoTracking().ToListAsync();
            return query;
        }

        public async Task<int> GetMaxPaletteNumber(string part_no)
        {
            var Flag = new OracleParameter("P_FLAG", OracleDbType.Decimal, 100, 1, ParameterDirection.Input);
            var Part_No = new OracleParameter("P_PART_NO", OracleDbType.NVarchar2, 200, part_no, ParameterDirection.Input);
            var output = new OracleParameter("P_MAX_BOX_OR_PALETTE", OracleDbType.Decimal, 100, ParameterDirection.Output);
            var res = await _context.Database.ExecuteSqlInterpolatedAsync($"BEGIN TRS_FINISHED_GOOD_PS_PKG.GET_MAX_BOX_OR_PALETTE_PRC({Flag},{Part_No},{output}); END;", default);
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

            var res = await _context.Database.ExecuteSqlInterpolatedAsync($"BEGIN TRS_FINISHED_GOOD_PS_PKG.UPDATE_FINISHED_GOOD_PRC({Flag},{Barcode_Box},{Barcode_Palette},{Palette_number},{Dates}); END;", default);
            await _context.SaveChangesAsync();
            //}
        }

        public async Task<IEnumerable<ModelProperties>>
           GetFamily(string FamilyInput, string partNoInput)
        {
            return await _context.ModelProperties
                                 .Where(j => j.Routing_station == FamilyInput && j.PartNo == partNoInput)
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<string> GetCustomerVersion(int flag, string po_number)
        {
            string max_number = "";
            var Flag = new OracleParameter("P_FLAG", OracleDbType.Decimal, 100, flag, ParameterDirection.Input);
            var Part_No = new OracleParameter("P_PART_NO", OracleDbType.NVarchar2, 200, po_number, ParameterDirection.Input);
            var output = new OracleParameter("P_OUT",OracleDbType.NVarchar2, 200,max_number,ParameterDirection.Output);
            var res = await _context.Database.ExecuteSqlInterpolatedAsync($"BEGIN TRS_PLANNING_PKG.GET_CUSTOMER_VERSION_PRC({Flag},{Part_No},{output}); END;");
            
            Console.WriteLine(output.Value);
            max_number = output.Value.ToString();
            return max_number;
        }

        public async Task<string> GetQTYperBox(int flag, string part_number)
        {
            var Flag = new OracleParameter("P_FLAG", OracleDbType.Decimal, 100, flag, ParameterDirection.Input);
            var Part_No = new OracleParameter("P_PART_NO", OracleDbType.NVarchar2, 200, part_number, ParameterDirection.Input);
            var output = new OracleParameter("P_RESULT", OracleDbType.Decimal, 100, ParameterDirection.Output);
            var res = await _context.Database.ExecuteSqlInterpolatedAsync($"BEGIN TRS_MODEL_PROPERTIES_PKG.GET_MODEL_PRC({Flag},{Part_No},{output}); END;", default);
            string QTY_Box = "";
            Console.WriteLine(output.Value);
            QTY_Box = output.Value.ToString();
            return QTY_Box;
        }
    }
}
