using HA_Ossooll.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HA_Ossooll.Data.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Storage> Storages { get; set; }
        public DbSet<Maintenance> Maintenances { get; set; }
        public DbSet<Operation> Operations { get; set; }
        public DbSet<OperationType> OperationTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRole");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaim");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogin");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserToken");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaim");

            builder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Employee>()
                .HasOne(e => e.Manager)
                .WithMany(e => e.Subordinates)
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Department>()
                .HasOne(d => d.Manager)
                .WithMany()
                .HasForeignKey(d => d.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
                .HasOne(p => p.Storage)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.StorageId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Product>()
                .HasOne(p => p.ProductType)
                .WithMany(pt => pt.Products)
                .HasForeignKey(p => p.ProductTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Maintenance>()
                .HasOne(m => m.Storage)
                .WithMany(s => s.Maintenances)
                .HasForeignKey(m => m.StorageId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Operation>()
                .HasOne(o => o.OperationType)
                .WithMany(ot => ot.Operations)
                .HasForeignKey(o => o.OperationTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
