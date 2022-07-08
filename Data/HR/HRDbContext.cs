using MESystem.Pages.HR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

#nullable disable

namespace MESystem.Data.HR
{
    public partial class HRDbContext : DbContext
    {
        private readonly DbContextOptions _options;

        public HRDbContext(DbContextOptions options) : base(options)
        {
            _options = options;
        }

     

        public virtual DbSet<HRCheckInOut> CheckInOuts { get; set; }
        //public virtual DbSet<TProductionPlanSMT> ProductionPlanSMT { get; set; }
        //public virtual DbSet<vStencilOverview> vStencilOverview { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Latin1_General_CI_AS");

            modelBuilder
                .Entity<HRCheckInOut>()
                .ToTable(nameof(HRCheckInOut))
                .HasKey(c => new { c.UserEnrollNumber, c.TimeStr});

            //modelBuilder
            //    .Entity<TProductionPlanSMT>()
            //    .ToView("vProductionPlanSMT")
            //    .HasKey(c => new { c.Id });

            //modelBuilder
            //    .Entity<vStencilOverview>()
            //    .ToView("vstencil_overview")
            //    .HasAlternateKey(c => new { c.StorageID });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    }
}