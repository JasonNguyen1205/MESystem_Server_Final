using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

#nullable disable

namespace MESystem.Data.IFS;

public partial class IfsDbContext : DbContext, IDbContextPool
{
    DbContextOptions db;


    public IfsDbContext(DbContextOptions options) : base(options)
    {
        db=options;
    }

    public IDbContextPoolable Rent()
    {
        return new IfsDbContext(db);
    }

    public void Return(IDbContextPoolable context)
    {
        this.Rent().Dispose();
    }

    public ValueTask ReturnAsync(IDbContextPoolable context, CancellationToken cancellationToken = default)
    {

        return this.Rent().DisposeAsync();
    }

    //public IfsDbContext()
    //{
    //}

    //public IfsDbContext(DbContextOptions<IfsDbContext> options)
    //    : base(options)
    //{
    //    //Task.Run(() =>
    //    //{
    //    //    using (var dbContext = new IfsDbContext(options))
    //    //    {
    //    //        var model = dbContext.Model; //force the model creation
    //    //    }
    //    //});
    //}

    public virtual DbSet<ShopOrd> ShopOrds { get; set; }
    public virtual DbSet<ProductionPlanFIS> ProductionPlans { get; set; }
    public virtual DbSet<ShopOrderOperTool> ShopOrderOperTools { get; set; }
    public virtual DbSet<ShopOrderOperToolQty> ShopOrderOperToolsQty { get; set; }
    public virtual DbSet<ManufToolDetail> ManufToolDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _=modelBuilder.HasAnnotation("Relational:Collation", "Latin1_General_CI_AS");

        _=modelBuilder.Entity<ShopOrd>()
                    .HasNoKey();

        _=modelBuilder.Entity<ProductionPlanFIS>()
                    .HasNoKey();

        _=modelBuilder.Entity<ShopOrderOperTool>()
                    .HasNoKey();

        _=modelBuilder.Entity<ShopOrderOperToolQty>()
                    .HasNoKey();

        _=modelBuilder.Entity<ManufToolDetail>()
                    .HasNoKey();

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

}