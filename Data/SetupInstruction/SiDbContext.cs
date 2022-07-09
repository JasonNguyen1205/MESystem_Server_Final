using Microsoft.EntityFrameworkCore;

#nullable disable

namespace MESystem.Data.SetupInstruction;

public partial class SiDbContext : DbContext
{
    DbContextOptions db;

    public SiDbContext(DbContextOptions options) : base(options)
    {
        db=options;
        //Task.Run(() =>
        //{
        //    using (var dbContext = new SiDbContext(options))
        //    {
        //        var model = dbContext.Model; //force the model creation
        //    }
        //});
    }


    public virtual DbSet<vProgramInformation> ProgramInformations { get; set; }
    public virtual DbSet<TProductionPlanSMT> ProductionPlanSMT { get; set; }
    public virtual DbSet<vStencilOverview> vStencilOverview { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.HasAnnotation("Relational:Collation", "Latin1_General_CI_AS");

        _=modelBuilder
            .Entity<vProgramInformation>()
            .ToView(nameof(vProgramInformation))
            .HasAlternateKey(c => new { c.Id, c.PartNo, c.Side });

        _=modelBuilder
            .Entity<TProductionPlanSMT>()
            .ToView("vProductionPlanSMT")
            .HasKey(c => new { c.Id });

        _=modelBuilder
            .Entity<vStencilOverview>()
            .ToView("vstencil_overview")
            .HasAlternateKey(c => new { c.StorageID });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

}