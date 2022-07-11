using System.Data;
using System.IO;

using DevExpress.Blazor.Internal;

using MESystem.Data.TRACE;

using Microsoft.EntityFrameworkCore;

using OfficeOpenXml;

using Oracle.ManagedDataAccess.Client;

namespace MESystem.Service;

public class TraceService
{
    private readonly TraceDbContext _context;
    public TraceService(TraceDbContext context)
    {
        _context=context;
    }

    public async Task<IEnumerable<ProductionLine>>
        LoadProductionLines(string departmentFilter)
    {
        if(departmentFilter.Equals("all"))
        {
            return await _context.ProductionLines
                            .AsNoTracking()
                            .ToListAsync();
        }
        else
        {
            return await _context.ProductionLines
                            .Where(s => s.DepartmentNo==departmentFilter)
                            .AsNoTracking()
                            .ToListAsync();
        }
    }

    public async Task<IEnumerable<vProductionPlan>>
        GetProductionPlan(string department, string workcenterFilter)
    {
        var stringsToSearchFor = workcenterFilter.Split(",");
        if(!stringsToSearchFor.Contains("T&J"))
        {
            return await _context.ProductionPlan
                                 .Where(s => s.DepartmentNo==department&&stringsToSearchFor.Contains(s.WorkCenterNo))
                                 .OrderBy(o => o.LineDescription).ThenBy(o => o.PlannedStartTime).ThenBy(o => o.ProductionOrder)
                                 .AsNoTracking()
                                 .ToListAsync();
        }
        else
        {
            var departmentFilter = department.Split(",");
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
        var stringsToSearchFor = workcenterFilter.Split(",");
        if(!stringsToSearchFor.Contains("T&J"))
        {
            return await _context.ProductionPlanJigs
                                 //.Where(s => s.DepartmentNo == department && stringsToSearchFor.Contains(s.WorkCenterNo))
                                 .Where(s => s.DepartmentNo==department)
                                 .OrderBy(o => o.LineDescription).ThenBy(o => o.PlannedStartTime).ThenBy(o => o.PlannedStartTime)
                                 .AsNoTracking()
                                 .ToListAsync();
        }
        else
        {
            var departmentFilter = department.Split(",");
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

        List<vProductionPlanJigs>? result = await _context.ProductionPlanJigs
                                   .Where(s => s.OrderNo==orderNo)
                                   .AsNoTracking()
                                   .ToListAsync();

        if(result.Count()!=0)
        {
            editFormData=new ProductionPlanJigs()
            {
                OrderNo=result.FirstOrDefault().OrderNo,
                PartNo=result.FirstOrDefault().PartNo,
                PartDescription=result.FirstOrDefault().PartDescription,
                LineDescription=result.FirstOrDefault().LineDescription,
                Location=result.FirstOrDefault().Location
            };
        }
        else
        {
            editFormData=new ProductionPlanJigs()
            {
                OrderNo=string.Empty,
                PartNo="NO DATA",
                PartDescription=string.Empty,
                LineDescription=string.Empty,
                Location=string.Empty
            };
        }

        return editFormData;
    }

    public async Task<IEnumerable<vProductionPlanIFS>>
        GetShopOrderStatus(string department)
    {
        if(department.Equals(string.Empty))
        {
            return await _context.ProductionPlanIfs
                                 .Where(pp => pp.PercentDone>=90)
                                 .Distinct()
                                 .OrderBy(pp => pp.DepartmentNo).ThenByDescending(pp => pp.QtyNotBooked).ThenByDescending(pp => pp.PercentDone)
                                 .AsNoTracking()
                                 .ToListAsync();
        }
        else
        {
            return await _context.ProductionPlanIfs
                                 .Where(pp => pp.PercentDone>=90&&pp.DepartmentNo==department)
                                 .Distinct()
                                 .OrderBy(pp => pp.DepartmentNo).ThenByDescending(pp => pp.QtyNotBooked).ThenByDescending(pp => pp.PercentDone)
                                 .AsNoTracking()
                                 .ToListAsync();
        }
    }

    public async Task<IEnumerable<ProductionPlanLine>>
        GetProductionPlanLinesAsync(string departmentFilter)
    {
        return await _context.ProductionPlanLines
                             .Where(s => s.DepartmentNo==departmentFilter)
                             .AsNoTracking()
                             .ToListAsync();
    }

    public async Task<ProductionPlanLine>
        UpdateProductionValues(ProductionPlanLine dataItem, bool newInsert)
    {
        if(newInsert)
        {
            _=_context.ProductionPlanLines
                    .Add(dataItem);
            _=await _context.SaveChangesAsync();
        }
        else
        {
            ProductionPlanLine? updateQuery = await _context.ProductionPlanLines
            .FirstAsync(c => c.DepartmentNo==dataItem.DepartmentNo&&c.OrderNo==dataItem.OrderNo&&c.OperationNo==dataItem.OperationNo);

            updateQuery.LineId=dataItem.LineId;
            updateQuery.PlannedStartTime=dataItem.PlannedStartTime;
            updateQuery.ProductionOrder=dataItem.ProductionOrder;
            updateQuery.ModificationTime=DateTime.Now;

            _=_context.ProductionPlanLines
                    .Update(updateQuery);
            _=await _context.SaveChangesAsync();
        }

        return null;
    }

    ProductionPlanLine[] RemoveInternal(ProductionPlanLine dataItem)
    {
        _=_context.ProductionPlanLines.Attach(dataItem);
        _=_context.ProductionPlanLines.Remove(dataItem);
        _=_context.SaveChanges();
        return _context.ProductionPlanLines.ToArray();
    }

    public Task<ProductionPlanLine[]> Remove(ProductionPlanLine dataItem)
    {
        return Task.FromResult(RemoveInternal(dataItem));
    }

    public int CountTotalQty(int? prepotting, string orderNo)
    {
        string query;
        if(prepotting!=null)
        {
            query="SELECT COUNT(DISTINCT BARCODE) AS TOTAL_QUANTITIY FROM data_ate_pre_potting WHERE ORDER_NO = '"+orderNo+"' ";
        }
        else
        {
            query="SELECT COUNT(BARCODE) AS TOTAL_QUANTITIY FROM TRACE.finished_good_ps WHERE ORDER_NO = '"+orderNo+"' ";
        }

        IQueryable<TotalQuantitys>? result = _context.TotalQuantity
                             .FromSqlRaw(query);

        return result.FirstOrDefault().TOTAL_QUANTITIY;
    }

    public Task<vShopOrderStates[]> GetShopOrderState(string filter)
    {
        if(filter==string.Empty)
        {
            return _context.ShopOrderState
                           .AsNoTracking()
                           .ToArrayAsync();
        }
        else
        {
            return _context.ShopOrderState
                           .Where(s => s.Department==filter)
                           .AsNoTracking()
                           .ToArrayAsync();
        }
    }

    public Task<vShopOrderStateMI[]> GetShopOrderStateMI(string filter)
    {
        if(filter==string.Empty)
        {
            return _context.ShopOrderStateMI
            .AsNoTracking()
            .ToArrayAsync();
        }
        else
        {
            return _context.ShopOrderStateMI
            .Where(s => s.Department==filter)
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
                             .Where(o => o.PartNo==partNo)
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
                             .Where(w => w.PartNo==partNo&&w.Family==family)
                             .OrderBy(o => o.PartNo).ThenBy(o => o.Id)
                             .AsNoTracking()
                             .ToListAsync();
    }

    public async Task UpdateTargetPph(string productionLineId, int newTargetPPH)
    {
        ProductionPlanLine? updateQuery = await _context.ProductionPlanLines.FirstOrDefaultAsync(c => c.Id==productionLineId);

        if(updateQuery!=null)
        {
            updateQuery.TargetPPH=newTargetPPH;
            _=_context.ProductionPlanLines
                    .Update(updateQuery);
            _=await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateBasePph(SiPartsPerHour newValues)
    {
        SiPartsPerHour? updateQuery = await _context.SiPartPerhour.FirstOrDefaultAsync(c => c.partNo==newValues.partNo&&c.workCenterNo==newValues.workCenterNo);

        if(updateQuery!=null)
        {
            _=_context.SiPartPerhour
                    .Remove(updateQuery);
            _=await _context.SaveChangesAsync();
        }
        _=await _context.SiPartPerhour.AddAsync(newValues);
        _=await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<vFinishedGoods>>
        GetOutputByOrderNoDay(string orderNo, string departmentNo)
    {
        IEnumerable<vFinishedGoods> result;

        if(departmentNo.Equals("V3100"))
        {
            result=await _context.vFinishedGoods
                                   .Where(f => f.OrderNo==orderNo)
                                   .GroupBy(r => new { r.OrderNo, r.PartNo, r.Day, r.Month, r.Year, r.PanelQty })
                                   .Select(g => new vFinishedGoods() { OrderNo=g.Key.OrderNo, PartNo=g.Key.PartNo, Day=g.Key.Day, Month=g.Key.Month, Year=g.Key.Year, QtyCounted=g.Count()*g.Key.PanelQty })
                                   .AsNoTracking()
                                   .ToListAsync();
        }
        else if(departmentNo.Equals("T3500"))
        {
            result=await _context.vFinishedGoods
                                    .Where(f => f.OrderNo==orderNo)
                                    .GroupBy(r => new { r.OrderNo, r.PartNo, r.Day, r.Month, r.Year })
                                    .Select(g => new vFinishedGoods() { OrderNo=g.Key.OrderNo, PartNo=g.Key.PartNo, Day=g.Key.Day, Month=g.Key.Month, Year=g.Key.Year, QtyCounted=g.Sum(s => s.BoxNumber) })
                                    .AsNoTracking()
                                    .ToListAsync();
        }
        else
        {
            result=await _context.vFinishedGoods
                                    .Where(f => f.OrderNo==orderNo)
                                    .GroupBy(r => new { r.OrderNo, r.PartNo, r.Day, r.Month, r.Year })
                                    .Select(g => new vFinishedGoods() { OrderNo=g.Key.OrderNo, PartNo=g.Key.PartNo, Day=g.Key.Day, Month=g.Key.Month, Year=g.Key.Year, QtyCounted=g.Count() })
                                    .AsNoTracking()
                                    .ToListAsync();
        }
        return result;
    }

    public async Task<IEnumerable<FinishedGood>>
        GetBoxContentInformation(string barcodeBox, string partNo)
    {
        List<FinishedGood>? result = await _context.FinishedGood
                                   .Where(_ => _.BarcodeBox==barcodeBox&&_.PartNo==partNo)
                                   .AsNoTracking()
                                   .ToListAsync();

        return result.Select(s => new FinishedGood() { Barcode=s.Barcode, OrderNo=s.OrderNo, PartNo=s.PartNo, BarcodeBox=s.BarcodeBox, DateOfPackingBox=s.DateOfPackingBox, QtyBox=result.Count(), InvoiceNumber=s.InvoiceNumber, Rev=result.FirstOrDefault().Barcode.Substring(7, 2) }).ToList().AsEnumerable();
    }

    public async Task<IEnumerable<FinishedGood>>
      GetPalletContentInformation(string barcodePallet)
    {
        List<FinishedGood>? result = await _context.FinishedGood
                                   .Where(_ => _.BarcodePalette==barcodePallet)
                                   .AsNoTracking()
                                   .ToListAsync();

        return result.Select(s => new FinishedGood() { OrderNo=s.OrderNo, PartNo=s.PartNo, BarcodeBox=s.BarcodeBox, DateOfPackingBox=s.DateOfPackingBox, QtyPallet=result.Count(), InvoiceNumber=s.InvoiceNumber, Rev=result.FirstOrDefault().Barcode.Substring(7, 2) }).Take(1).ToList().AsEnumerable();
    }

    public async Task<IEnumerable<FinishedGood>>
        GetQtyOfAddedPoNumbers(string poNumber, string partNo, string shipmentId)
    {
        shipmentId=shipmentId??"";
        OracleParameter? p0 = new("p0", OracleDbType.Varchar2, 2000, poNumber, ParameterDirection.Input);
        OracleParameter? p1 = new("p1", OracleDbType.Varchar2, 2000, partNo, ParameterDirection.Input);
        OracleParameter? p2 = new("p2", OracleDbType.Varchar2, 2000, shipmentId, ParameterDirection.Input);
        List<FinishedGood>? rs;

        if(!string.IsNullOrEmpty(shipmentId))
        {
            rs = await _context.FinishedGood.FromSqlInterpolated($"select * from TRACE.FINISHED_GOOD_PS where invoice_number = {p0} and part_no = {p1} and shipment_id = {p2}").ToListAsync();
        }
        else
        {
            rs = await _context.FinishedGood.FromSqlInterpolated($"select * from TRACE.FINISHED_GOOD_PS where invoice_number = {p0} and part_no = {p1} and shipment_id is null").ToListAsync();
        }
        
        return rs.AsEnumerable();
    }

    public async Task<IEnumerable<CustomerOrder>>
        GetCustomerOrders()
    {


        List<CustomerOrder>? rs = await _context.CustomerOrders.ToListAsync();
        //.Where(f => f.QtyInvoiced > 0)
        //(f=>f.CustomerPoNo)

        return rs;
    }

    public async Task<bool>
        InsertPurchaseOrderNo(string barcodeBox, string poNumber, string shipmentId)
    {
        Task<List<FinishedGood>>? updateQuery = _context.FinishedGood
                                   .Where(c => c.BarcodeBox==barcodeBox).ToListAsync();

        if(updateQuery!=null)
        {
            OracleParameter? p0 = new("p0", OracleDbType.Varchar2, 2000, poNumber, ParameterDirection.Input);
            OracleParameter? p1 = new("p1", OracleDbType.Varchar2, 2000, barcodeBox, ParameterDirection.Input);
            OracleParameter? p2 = new("p2", OracleDbType.Varchar2, 2000, shipmentId, ParameterDirection.Input);
            var rs = await _context.Database.ExecuteSqlInterpolatedAsync($"UPDATE TRACE.FINISHED_GOOD_PS SET INVOICE_NUMBER = {p0}, DATE_OF_SHIPPING = SYSDATE, SHIPMENT_ID = {p2} WHERE BARCODE_BOX = {p1}");
            if(rs>0)
            {
                _=await _context.SaveChangesAsync();
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
                             .Where(w => w.Barcode==barcode&&w.Id==28734334)
                             .AsNoTracking()
                             .ToListAsync();
    }

    public async Task UpdateManufTool(SiManufacturingTool newValues)
    {
        SiManufacturingTool? updateQuery = await _context.SiManufacturingTools.FirstOrDefaultAsync(mft => mft.Id==newValues.Id);

        if(updateQuery!=null)
        {
            _=_context.SiManufacturingTools
                    .Remove(updateQuery);
            _=await _context.SaveChangesAsync();
        }
        _=await _context.SiManufacturingTools.AddAsync(newValues);
        _=await _context.SaveChangesAsync();
    }

    public async Task RemoveManufTool(SiManufacturingTool newValues)
    {
        SiManufacturingTool? updateQuery = await _context.SiManufacturingTools.FirstOrDefaultAsync(mft => mft.Id==newValues.Id);

        if(updateQuery!=null)
        {
            _=_context.SiManufacturingTools
                    .Remove(updateQuery);
            _=await _context.SaveChangesAsync();
        }
    }

    public async Task UpdatefToolType(SiManufacturingToolType newValues)
    {
        SiManufacturingToolType? updateQuery = await _context.SiManufacturingToolTypes.FirstOrDefaultAsync(mft => mft.Id==newValues.Id);

        if(updateQuery!=null)
        {
            _=_context.SiManufacturingToolTypes
                    .Remove(updateQuery);
            _=await _context.SaveChangesAsync();
        }
        _=await _context.SiManufacturingToolTypes.AddAsync(newValues);
        _=await _context.SaveChangesAsync();
    }

    public async Task UpdateManufToolLink(SiManufacturingToolLink newValues)
    {
        SiManufacturingToolLink? updateQuery = await _context.SiManufacturingToolLinks.FirstOrDefaultAsync(mftl => mftl.Id==newValues.Id);

        if(updateQuery!=null)
        {
            _=_context.SiManufacturingToolLinks
                    .Remove(updateQuery);
            _=await _context.SaveChangesAsync();
        }
        _=await _context.SiManufacturingToolLinks.AddAsync(newValues);
        _=await _context.SaveChangesAsync();
    }

    public async Task RemoveManufToolLink(vSiManufacturingToolPart newValues)
    {
        SiManufacturingToolLink? updateQuery = await _context.SiManufacturingToolLinks.FirstOrDefaultAsync(mftl => mftl.Id==newValues.Id);

        if(updateQuery!=null)
        {
            _=_context.SiManufacturingToolLinks
                    .Remove(updateQuery);
            _=await _context.SaveChangesAsync();
        }
    }

    // Scan Pallete 

    //Check Box exist
    public async Task<IEnumerable<FinishedGood>?>
       CheckExistBarcodeBox(string barcodeBox, string pONo)
    {
        List<FinishedGood>? query = await _context.FinishedGood
                             .Where(f => f.BarcodeBox==barcodeBox&&f.InvoiceNumber==pONo).ToListAsync();
        return query.AsEnumerable();
    }

    //Check box is linked to pallete
    public async Task<IEnumerable<FinishedGood>?>
      CheckBoxInAnyPallete(string barcodeBox)
    {
        List<FinishedGood>? query = await _context.FinishedGood
                             .Where(f => f.BarcodeBox==barcodeBox&&f.BarcodePalette!=null).ToListAsync();
        if(query==null)
        {
            return null;
        }

        return query.AsEnumerable();
    }

    //Check box is linked to PO
    public async Task<IEnumerable<FinishedGood>?>
     CheckBoxLinkedToPO(string barcodeBox)
    {
        List<FinishedGood>? query = await _context.FinishedGood
                             .Where(f => f.BarcodeBox==barcodeBox&&f.InvoiceNumber==null).ToListAsync();
        return query.AsEnumerable();
    }

    public async Task<int> GetMaxPaletteNumber(string part_no)
    {
        OracleParameter? Flag = new("P_FLAG", OracleDbType.Decimal, 100, 1, ParameterDirection.Input);
        OracleParameter? Part_No = new("P_PART_NO", OracleDbType.NVarchar2, 200, part_no, ParameterDirection.Input);
        OracleParameter? output = new("P_MAX_BOX_OR_PALETTE", OracleDbType.Decimal, 100, ParameterDirection.Output);
        _=await _context.Database.ExecuteSqlInterpolatedAsync($"BEGIN TRACE.TRS_FINISHED_GOOD_PS_PKG.GET_MAX_BOX_OR_PALETTE_PRC({Flag},{Part_No},{output}); END;", default);
        Console.WriteLine(output.Value);
        var max_number = int.Parse(output.Value.ToString());
        return max_number;
    }

    public async Task UpdateFinishedGood(string barcode_box, string barcode_palette, int palette_number)
    {
        _=new OracleParameter[0];
        OracleParameter? Flag = new("P_FLAG", OracleDbType.Decimal, 100, 1, ParameterDirection.Input);
        OracleParameter? Barcode_Box = new("P_INPUT", OracleDbType.NVarchar2, barcode_box, ParameterDirection.Input);
        OracleParameter? Barcode_Palette = new("P_BARCODE_PALETTE", OracleDbType.NVarchar2, barcode_palette, ParameterDirection.Input);
        OracleParameter? Palette_number = new("P_PALETTE_NUMBER", OracleDbType.Decimal, palette_number, ParameterDirection.Input);
        OracleParameter? Dates = new("P_DATE_OF_PACKING_PALETTE", OracleDbType.Date, DateTime.Now, ParameterDirection.Input);
        _=await _context.Database.ExecuteSqlInterpolatedAsync($"BEGIN TRACE.TRS_FINISHED_GOOD_PS_PKG.UPDATE_FINISHED_GOOD_PRC({Flag},{Barcode_Box},{Barcode_Palette},{Palette_number},{Dates}); END;", default);
        _=await _context.SaveChangesAsync();
        //}
    }


    public async Task<bool> VerifyPallet(string barcode_palette,
        int state, string shipmentID)
    {
        OracleParameter? p10 = new("p10", OracleDbType.Int32, 2000, state, ParameterDirection.Input);
        OracleParameter? p11 = new("p11", OracleDbType.Varchar2, 2000, barcode_palette, ParameterDirection.Input);
        OracleParameter? p12 = new("p12", OracleDbType.Varchar2, 2000, shipmentID, ParameterDirection.Input);
        var rs = await _context.Database.ExecuteSqlInterpolatedAsync($"UPDATE TRACE.FINISHED_GOOD_PS SET VERIFIED_PALLET = {p10}, SHIPMENT_ID = {p12} WHERE BARCODE_PALETTE = {p11}");

        if(rs>0)
        {
            _=await _context.SaveChangesAsync();
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> VerifyBoxPallet(string barcode_palette,
        int state, string shipmentID, string barcodeBox)
    {
        var rsCheck = false;
        OracleParameter? p1 = new("p1", OracleDbType.Varchar2, 2000, barcode_palette, ParameterDirection.Input);
        OracleParameter? p2 = new("p2", OracleDbType.Varchar2, 2000, shipmentID, ParameterDirection.Input);
        OracleParameter? p3 = new("p3", OracleDbType.Varchar2, 2000, barcodeBox, ParameterDirection.Input);
        OracleParameter? p4 = new("p4", OracleDbType.RefCursor, ParameterDirection.Output);
        //p4.Value = rsCheck;
        await using TraceDbContext? context = _context;
        OracleConnection? conn = new(context.Database.GetConnectionString());

        var query = "TRACE.TRS_PACKING_MASTER_LIST_PKG.VERIFIED_PALLET_PRC";
        conn.Open();
        if(conn.State==ConnectionState.Open)
        {
            OracleCommand? command = conn.CreateCommand();
            command.CommandText=query;
            command.CommandType=CommandType.StoredProcedure;
            _=command.Parameters.Add(p2);
            _=command.Parameters.Add(p1);
            _=command.Parameters.Add(p3);
            _=command.Parameters.Add(p4);
            command.Connection=conn;
            OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync();
            rsCheck=reader.RowSize>0;

            command.Parameters.Clear();
            reader.Dispose();
            command.Dispose();
        }

        conn.Dispose();
        return rsCheck;
    }

    public async Task<IEnumerable<ModelProperties>>
       GetFamily(string FamilyInput, string partNoInput)
    {
        return await _context.ModelProperties
                             .Where(j => j.Routing_station==FamilyInput&&j.PartNo==partNoInput)
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
        OracleParameter? flagParam = new("P_FLAG", OracleDbType.Decimal, 100, flag, ParameterDirection.Input);
        OracleParameter? partNo = new("P_PART_NO", OracleDbType.NVarchar2, 200, part_number, ParameterDirection.Input);
        OracleParameter? output = new("P_RESULT", OracleDbType.Decimal, 100, ParameterDirection.Output);
        _=await _context.Database.ExecuteSqlInterpolatedAsync($"BEGIN TRACE.TRS_MODEL_PROPERTIES_PKG.GET_MODEL_PRC({flagParam},{partNo},{output}); END;");
        //if (res < 1) return 0;
        var rs = 0;
        if(output.Value!=null)
        {
            rs=int.Parse(output.Value.ToString()??throw new InvalidOperationException());
        }

        return rs;
    }


    public async Task<IEnumerable<CustomerRevision>> GetCustomerRevisionByPartNo(string partNo, string family)
    {
        List<CustomerRevision> revisions = new();
        OracleParameter? partNoParam = new("P_PART_NO", OracleDbType.NVarchar2, 100, partNo, ParameterDirection.Input);
        OracleParameter? familyParam = new("P_FAMILY", OracleDbType.NVarchar2, 100, family, ParameterDirection.Input);
        OracleParameter? outputParam = new("P_REF_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
        await using TraceDbContext? context = _context;
        OracleConnection? conn = new(context.Database.GetConnectionString());
        var query = "TRACE.TRS_CUSTOMER_VERION_PKG.GET_STOCK_BY_VER_PRC";
        conn.Open();
        if(conn.State==ConnectionState.Open)
        {
            await using OracleCommand? command = conn.CreateCommand();
            command.CommandText=query;
            command.CommandType=CommandType.StoredProcedure;
            _=command.Parameters.Add(partNoParam);
            _=command.Parameters.Add(familyParam);
            _=command.Parameters.Add(outputParam);
            command.Connection=conn;
            OracleDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                var qty = 0;
                if(!int.TryParse(reader[1].ToString(), out qty))
                {
                    qty=0;
                }

                revisions.Add(new CustomerRevision(reader[0].ToString(), qty));
            }

            command.Parameters.Clear();
            reader.Dispose();
            command.Dispose();
        }

        conn.Dispose();
        return revisions.AsEnumerable();
    }

    public async Task<IEnumerable<CustomerRevision>> GetCustomerRevision(int flag, string poNo, string family, string partNo, string orderNo)
    {
        List<CustomerRevision> revisions = new();
        OracleParameter? flagParam = new("P_FLAG", OracleDbType.Decimal, 8, flag, ParameterDirection.Input);
        OracleParameter? poNoParam = new("P_CO_NO", OracleDbType.NVarchar2, 100, poNo, ParameterDirection.Input);
        OracleParameter? familyParam = new("P_FAMILY", OracleDbType.NVarchar2, 100, family, ParameterDirection.Input);
        OracleParameter? partNoParam = new("P_PART_NO", OracleDbType.NVarchar2, 100, partNo, ParameterDirection.Input);
        OracleParameter? orderNoParam = new("P_ORDER_NO", OracleDbType.NVarchar2, 100, orderNo, ParameterDirection.Input);
        OracleParameter? outputParam = new("P_REF_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
        //var res = await _context.Database.ExecuteSqlInterpolatedAsync($"BEGIN TRS_PLANNING_PKG.GET_CV_BY_FAMILY_PRC({familyParam},{Part_No}); END;");
        await using TraceDbContext? context = _context;
        OracleConnection? conn = new(context.Database.GetConnectionString());
        var query = "TRS_PLANNING_PKG.GET_CV_BY_FAMILY_PRC";
        conn.Open();
        if(conn.State==ConnectionState.Open)
        {
            await using OracleCommand? command = conn.CreateCommand();
            command.CommandText=query;
            command.CommandType=CommandType.StoredProcedure;
            _=command.Parameters.Add(flagParam);
            _=command.Parameters.Add(poNoParam);
            _=command.Parameters.Add(familyParam);
            _=command.Parameters.Add(partNoParam);
            _=command.Parameters.Add(orderNoParam);
            _=command.Parameters.Add(outputParam);
            command.Connection=conn;
            OracleDataReader reader = command.ExecuteReader();
            while(reader.Read())
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
        List<CustomerRevision> revisions = new();
        OracleParameter? flagParam = new("P_FLAG", OracleDbType.Int16, 3
            , ParameterDirection.Input);
        OracleParameter? poNoParam = new("P_CO_NO", OracleDbType.NVarchar2, 100, string.Empty, ParameterDirection.Input);
        OracleParameter? familyParam = new("P_FAMILY", OracleDbType.NVarchar2, 100, string.Empty, ParameterDirection.Input); ;
        OracleParameter? partNoParam = new("P_PART_NO", OracleDbType.NVarchar2, 100, partNo, ParameterDirection.Input);
        OracleParameter? orderNoParam = new("P_ORDER_NO", OracleDbType.NVarchar2, 100, string.Empty, ParameterDirection.Input);
        OracleParameter? outputParam = new("P_REF_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
        await using TraceDbContext? context = _context;
        OracleConnection? conn = new(context.Database.GetConnectionString());
        var query = "TRS_PLANNING_PKG.GET_CV_BY_FAMILY_PRC";
        conn.Open();
        if(conn.State==ConnectionState.Open)
        {
            await using OracleCommand? command = conn.CreateCommand();
            command.CommandText=query;
            command.CommandType=CommandType.StoredProcedure;
            _=command.Parameters.Add(flagParam);
            _=command.Parameters.Add(poNoParam);
            _=command.Parameters.Add(familyParam);
            _=command.Parameters.Add(partNoParam);
            _=command.Parameters.Add(orderNoParam);
            _=command.Parameters.Add(outputParam);
            command.Connection=conn;
            OracleDataReader reader = command.ExecuteReader();
            while(reader.Read())
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
        List<FinishedGood>? query = await _context.FinishedGood
                             .Where(f => f.BarcodeBox==barcodeBox&&f.BarcodePalette==null).ToListAsync();
        return query.AsEnumerable();
    }

    //Check PartNos of BarcodeBox exist
    public async Task<IEnumerable<FinishedGood>?>
       CheckPartNoBarcodeBox(string barcodeBox, string partNo)
    {
        List<FinishedGood>? query = await _context.FinishedGood
                             .Where(_ => _.BarcodeBox==barcodeBox&&_.PartNo==partNo).ToListAsync();
        return query.AsEnumerable();
    }

    public async Task<bool> UpdateBarcodeBox(string barcode_box, string barcode_box2)
    {
        OracleParameter? p0 = new("p0", OracleDbType.Varchar2, 2000, barcode_box, ParameterDirection.Input);
        OracleParameter? p1 = new("p1", OracleDbType.Varchar2, 2000, barcode_box2, ParameterDirection.Input);

        var status = await _context.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO TRACE.FINISHED_GOOD_PS_HISTORY(CONTRACT,RELEASE_NO,SEQUENCE_NO,BARCODE_ID,BARCODE,BARCODE_BOX,BOX_NUMBER,DATE_OF_PACKING,BARCODE_PALETTE,PALETTE_NUMBER,DATE_OF_PACKING_PALETTE,PART_NO,SHIFT,LINE,INVOICE_NUMBER,DATE_OF_SHIPPING,INTERNAL_BARCODE,ORDER_NO,EMPLOYEE_ID,DAYY,WEEK,MONTHH,YEARR,SERIAL_NUMBER,PRODUCT_INFO,DATE_OF_DELETION) SELECT CONTRACT, RELEASE_NO, SEQUENCE_NO, BARCODE_ID, BARCODE, BARCODE_BOX, BOX_NUMBER, DATE_OF_PACKING, BARCODE_PALETTE, PALETTE_NUMBER, DATE_OF_PACKING_PALETTE, PART_NO, SHIFT_ID, LINE_ID, INVOICE_NUMBER, DATE_OF_SHIPPING, INTERNAL_BARCODE, ORDER_NO, EMPLOYEE_ID, DAYY, WEEK, MONTHH, YEARR, SERIAL_NUMBER, PRODUCT_INFO, TO_CHAR(SYSDATE,'DD-MON-YY HH24:MI:SS') FROM FINISHED_GOOD_PS WHERE FINISHED_GOOD_PS.BARCODE_BOX = {p0}");

        if(status<=0)
        {
            return false;
        }

        _=await _context.SaveChangesAsync();

        var rs = await _context.Database.ExecuteSqlInterpolatedAsync($"UPDATE TRACE.FINISHED_GOOD_PS SET BARCODE_BOX = {p1} WHERE BARCODE_BOX = {p0}");
        if(rs>0)
        {
            _=await _context.SaveChangesAsync();
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<string> GetFamilyFromPartNo(string part_number)
    {
        var resultString = string.Empty;
        OracleParameter? Part_No = new("P_PART_NO", OracleDbType.NVarchar2, 200, part_number, ParameterDirection.Input);
        OracleParameter? output = new("P_RESULT", OracleDbType.NVarchar2, 200, resultString, ParameterDirection.Output);

        using(TraceDbContext? context = _context)
        {
            OracleConnection? conn = new(context.Database.GetConnectionString());
            var query = "TRS_MODEL_PROPERTIES_PKG.GET_FAMILY_OF_PARTNO";
            conn.Open();
            if(conn.State==ConnectionState.Open)
            {
                using OracleCommand? command = conn.CreateCommand();
                command.CommandText=query;
                command.CommandType=CommandType.StoredProcedure;
                _=command.Parameters.Add(Part_No);
                _=command.Parameters.Add(output);
                command.Connection=conn;
                OracleDataReader reader = command.ExecuteReader();
                while(reader.Read())
                {
                    resultString=reader.GetString(0);
                }
                reader.Dispose();
                command.Dispose();
            }

            conn.Dispose();
        }
        resultString=output.Value.ToString();
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
        List<CustomerRevision> revisions = new();
        OracleParameter? orderNoParam = new("P_ORDER_NO", OracleDbType.NVarchar2, 100, orderNo, ParameterDirection.Input);
        OracleParameter? outputParam = new("P_REF_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
        //var res = await _context.Database.ExecuteSqlInterpolatedAsync($"BEGIN TRS_PLANNING_PKG.GET_CV_BY_FAMILY_PRC({familyParam},{Part_No}); END;");
        using TraceDbContext? context = _context;
        OracleConnection? conn = new(context.Database.GetConnectionString());
        var query = "TRS_CUSTOMER_VERION_PKG.GET_DIFF_V_ORDER_PRC";
        conn.Open();
        if(conn.State==ConnectionState.Open)
        {
            using OracleCommand? command = conn.CreateCommand();
            command.CommandText=query;
            command.CommandType=CommandType.StoredProcedure;
            _=command.Parameters.Add(orderNoParam);
            _=command.Parameters.Add(outputParam);
            command.Connection=conn;
            OracleDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                var qty = 0;
                qty=int.TryParse(reader[8].ToString(), out qty) ? qty : 0;
                DateTime date = default;
                if(reader[7]!=null)
                {
                    _=DateTime.TryParse(reader[7].ToString(), out date);
                }

                revisions.Add(new CustomerRevision(string.Empty, reader[1].ToString(), reader[0].ToString(), reader[2].ToString(), string.Empty, reader[3].ToString(), reader[4].ToString(), int.Parse(reader[5].ToString()), reader[6].ToString(), qty, reader[9].ToString(), date, reader[10].ToString()));
            }
            reader.Dispose();
            command.Dispose();
        }

        conn.Dispose();
        return revisions;

    }

    public async Task<bool> UpdateRevision(CustomerRevision revision)
    {
        OracleParameter? p0 = new("p0", OracleDbType.Int16, revision.Status, ParameterDirection.Input);
        OracleParameter? p1 = new("p1", OracleDbType.Varchar2, 2000, revision.OrderNo, ParameterDirection.Input);
        OracleParameter? p2 = new("p2", OracleDbType.Varchar2, 2000, revision.PartNo, ParameterDirection.Input);
        var rs = await _context.Database.ExecuteSqlInterpolatedAsync($"UPDATE CUSTOMER_VERSION_MASTER_DATA SET STATUS = {p0}, CONFIRM_DATE=SYSDATE WHERE ORDER_NO = {p1} AND PART_NO={p2}");
        if(rs>0)
        {
            _=await _context.SaveChangesAsync();
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> UpdateRemarkDB(CustomerRevision revision)
    {
        OracleParameter? p0 = new("p0", OracleDbType.Varchar2, 2000, revision.Remark, ParameterDirection.Input);
        OracleParameter? p1 = new("p1", OracleDbType.Varchar2, 2000, revision.OrderNo, ParameterDirection.Input);
        OracleParameter? p2 = new("p2", OracleDbType.Varchar2, 2000, revision.PartNo, ParameterDirection.Input);
        var rs = await _context.Database.ExecuteSqlInterpolatedAsync($"UPDATE CUSTOMER_VERSION_MASTER_DATA SET REMARK = {p0}, CONFIRM_DATE=SYSDATE WHERE ORDER_NO = {p1} AND PART_NO={p2}");
        if(rs>0)
        {
            _=await _context.SaveChangesAsync();
            return true;
        }
        else
        {
            return false;
        }
    }
    public async Task<IEnumerable<StockByFamily>> GetStockByFamily(string family)
    {
        List<StockByFamily> stocks = new();
        OracleParameter? orderNoParam = new("P_FAMILY", OracleDbType.NVarchar2, 100, family, ParameterDirection.Input);
        OracleParameter? outputParam = new("P_REF_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
        //var res = await _context.Database.ExecuteSqlInterpolatedAsync($"BEGIN TRS_PLANNING_PKG.GET_CV_BY_FAMILY_PRC({familyParam},{Part_No}); END;");
        using TraceDbContext? context = _context;
        OracleConnection? conn = new(context.Database.GetConnectionString());
        var query = "TRS_CUSTOMER_VERION_PKG.GET_STOCK_BY_FAMILY_PRC";
        conn.Open();
        if(conn.State==ConnectionState.Open)
        {
            using OracleCommand? command = conn.CreateCommand();
            command.CommandText=query;
            command.CommandType=CommandType.StoredProcedure;
            _=command.Parameters.Add(orderNoParam);
            _=command.Parameters.Add(outputParam);
            command.Connection=conn;
            OracleDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                stocks.Add(new StockByFamily(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), int.Parse(reader[5].ToString())));
            }
            reader.Dispose();
            command.Dispose();
        }

        conn.Dispose();
        return stocks;
    }


    // Import Excel EPPlus
    public async Task<ExcelWorksheets> GetSheetFromExcel(string filePath)
    {
        //List<Shipment> shipments = new List<Shipment>();
        FileInfo fileInfo = new(filePath);
        ExcelPackage.LicenseContext=LicenseContext.NonCommercial;
        using ExcelPackage excelPackage = new(fileInfo);
        ExcelWorksheets excelWorksheets = excelPackage.Workbook.Worksheets;
        return excelWorksheets;

    }

    public async Task<bool> InsertPackingList(Shipment shipment)
    {
        OracleParameter? PO_NO = new("PO_NO", OracleDbType.Varchar2, 2000, shipment.PoNo, ParameterDirection.Input);
        OracleParameter? PART_NO = new("PART_NO", OracleDbType.Varchar2, 2000, shipment.PartNo, ParameterDirection.Input);
        OracleParameter? CUSTOMER_PO = new("CUSTOMER_PO", OracleDbType.Varchar2, 2000, shipment.CustomerPo, ParameterDirection.Input);
        OracleParameter? CUSTOMER_PART_NO = new("CUSTOMER_PART_NO", OracleDbType.Varchar2, 2000, shipment.CustomerPartNo, ParameterDirection.Input);
        OracleParameter? PART_DESC = new("PART_DESC", OracleDbType.Varchar2, 2000, shipment.PartDesc, ParameterDirection.Input);
        OracleParameter? SHIPPING_ADDRESS = new("SHIPPING_ADDRESS", OracleDbType.Varchar2, 2000, shipment.ShippingAddress, ParameterDirection.Input);
        OracleParameter? SHIPMODE = new("SHIPMODE", OracleDbType.Varchar2, 2000, shipment.ShipMode, ParameterDirection.Input);
        OracleParameter? SHIP_QTY = new("SHIP_QTY", OracleDbType.Int32, shipment.ShipQty, ParameterDirection.Input);
        OracleParameter? SHIPMENT_ID = new("SHIPMENT_ID", OracleDbType.Varchar2, 2000, shipment.ShipmentId, ParameterDirection.Input);
        OracleParameter? WEEK = new("WEEK", OracleDbType.Varchar2, 2000, shipment.Week_, ParameterDirection.Input);
        OracleParameter? YEAR = new("YEAR", OracleDbType.Varchar2, 2000, shipment.Year_, ParameterDirection.Input);
        var rs = await _context.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO PACKING_MASTER_LIST (PO_NO, PART_NO, CUSTOMER_PO, CUSTOMER_PART_NO, PART_DESC, SHIP_QTY, SHIPPING_ADDRESS, SHIPMODE, SHIPMENT_ID,WEEK_,YEAR_) VALUES({PO_NO}, {PART_NO}, {CUSTOMER_PO}, {CUSTOMER_PART_NO}, {PART_DESC}, {SHIP_QTY}, {SHIPPING_ADDRESS}, {SHIPMODE},{SHIPMENT_ID},{WEEK},{YEAR})");

        if(rs>0)
        {
            _=await _context.SaveChangesAsync();
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> UpdatePackingList(Shipment shipment)
    {
        try
        {
            OracleParameter? p0 = new("p0", OracleDbType.Int32, shipment.Idx, ParameterDirection.Input);
            OracleParameter? p1 = new("p1", OracleDbType.Int32, -3, ParameterDirection.Input);
            var rs = await _context.Database.ExecuteSqlInterpolatedAsync($"UPDATE PACKING_MASTER_LIST SET RAW_DATA = {p1} WHERE IDX = {p0}");
            if(rs>0)
            {
                _=await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        catch(Exception)
        {
            return false;
        }
    }

    public async Task<bool> ShipmentInfoUpdate(string shipment_id)
    {
        using(TraceDbContext? context = _context)
        {
            OracleParameter? p0 = new("p0", OracleDbType.Varchar2, 2000, shipment_id, ParameterDirection.Input);
            OracleConnection? conn = new(context.Database.GetConnectionString());
            var query = $"TRS_PACKING_MASTER_LIST_PKG.UPDATE_SHIPMENT_ID";
            conn.Open();
            if(conn.State==ConnectionState.Open)
            {
                using OracleCommand? command = conn.CreateCommand();
                command.CommandText=query;
                command.CommandType=CommandType.StoredProcedure;
                command.Connection=conn;
                _=command.Parameters.Add(p0);
                _=await command.ExecuteNonQueryAsync();
            }

            conn.Dispose();

        }
        _=await Task.FromResult(true);
        return true;
    }

    public async Task<bool> ShipmentInfoCalculation(string shipmentId)
    {
        using(TraceDbContext? context = _context)
        {
            OracleConnection? conn = new(context.Database.GetConnectionString());
            OracleParameter? p0 = new("p0", OracleDbType.Varchar2, 2000, shipmentId, ParameterDirection.Input);
            var query = $"TRS_PACKING_MASTER_LIST_PKG.CALCULATE_DATA";
            conn.Open();
            if(conn.State==ConnectionState.Open)
            {
                using OracleCommand? command = conn.CreateCommand();
                command.CommandText=query;
                command.CommandType=CommandType.StoredProcedure;
                command.Connection=conn;
                _=command.Parameters.Add(p0);
                _=await command.ExecuteNonQueryAsync();
            }

            conn.Dispose();

        }
        _=await Task.FromResult(true);
        return true;
    }

    public async Task<IEnumerable<Shipment>> GetLogisticData(string shipmentId = "ALL")
    {
        List<Shipment> revisions = new();
        Shipment s = new();
        OracleParameter? p0 = new("P0", OracleDbType.NVarchar2, shipmentId, ParameterDirection.Input);
        OracleParameter? p1 = new("P_REF_CURSOR", OracleDbType.RefCursor, revisions, ParameterDirection.Output);
        using TraceDbContext? context = _context;

        OracleConnection? conn = new(context.Database.GetConnectionString());
        var query = "TRS_PACKING_MASTER_LIST_PKG.GET_LOGISTIC_DATA_PRC";
        conn.Open();
        if(conn.State==ConnectionState.Open)
        {
            using OracleCommand? command = conn.CreateCommand();
            command.CommandText=query;
            command.CommandType=CommandType.StoredProcedure;
            _=command.Parameters.Add(p0);
            _=command.Parameters.Add(p1);
            command.Connection=conn;
            System.Data.Common.DbDataReader? reader = await command.ExecuteReaderAsync();

            while(reader.Read())
            {
                var i5 = 0;
                var i6 = 0;
                var i7 = 0;
                var i8 = 0;
                var i9 = 0.0;
                var i10 = 0.0;
                var i13 = 0.0;
                var i16 = 0;
                var i21 = 0;
                DateTime i22;
                var i23 = 0;

                i5=int.TryParse(reader[6].ToString(), out i5) ? i5 : 0;
                i6=int.TryParse(reader[7].ToString(), out i6) ? i6 : 0;
                i7=int.TryParse(reader[8].ToString(), out i7) ? i7 : 0;
                i8=int.TryParse(reader[9].ToString(), out i8) ? i8 : 0;
                i9=double.TryParse(reader[10].ToString(), out i9) ? i9 : 0;
                i10=double.TryParse(reader[11].ToString(), out i10) ? i10 : 0;

                i13=double.TryParse(reader[13].ToString(), out i13) ? i13 : 0;
                i16=int.TryParse(reader[16].ToString(), out i16) ? i16 : 0;
                i21=int.TryParse(reader[21].ToString(), out i21) ? i21 : 0;
                i22=DateTime.TryParse(reader[22].ToString(), out i22) ? i22 : DateTime.Now;
                i23=int.TryParse(reader[23].ToString(), out i23) ? i23 : 0;
                try
                {
                    s=new Shipment
                    {
                        PoNo=reader[0].ToString(),
                        PartNo=reader[1].ToString(),
                        CustomerPo=reader[2].ToString(),
                        CustomerPartNo=reader[3].ToString(),
                        PartDesc=reader[4].ToString(),
                        BarcodePallet=reader[5].ToString(),
                        CartonQty=i5,
                        RealPalletQty=i6,
                        ShipQty=i7,
                        PoTotalQty=i8,
                        Net=i9,
                        Gross=i10,
                        Dimension=reader[12].ToString(),
                        Cbm=i13,
                        ShippingAddress=reader[14].ToString(),
                        ShipMode=reader[15].ToString(),
                        PalletQtyStandard=i16,
                        TracePalletBarcode=reader[17].ToString(),
                        ShipmentId=reader[18].ToString(),
                        PackingListId=reader[19].ToString(),
                        ContainerNo=reader[20].ToString(),
                        Idx=i21,
                        ShippingDate=i22,
                        RawData=i23
                    };
                }
                catch(Exception)
                {

                }


                revisions.Add(s);


                for(var i = 0; i<17; i++)
                {
                    Console.WriteLine(reader[i]);
                }
            }
            reader.Dispose();
            command.Dispose();
        }

        conn.Dispose();
        return revisions.AsEnumerable();
    }

    public async Task<bool> UpdateInvoiceNumberToShipment(string shipmentId, string invoiceNumber)
    {
        OracleParameter? p0 = new("p0", OracleDbType.Varchar2, invoiceNumber, ParameterDirection.Input);
        OracleParameter? p1 = new("p1", OracleDbType.Varchar2, 2000, shipmentId, ParameterDirection.Input);

        var rs = await _context.Database.ExecuteSqlInterpolatedAsync($"UPDATE PACKING_MASTER_LIST SET PACKING_LIST_NUMBER = {p0} WHERE SHIPMENT_ID = {p1}");
        if(rs>0)
        {
            _=await _context.SaveChangesAsync();
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> UpdateContainerNoToShipment(string shipmentId, string containerNo)
    {
        OracleParameter? p0 = new("p0", OracleDbType.Varchar2, 2000, containerNo, ParameterDirection.Input);
        OracleParameter? p1 = new("p1", OracleDbType.Varchar2, 2000, shipmentId, ParameterDirection.Input);
        var rs = await _context.Database.ExecuteSqlInterpolatedAsync($"UPDATE PACKING_MASTER_LIST SET CONTAINER_NO = {p0} WHERE SHIPMENT_ID = {p1}");
        if(rs>0)
        {
            _=await _context.SaveChangesAsync();
            return true;
        }
        else
        {
            return false;
        }



    }

    public async Task<bool> UpdateShippingDateToShipment(string shipmentId, DateTime? dateTime)
    {
        OracleParameter? p0 = new("p0", OracleDbType.Date, dateTime, ParameterDirection.Input);
        OracleParameter? p1 = new("p1", OracleDbType.Varchar2, 2000, shipmentId, ParameterDirection.Input);
        var rs = await _context.Database.ExecuteSqlInterpolatedAsync($"UPDATE PACKING_MASTER_LIST SET SHIPPING_DATE = {p0} WHERE SHIPMENT_ID = {p1}");
        if(rs>0)
        {
            _=await _context.SaveChangesAsync();
            return true;
        }
        else
        {
            return false;
        }



    }

    public async Task<bool> UpdateInvoiceByIdx(int idx, string invoiceNumber)
    {
        try
        {
            OracleParameter? p0 = new("p0", OracleDbType.Int32, idx, ParameterDirection.Input);
            OracleParameter? p1 = new("p1", OracleDbType.Varchar2, 2000, invoiceNumber, ParameterDirection.Input);
            var rs = await _context.Database.ExecuteSqlInterpolatedAsync($"UPDATE PACKING_MASTER_LIST SET PACKING_LIST_NUMBER = {p1} WHERE IDX = {p0}");
            if(rs>0)
            {
                _=await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        catch(Exception)
        {
            return false;
        }
    }

    public async Task<bool> UpdateContainerByIdx(int idx, string container)
    {
        try
        {
            OracleParameter? p0 = new("p0", OracleDbType.Int32, idx, ParameterDirection.Input);
            OracleParameter? p1 = new("p1", OracleDbType.Varchar2, 2000, container, ParameterDirection.Input);
            var rs = await _context.Database.ExecuteSqlInterpolatedAsync($"UPDATE PACKING_MASTER_LIST SET CONTAINER_NO = {p1} WHERE IDX = {p0}");
            if(rs>0)
            {
                _=await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        catch(Exception)
        {
            return false;
        }

    }

    public async Task<bool> UpdateShippingDateByIdx(int idx, DateTime? shippingDate)
    {
        try
        {
            OracleParameter? p0 = new("p0", OracleDbType.Int32, idx, ParameterDirection.Input);
            OracleParameter? p1 = new("p1", OracleDbType.Date, shippingDate, ParameterDirection.Input);
            var rs = await _context.Database.ExecuteSqlInterpolatedAsync($"UPDATE PACKING_MASTER_LIST SET SHIPPING_DATE = {p1} WHERE IDX = {p0}");
            if(rs>0)
            {
                _=await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        catch(Exception)
        {
            return false;
        }

    }
    public async Task<bool> UpdateShippingDayByIdx(int idx, string container)
    {
        try
        {
            OracleParameter? p0 = new("p0", OracleDbType.Int32, idx, ParameterDirection.Input);
            OracleParameter? p1 = new("p1", OracleDbType.Varchar2, 2000, container, ParameterDirection.Input);
            var rs = await _context.Database.ExecuteSqlInterpolatedAsync($"UPDATE PACKING_MASTER_LIST SET CONTAINER_NO = {p1} WHERE IDX = {p0}");
            if(rs>0)
            {
                _=await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        catch(Exception)
        {
            return false;
        }

    }
    public async Task<IEnumerable<ModelProperties>> GetPalletContentInfoByPartNo(string partNo)
    {
        List<ModelProperties>? result = await _context.ModelProperties
                                   .Where(_ => _.PartNo==partNo)
                                   .AsNoTracking()
                                   .ToListAsync();

        return result.Select(s => new ModelProperties() { PartNo=s.PartNo, QtyPerBox=s.QtyPerBox }).Take(1).ToList().AsEnumerable();
    }

    public async Task<bool> UpdateRawDataByIdx(int idx, int rawdata)
    {
        try
        {
            OracleParameter? p0 = new("p0", OracleDbType.Int32, idx, ParameterDirection.Input);
            OracleParameter? p1 = new("p1", OracleDbType.Int32, rawdata, ParameterDirection.Input);
            var rs = await _context.Database.ExecuteSqlInterpolatedAsync($"UPDATE PACKING_MASTER_LIST SET RAW_DATA = {p1} WHERE IDX = {p0}");
            if(rs>0)
            {
                _=await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        catch(Exception)
        {
            return false;
        }

    }

    public async Task<IEnumerable<Shipment>> CheckExistShipmentId(string shipmentId)
    {
        try
        {
            List<Shipment> shipments = new();
            OracleParameter? id = new("P_SHIPMENT_ID", OracleDbType.NVarchar2, 100, shipmentId, ParameterDirection.Input);
            OracleParameter? outputParam = new("P_REF_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);

            using TraceDbContext? context = _context;
            OracleConnection? conn = new(context.Database.GetConnectionString());
            var query = "TRS_PACKING_MASTER_LIST_PKG.GET_SHIPMENTS_BY_ID_PRC";
            conn.Open();
            if(conn.State==ConnectionState.Open)
            {
                using OracleCommand? command = conn.CreateCommand();
                command.CommandText=query;
                command.CommandType=CommandType.StoredProcedure;
                _=command.Parameters.Add(id);
                _=command.Parameters.Add(outputParam);
                command.Connection=conn;
                OracleDataReader reader = command.ExecuteReader();
                while(reader.Read())
                {
                    var i = 0;
                    Shipment s = new();
                    i=int.TryParse(reader[12].ToString(), out i) ? i : 0;
                    s=new Shipment
                    {
                        Idx=i,
                        ShipMode=reader[6].ToString()
                    };
                    shipments.Add(s);
                }
                reader.Dispose();
                command.Dispose();
            }

            conn.Dispose();
            return shipments.AsEnumerable();
        }
        catch(Exception)
        {
            return null;
        }
    }

    public async Task<bool> UpdateNetGrossDimensionSCM(int idx, double net, double gross, string dimension)
    {
        try
        {
            OracleParameter? p0 = new("p0", OracleDbType.Int32, idx, ParameterDirection.Input);
            OracleParameter? p1 = new("p1", OracleDbType.Double, net, ParameterDirection.Input);
            OracleParameter? p2 = new("p2", OracleDbType.Double, gross, ParameterDirection.Input);
            OracleParameter? p3 = new("p3", OracleDbType.Varchar2, 2000, dimension, ParameterDirection.Input);
            var rs = await _context.Database.ExecuteSqlInterpolatedAsync($"UPDATE PACKING_MASTER_LIST SET NET = {p1}, GROSS = {p2}, DIMENSION={p3} WHERE IDX = {p0}");
            if(rs>0)
            {
                _=await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        catch(Exception)
        {
            return false;
        }
    }

    public async Task<IEnumerable<string>> GetFinishGoodByBarcodePallete(string barcodePallet)
    {
        List<string?>? result = await _context.FinishedGood
                              .Where(f => f.BarcodePalette==barcodePallet)
                              .Select(f => f.OrderNo)
                              .Distinct()
                              .AsNoTracking()
                              .ToListAsync();
        return result.ToList().AsEnumerable();
    }
}
