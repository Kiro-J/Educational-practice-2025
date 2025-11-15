using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.Domain.ModelsDb;

namespace Service.DAL
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<UserDb> Users { get; set; }
        public DbSet<TakeDb> Takes { get; set; }
        public DbSet<InventoryDb> Inventory { get; set; }
        public DbSet<OrderDb> Orders { get; set; }
        public DbSet<DeliveryDb> Deliveries { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    // Configure relationships

        //    // User -> Takes (One-to-Many)
        //    modelBuilder.Entity<UserDb>()
        //        .HasMany(u => u.Takes)
        //        .WithOne(t => t.User)
        //        .HasForeignKey(t => t.UserId)
        //        .OnDelete(DeleteBehavior.Cascade);

        //    // User -> Orders (One-to-Many)
        //    modelBuilder.Entity<UserDb>()
        //        .HasMany(u => u.Orders)
        //        .WithOne(o => o.User)
        //        .HasForeignKey(o => o.UserId)
        //        .OnDelete(DeleteBehavior.Cascade);

        //    // Inventory -> Orders (One-to-Many)
        //    modelBuilder.Entity<InventoryDb>()
        //        .HasMany(i => i.Orders)
        //        .WithOne(o => o.Inventory)
        //        .HasForeignKey(o => o.ItemId)
        //        .OnDelete(DeleteBehavior.Cascade);

        //    // Order -> Delivery (One-to-One)
        //    modelBuilder.Entity<OrderDb>()
        //        .HasOne(o => o.Delivery)
        //        .WithOne(d => d.Order)
        //        .HasForeignKey<DeliveryDb>(d => d.OrderId)
        //        .OnDelete(DeleteBehavior.Cascade);
        //}
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

    }
}
