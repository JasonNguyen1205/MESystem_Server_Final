using System.Data;

using MESystem.Data.IFS;

using Microsoft.EntityFrameworkCore;

namespace MESystem.Service;

public class IfsService
{
    private readonly IfsDbContext _context;
    public IfsService(IfsDbContext context)
    {
        _context=context;
    }

    public async Task<ProductionPlanFIS>
        GetShopOrderDetails(string orderNo, string releaserNo, string sequenceNo)
    {
        ProductionPlanFIS editFormData = null;

        List<ProductionPlanFIS>? result = await _context.ProductionPlans
                                   .Where(s => s.OrderNo==orderNo
                                            &&s.ReleaseNo==releaserNo
                                            &&s.SequenceNo==sequenceNo)
                                   .AsNoTracking()
                                   .ToListAsync();

        if(result.Count()!=0)
        {
            editFormData=new ProductionPlanFIS()
            {
                OrderNo=result.FirstOrDefault().OrderNo,
                PartNo=result.FirstOrDefault().PartNo,
                PartDescription=result.FirstOrDefault().PartDescription,
                Station=result.FirstOrDefault().Station,
                WorkCenterNo=result.FirstOrDefault().WorkCenterNo
            };
        }
        else
        {
            editFormData=new ProductionPlanFIS()
            {
                OrderNo="",
                PartNo="NO DATA",
                PartDescription="",
                Station="",
                WorkCenterNo=""
            };
        }

        return editFormData;
    }

    public async Task<IEnumerable<ProductionPlanFIS>>
        GetProductionPlan(string departmentFilter, string workcenterFilter)
    {
        var stringsToSearchFor = workcenterFilter.Split(",");

        if(!workcenterFilter.Equals("all"))
        {
            return await _context.ProductionPlans
                                  .AsNoTrackingWithIdentityResolution()
                                  //.Where(s => s.DepartmentNo == departmentFilter && s.WorkCenterNo == workcenterFilter && s.OperStatuscode != "Abgeschlossen" && s.OperStatuscode != "Geplant")
                                  .Where(s => s.DepartmentNo==departmentFilter&&stringsToSearchFor.Contains(s.WorkCenterNo)&&s.OperStatuscode!="Abgeschlossen"&&s.OperStatuscode!="Geplant")
                                  .AsNoTracking()
                                  .OrderBy(o => o.Station).ThenByDescending(o => o.PercentDone).ThenBy(o => o.OperationPriority)
                                  .ToListAsync();
        }
        else
        {
            return await _context.ProductionPlans
                                 .AsNoTracking()
                                 .Where(s => s.DepartmentNo==departmentFilter&&s.OperStatuscode!="Abgeschlossen"&&s.OperStatuscode!="Geplant")
                                 .OrderBy(o => o.Station).ThenByDescending(o => o.PercentDone).ThenBy(o => o.OperationPriority)
                                 .AsNoTracking()
                                 .ToListAsync();
        }
    }

