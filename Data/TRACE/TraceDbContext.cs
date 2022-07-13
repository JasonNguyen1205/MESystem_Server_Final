using Microsoft.EntityFrameworkCore;

namespace MESystem.Data.TRACE;

public partial class TraceDbContext : DbContext
{
    DbContextOptions db;
    public TraceDbContext(DbContextOptions options) : base(options)
    {
        db=options;
    }

    public virtual DbSet<CustomerOrder> CustomerOrders { get; set; }
    public virtual DbSet<FinishedGood> FinishedGood { get; set; }
    public virtual DbSet<vFinishedGoods> vFinishedGoods { get; set; }
    public virtual DbSet<ModelProperties> ModelProperties { get; set; }
    public virtual DbSet<ProductionLine> ProductionLines { get; set; }
    public virtual DbSet<ProductionPlanLine> ProductionPlanLines { get; set; }
    public virtual DbSet<vProductionPlan> ProductionPlan { get; set; }
    public virtual DbSet<vProductionPlanIFS> ProductionPlanIfs { get; set; }
    public virtual DbSet<vProductionPlanJigs> ProductionPlanJigs { get; set; }

    //public virtual DbSet<vProductionPlanSMTdef> ProductionPlanSMT { get; set; }
    public virtual DbSet<TotalQuantitys> TotalQuantity { get; set; }
    public virtual DbSet<vShopOrderStates> ShopOrderState { get; set; }
    public virtual DbSet<vShopOrderStateMI> ShopOrderStateMI { get; set; }
    public virtual DbSet<vShopOrderLinks> ShopOrderLink { get; set; }

    //for Setup Instructions
    public virtual DbSet<SiPartsPerHour> SiPartPerhour { get; set; }
    public virtual DbSet<SiFamily> SiFamilys { get; set; }
    public virtual DbSet<SiManufacturingTool> SiManufacturingTools { get; set; }
    public virtual DbSet<SiManufacturingToolLink> SiManufacturingToolLinks { get; set; }
    public virtual DbSet<SiManufacturingToolType> SiManufacturingToolTypes { get; set; }
    public virtual DbSet<vSiManufacturingToolPart> SiManufacturingToolParts { get; set; }
    public virtual DbSet<SiProduct> SiProducts { get; set; }
    public virtual DbSet<vSiProductFamily> vSiProductFamilys { get; set; }
    public virtual DbSet<vProductionLayout> vProductionLayouts { get; set; }
    public virtual DbSet<vDepartmentStation> vDepartmentStations { get; set; }
    public virtual DbSet<FinalResultFg> FinalResultFgs { get; set; }

    public virtual DbSet<Shipment> Shipments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _=modelBuilder.HasAnnotation("Relational:Collation", "Latin1_General_CI_AS");

        _=modelBuilder.Entity<CustomerOrder>()
                    .HasNoKey();

        _=modelBuilder.Entity<FinishedGood>()
                    .HasIndex(f => f.BarcodeBox);

        _=modelBuilder.Entity<ModelProperties>()
                    .HasKey(mp => mp.Id);

        _=modelBuilder.Entity<vFinishedGoods>()
                    .HasIndex(f => f.BarcodeBox);

        _=modelBuilder.Entity<ProductionLine>()
                    .HasNoKey();

        _=modelBuilder.Entity<ProductionPlanLine>()
                    .HasAlternateKey(c => new { c.Id, c.OrderNo, c.DepartmentNo, c.OperationNo });

        _=modelBuilder.Entity<vProductionPlan>()
                    .ToView("V_PRODUCTION_PLAN")
                    .HasKey(c => c.Id);
        //.HasAlternateKey(c => new { c.OrderNo, c.DepartmentNo, c.OperationNo });

        _=modelBuilder.Entity<vProductionPlanIFS>()
                    .ToView("V_PRODUCTION_PLAN_IFS")
                    .HasKey(c => c.OrderNo);

        _=modelBuilder.Entity<vProductionPlanJigs>()
                    .ToView("V_PRODUCTION_PLAN_JIGS")
                    .HasNoKey();
        //.HasKey(c => c.Id);

        _=modelBuilder.Entity<TotalQuantitys>()
                    .HasNoKey();

        _=modelBuilder.Entity<vShopOrderStates>()
                    .ToView("V_SHOP_ORDER_STATUS")
                    .HasNoKey();

        _=modelBuilder.Entity<vShopOrderStateMI>()
                    .ToView("V_SHOP_ORDER_STATUS_MI")
                    .HasNoKey();

        _=modelBuilder.Entity<vShopOrderLinks>()
                    .ToView("V_SHOP_ORDER_LINK")
                    .HasNoKey();

        //for Setup Instructions
        _=modelBuilder.Entity<SiPartsPerHour>()
                    .HasKey(c => c.Id);

        _=modelBuilder.Entity<SiFamily>()
                    .HasKey(c => c.Id);

        _=modelBuilder.Entity<SiManufacturingTool>()
                    .HasKey(c => c.Id);

        _=modelBuilder.Entity<SiManufacturingToolLink>()
                    .HasKey(c => c.Id);

        _=modelBuilder.Entity<SiManufacturingToolType>()
                    .HasKey(c => c.Id);

        _=modelBuilder.Entity<vSiManufacturingToolPart>()
                    .ToView("V_SI_MANUF_TOOL_PART_NO")
                    .HasNoKey();
        //.HasAlternateKey(c => new { c.PartNos, c.ManufToolId });

        _=modelBuilder.Entity<SiProduct>()
                    .HasNoKey();
        //.HasKey(c => c.Id);

        _=modelBuilder.Entity<vSiProductFamily>()
                    .ToView("V_SI_PRODUCTS_FAMILYS")
                    .HasKey(c => c.Id);

        _=modelBuilder.Entity<vProductionLayout>()
                    .ToView("V_SI_PRODUCTION_LAYOUTS")
                    .HasKey(c => c.Id);

        _=modelBuilder.Entity<vDepartmentStation>()
                    .ToView("V_SI_DEPARTMENTS_STATIONS")
                    .HasKey(c => c.StationId);

        _=modelBuilder.Entity<FinalResultFg>()
                    //.HasKey(c => c.Barcode)
                    .HasIndex(c => new { c.Barcode, c.Result, c.Status, c.Remark });

        _=modelBuilder.Entity<Shipment>()
            .ToTable("PACKING_MASTER_LIST")
            .HasKey(c => c.Idx);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}