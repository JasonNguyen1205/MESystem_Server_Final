using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

#nullable disable

namespace MESystem.Data.TRACE
{
#pragma warning disable EF1001 // Internal EF Core API usage.
    public partial class TraceDbContext : DbContext, IDbContextPool
#pragma warning restore EF1001 // Internal EF Core API usage.
    {
        DbContextOptions db;

        public TraceDbContext(DbContextOptions options) : base(options)
        {
            db = options;
        }

        public IDbContextPoolable Rent()
        {
            return new TraceDbContext(db);
        }

        public void Return(IDbContextPoolable context)
        {
            this.Rent().Dispose();
        }

        public ValueTask ReturnAsync(IDbContextPoolable context, CancellationToken cancellationToken = default)
        {

            return this.Rent().DisposeAsync();
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Latin1_General_CI_AS");

            modelBuilder.Entity<CustomerOrder>()
                        .HasNoKey();

            modelBuilder.Entity<FinishedGood>()
                        .HasIndex(f => f.BarcodeBox);

            modelBuilder.Entity<ModelProperties>()
                        .HasKey(mp => mp.Id);

            modelBuilder.Entity<vFinishedGoods>()
                        .HasIndex(f => f.BarcodeBox);

            modelBuilder.Entity<ProductionLine>()
                        .HasNoKey();

            modelBuilder.Entity<ProductionPlanLine>()
                        .HasAlternateKey(c => new { c.Id, c.OrderNo, c.DepartmentNo, c.OperationNo });

            modelBuilder.Entity<vProductionPlan>()
                        .ToView("V_PRODUCTION_PLAN")
                        .HasKey(c => c.Id);
            //.HasAlternateKey(c => new { c.OrderNo, c.DepartmentNo, c.OperationNo });

            modelBuilder.Entity<vProductionPlanIFS>()
                        .ToView("V_PRODUCTION_PLAN_IFS")
                        .HasKey(c => c.OrderNo);

            modelBuilder.Entity<vProductionPlanJigs>()
                        .ToView("V_PRODUCTION_PLAN_JIGS")
                        .HasNoKey();
            //.HasKey(c => c.Id);

            modelBuilder.Entity<TotalQuantitys>()
                        .HasNoKey();

            modelBuilder.Entity<vShopOrderStates>()
                        .ToView("V_SHOP_ORDER_STATUS")
                        .HasNoKey();

            modelBuilder.Entity<vShopOrderStateMI>()
                        .ToView("V_SHOP_ORDER_STATUS_MI")
                        .HasNoKey();

            modelBuilder.Entity<vShopOrderLinks>()
                        .ToView("V_SHOP_ORDER_LINK")
                        .HasNoKey();

            //for Setup Instructions
            modelBuilder.Entity<SiPartsPerHour>()
                        .HasKey(c => c.Id);

            modelBuilder.Entity<SiFamily>()
                        .HasKey(c => c.Id);

            modelBuilder.Entity<SiManufacturingTool>()
                        .HasKey(c => c.Id);

            modelBuilder.Entity<SiManufacturingToolLink>()
                        .HasKey(c => c.Id);

            modelBuilder.Entity<SiManufacturingToolType>()
                        .HasKey(c => c.Id);

            modelBuilder.Entity<vSiManufacturingToolPart>()
                        .ToView("V_SI_MANUF_TOOL_PART_NO")
                        .HasNoKey();
            //.HasAlternateKey(c => new { c.PartNo, c.ManufToolId });

            modelBuilder.Entity<SiProduct>()
                        .HasNoKey();
            //.HasKey(c => c.Id);

            modelBuilder.Entity<vSiProductFamily>()
                        .ToView("V_SI_PRODUCTS_FAMILYS")
                        .HasKey(c => c.Id);

            modelBuilder.Entity<vProductionLayout>()
                        .ToView("V_SI_PRODUCTION_LAYOUTS")
                        .HasKey(c => c.Id);

            modelBuilder.Entity<vDepartmentStation>()
                        .ToView("V_SI_DEPARTMENTS_STATIONS")
                        .HasKey(c => c.StationId);

            modelBuilder.Entity<FinalResultFg>()
                        //.HasKey(c => c.Barcode)
                        .HasIndex(c => new { c.Barcode, c.Result, c.Status, c.Remark });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}