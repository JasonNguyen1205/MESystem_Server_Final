using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Threading;
using System.Threading.Tasks;

#nullable disable

namespace MESystem.Data.ASM
{
#pragma warning disable EF1001 // Internal EF Core API usage.
    public partial class AsmDbContext : DbContext, IDbContextPool
#pragma warning restore EF1001 // Internal EF Core API usage.
    {
        DbContextOptions db;

        public AsmDbContext(DbContextOptions options) : base(options)
        {
            db = options;
            Task.Run(() =>
            {
                using (var dbContext = new AsmDbContext(options))
                {
                    var model = dbContext.Model; //force the model creation
                }
            });
        }

        public IDbContextPoolable Rent()
        {
            return new AsmDbContext(db);
        }

        public void Return(IDbContextPoolable context)
        {
            this.Rent().Dispose();
        }

        public ValueTask ReturnAsync(IDbContextPoolable context, CancellationToken cancellationToken = default)
        {

            return this.Rent().DisposeAsync();
        }

        public virtual DbSet<ProdData> ProductionData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Latin1_General_CI_AS");

            modelBuilder.Entity<ProdData>()
                .HasAlternateKey(c => new { c.OrderNo });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    }
}