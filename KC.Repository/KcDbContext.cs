using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Models;
using Microsoft.EntityFrameworkCore;

namespace KC.Repository
{
    public class KcDbContext : DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<BettingBox> BettingBoxes { get; set; }

        public KcDbContext(DbContextOptions<KcDbContext> options) => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("KcDb").UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>().HasKey(p => p.Id);
            modelBuilder.Entity<Table>().HasKey(t => t.Id);
            modelBuilder.Entity<BettingBox>().HasKey(b => b.Id);

            modelBuilder.Entity<BettingBox>()
                .HasOne(b => b.Table)
                .WithMany(t => t.BettingBoxes)
                .HasForeignKey(b => b.TableId);

            modelBuilder.Entity<BettingBox>()
                .HasOne(b => b.OwnerNullable)
                .WithMany(p => p.BettingBoxes)
                .HasForeignKey(b => b.OwnerId);

        }
    }
}