    public async Task<IEnumerable<ShopOrderOperTool>>
        GetShopOrderOperToolsByShopOrder(string orderNo, string releaserNo, string sequenceNo)
    {
        var query = "select distinct  opt.CONTRACT,"+
                                        "opt.ORDER_NO,"+
                                        "opt.RELEASE_NO,"+
                                        "opt.SEQUENCE_NO,"+
                                        "opt.TOOL_ID,"+
                                        "mtd.DESCRIPTION,"+
                                        "opt.TOOL_QUANTITY,"+
                                        "(select nvl(max(cf$_fold), 'N/A') from ifsapp.manuf_tool_detail_cfv WHERE tool_id = opt.tool_id AND (normal_production_line = 'FRIWO' OR normal_production_line = 'VIETNAM')) AS FOLD, "+
                                        "count(mtd.tool_id) AS AVAILABLE,"+
                                        "opt.cf$_tool_instance,"+
                                        "opt.OBJID, "+
                                        "opt.OBJVERSION "+
                                        "FROM IFSAPP.SHOP_ORDER_OPER_TOOL_cfv opt "+
                                        "join IFSAPP.MANUF_TOOL_DETAIL_CFV mtd "+
                                        "on(mtd.tool_id = opt.tool_id "+
                                        "and mtd.contract = opt.contract) "+
                                        "join ifsapp.manuf_tool mt "+
                                        "on(mt.tool_id = opt.tool_id "+
                                        "and mt.contract = opt.contract) "+
                                        "WHERE opt.order_no = '"+orderNo+"' AND opt.release_no = '"+releaserNo+"' AND opt.sequence_no = '"+sequenceNo+"' "+
                                        "and mt.tool_type in ('01', '02', '06', '14', '20', '21', '22', '23', '24', '27', '30', '32', '33', '34', '37', '39', '40', '44', '45', '50', '53', '56', '70', '73', '76', '78', '80', '98') "+
                                        "and (mtd.normal_production_line = 'FRIWO' or mtd.normal_production_line = 'VIETNAM') "+
                                        "group by opt.contract,opt.order_no, opt.release_no,opt.sequence_no, opt.tool_id,mtd.description, opt.tool_quantity, opt.OBJID, opt.OBJVERSION, opt.cf$_tool_instance "+
                                        "order by opt.tool_id";

        List<ShopOrderOperTool>? result = _context.ShopOrderOperTools
                             .FromSqlRaw(query)
                             .AsNoTracking()
                             .ToList();

        return result;
    }

    public Task<int>
        GetShopOrderOperToolsQty(string orderNo, string releaserNo, string sequenceNo)
    {
        var query = "SELECT count (rowNumber) AS ToolsTotal "+
                                "FROM(SELECT distinct mtd.tool_instance, "+
                                        "Row_number() over( PARTITION BY mtd.tool_id ORDER BY mtd.tool_instance ASC) AS rowNumber "+
                                        "FROM ifsapp.shop_order_oper_tool_cfv opt "+
                                        "join ifsapp.manuf_tool_detail_cfv mtd "+
                                                "on(mtd.tool_id = opt.tool_id "+
                                                "and mtd.contract = opt.contract) "+
                                        "join ifsapp.manuf_tool mt "+
                                                "on(mt.tool_id = opt.tool_id "+
                                                "and mt.contract = opt.contract) "+
                                        "WHERE opt.order_no = '"+orderNo+"' AND opt.release_no = '"+releaserNo+"' AND opt.sequence_no = '"+sequenceNo+"' "+
                                                "and mt.tool_type in ('01', '02', '06', '14', '20', '21', '22', '23', '24', '27', '30', '32', '33', '34', '37', '39', '40', '44', '45', '50', '53', '56', '70', '73', '76', '78', '80', '98') "+
                                                "and (mtd.normal_production_line = 'FRIWO' or mtd.normal_production_line = 'VIETNAM') )  "+
                                        "WHERE rowNumber = 1";

        List<ShopOrderOperToolQty>? result = _context.ShopOrderOperToolsQty
                    .FromSqlRaw(query)
                    .ToList();

        return Task.FromResult(result[0].ToolsTotal);
    }

    public async Task<IEnumerable<ManufToolDetail>>
        GetValuesFromInternalCode(string toolId)
    {
        return await _context.ManufToolDetails
                             .Where(e => e.ToolId==toolId)
                             .AsNoTracking()
                             .ToListAsync();
    }
    public string UpdateOrderOperTool(string orderNo, string releaseNo, string sequenceNo, string toolBarcode, string workCenter)
    {
        var toolInformation = "";
        var result = (from mtd in _context.ManufToolDetails
                      join opt in _context.ShopOrderOperTools
                      on mtd.ToolId equals opt.ToolId
                      select new
                      {
                          orderNo = opt.OrderNo,
                          releaseNo = opt.ReleaseNo,
                          sequenceNo = opt.SequenceNo,
                          toolID = mtd.ToolId,
                          toolInstance = mtd.ToolInstance,
                          barcode = mtd.Barcode,
                          objid_mtd = mtd.Objid,
                          objver_mtd = mtd.Objversion,
                          objid_opt = opt.Objid,
                          objver_opt = opt.Objversion
                      }).Where(s => s.orderNo==orderNo
                                 &&s.releaseNo==releaseNo
                                 &&s.sequenceNo==sequenceNo
                                 &&s.barcode==toolBarcode);

        foreach(var t in result)
        {
            toolInformation=t.toolInstance+";"+t.toolID;
            SaveChangesToIFS(t.objid_mtd, t.objver_mtd, "NORMAL_WORK_CENTER_NO", workCenter, "manuf_tool_detail_cfp", "");
            SaveChangesToIFS(t.objid_opt, t.objver_opt, "CF$_TOOL_INSTANCE", t.toolInstance, "shop_order_oper_tool_cfp", "cf_");
            break;
        }
        return toolInformation;
    }

