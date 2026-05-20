using Microsoft.EntityFrameworkCore;
using Products.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Infrastructure.Data.Contexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
                entity.Property(p => p.Description).IsRequired().HasMaxLength(1000);
                entity.Property(p => p.OldPrice).HasColumnType("decimal(18,2)");
                entity.Property(p => p.NewPrice).HasColumnType("decimal(18,2)");
                entity.Property(p => p.ImagePath).HasMaxLength(500);
            });
        }
    }
}
