using Microsoft.EntityFrameworkCore;

#nullable disable

namespace MESystem.Data.HR;

public partial class HRDbContext : DbContext, IDbContextFactory<HRDbContext>
{
    public DbContextOptions _options { get; set; }

    private readonly IDbContextFactory<HRDbContext> _dbContextFactory;

    public HRDbContext(DbContextOptions options, IDbContextFactory<HRDbContext> contextFactory) : base(options)
    {
        _options=options;
        _dbContextFactory=contextFactory;
    }



    public virtual DbSet<CheckInOut> CheckInOuts { get; set; }
    public virtual DbSet<UserInfo> UserInfos { get; set; }
    public virtual DbSet<Attendance> Attendances { get; set; }
    public virtual DbSet<RelationDept> RelationDepts { get; set; }

     

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        _=modelBuilder.HasAnnotation("Relational:Collation", "Latin1_General_CI_AS");

        _=modelBuilder
            .Entity<CheckInOut>()
            .ToTable("CheckInOut")
            .HasKey(c => new { c.UserEnrollNumber, c.TimeStr });

        _=modelBuilder
            .Entity<Attendance>().HasNoKey();

       
        _=modelBuilder
            .Entity<UserInfo>()
            .ToTable("UserInfo")
            .HasKey(c => new { c.UserEnrollNumber });

        _=modelBuilder
            .Entity<RelationDept>()
            .ToTable("RelationDept")
            .HasKey(c => new { c.RelationID });


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    HRDbContext IDbContextFactory<HRDbContext>.CreateDbContext()
    {
        return _dbContextFactory.CreateDbContext();
    }
}