    public void UpdateManufToolDetails(string barcode, string storageLocation, string toolInstance)
    {
        IQueryable<ManufToolDetail>? result = _context.ManufToolDetails.Where(_ => false);
        if(!string.IsNullOrEmpty(barcode))
        {
            result=_context.ManufToolDetails.Where(s => s.Barcode==barcode);
        }

        if(!string.IsNullOrEmpty(toolInstance))
        {
            result=_context.ManufToolDetails.Where(s => s.ToolInstance==toolInstance);
        }

        if(result.Count()>0)
        {
            SaveChangesToIFS(result.Max(s => s.Objid).ToString(), result.Max(s => s.Objversion), "NORMAL_WORK_CENTER_NO", storageLocation, "manuf_tool_detail_cfp", "");
        }
    }

    public void UpdateOrderNo(string orderNo, string orderNoMI)
    {
        IQueryable<ShopOrd>? result = _context.ShopOrds.Where(s => s.OrderNo==orderNo);

        if(result.Count()>0)
        {
            SaveChangesToIFS(result.Max(s => s.Objid).ToString(), result.Max(s => s.Objversion), "CF$_LINKED_ORDER_NO", orderNoMI, "shop_ord_cfp", "cf_");
        }
    }

    public async void SaveChangesToIFS(string obejctId, string objectVersion, string column, string newValue, string table, string cv)
    {
        var queryUpdate = "DECLARE info_ VARCHAR2(4000); "
                                    +"objid_ VARCHAR2(4000); "
                                    +"objversion_ VARCHAR2(4000); "
                                    +"attr_ VARCHAR2(4000); "
                                    +"attr_cf_ VARCHAR2(4000); "
                                    +"BEGIN "
                                    +"objid_ := '"+obejctId+"'; "
                                    +"objversion_:= '"+objectVersion+"'; "
                                    +"ifsapp.client_sys.add_to_attr('"+column+"', '"+newValue+"', attr_"+cv+"); "
                                    +"ifsapp."+table+".Modify__(info_, objid_, objversion_, attr_cf_, attr_, 'DO'); "
                                    +"COMMIT; "
                                    +"END; ";

        _=await _context.Database.ExecuteSqlRawAsync(queryUpdate);
    }

    //first way to save to IFS, better for calling procedures
    //public async void SaveToManufToolDetail(string storageLocation, string obejctId, string objectVersion)
    //{
    //    var parameters = new OracleParameter[]
    //    {
    //        new OracleParameter("p_objid", obejctId ),
    //        new OracleParameter("p_objversion", objectVersion ),
    //        new OracleParameter("p_storageLocation", storageLocation)
    //    };

    //    string queryUpdate = "DECLARE info_ VARCHAR2(4000); "
    //                                + "objid_ VARCHAR2(4000); "
    //                                + "objversion_ VARCHAR2(4000); "
    //                                + "attr_ VARCHAR2(4000); "
    //                                + "attr_cf_ VARCHAR2(4000); "
    //                                + "BEGIN "
    //                                + "objid_ := :p_objid; "
    //                                + "objversion_:= :p_objversion; "
    //                                + "ifsapp.client_sys.add_to_attr('NORMAL_WORK_CENTER_NO', :p_storageLocation, attr_); "
    //                                + "ifsapp.manuf_tool_detail_cfp.Modify__(info_, objid_, objversion_, attr_cf_, attr_, 'DO'); "
    //                                + "COMMIT; "
    //                                + "END; ";

    //    await _context.Database.ExecuteSqlRawAsync(queryUpdate, parameters);
    //}
}
