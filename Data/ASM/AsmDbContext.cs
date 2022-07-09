using Microsoft.EntityFrameworkCore;

#nullable disable

namespace MESystem.Data.ASM;

public partial class AsmDbContext : DbContext
{
    DbContextOptions db;

    public AsmDbContext(DbContextOptions options) : base(options)
    {
        db=options;
        _=Task.Run(() =>
        {
            using AsmDbContext dbContext = new(options);
            Microsoft.EntityFrameworkCore.Metadata.IModel model = dbContext.Model; //force the model creation
        });
    }


    public virtual DbSet<ProdData> ProductionData { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _=modelBuilder.HasAnnotation("Relational:Collation", "Latin1_General_CI_AS");

        _=modelBuilder.Entity<ProdData>()
            .HasAlternateKey(c => new { c.OrderNo });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

}