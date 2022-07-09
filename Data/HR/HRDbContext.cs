using Microsoft.EntityFrameworkCore;

#nullable disable

namespace MESystem.Data.HR;

public partial class HRDbContext : DbContext
{
    private readonly DbContextOptions _options;

    public HRDbContext(DbContextOptions options) : base(options)
    {
        _options=options;
    }



    public virtual DbSet<CheckInOut> CheckInOuts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _=modelBuilder.HasAnnotation("Relational:Collation", "Latin1_General_CI_AS");

        _=modelBuilder
            .Entity<CheckInOut>()
            .ToTable("CheckInOut")
            .HasKey(c => new { c.UserEnrollNumber, c.TimeStr });
            

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

}