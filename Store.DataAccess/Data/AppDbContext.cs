using Microsoft.EntityFrameworkCore;
using Store.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.DataAccess.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
            modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<ProductImage>().HasQueryFilter(pi => !pi.IsDeleted);
            modelBuilder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<Order>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<OrderItem>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<Payment>().HasQueryFilter(c => !c.IsDeleted);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // 1. نبحث عن كل الكيانات التي يتم تتبعها حالياً وتطبق واجهة IBaseEntity
            var entries = ChangeTracker.Entries<IBaseEntity>();

            foreach (var entry in entries)
            {
                // 2. إذا كان الكيان جديداً (تمت إضافته للتو)
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow; // أو DateTime.Now حسب تفضيلك
                }
                // 3. إذا كان الكيان قديماً ولكن تم تعديله
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }

            // 4. أخيراً، اجعل EF Core يكمل عمله الطبيعي ويحفظ في الداتا بيز
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
