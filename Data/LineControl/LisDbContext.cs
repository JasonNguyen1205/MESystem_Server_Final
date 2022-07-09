using Microsoft.EntityFrameworkCore;

#nullable disable

namespace MESystem.Data.LineControl;

public partial class LisDbContext : DbContext
{
    public LisDbContext()
    {
    }

    public LisDbContext(DbContextOptions<LisDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<vLineEvent> LineEvents { get; set; }
    public virtual DbSet<LineComments> LineComment { get; set; }
    public virtual DbSet<LineEvent> LineEventComment { get; set; }
    public virtual DbSet<LastLineState> LastLineStates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _=modelBuilder.HasAnnotation("Relational:Collation", "Latin1_General_CI_AS");

        _=modelBuilder.Entity<vLineEvent>()
            .HasAlternateKey(c => new { c.Id, c.OrderNo });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

}