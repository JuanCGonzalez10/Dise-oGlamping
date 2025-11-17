using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OmniPyme.Web.Data.Entities;

namespace OmniPyme.Data
{
    public class DataContext : IdentityDbContext<Users>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        // DbSets
        public DbSet<Client> Clients { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleDetail> SaleDetails { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        public DbSet<PrivateURole> PrivateURoles  { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            ConfigureKeys(builder);
            base.OnModelCreating(builder);

            ConfigureIndexes(builder);
            ConfigureRolePermissionRelations(builder);
            ConfigureUserToRoleRelation(builder);

            // Índice único para DNI
            builder.Entity<Client>()
                .HasIndex(c => c.DNI)
                .IsUnique();
        }

        private void ConfigureKeys(ModelBuilder builder)
        {
            // ROle Permissions
            builder.Entity<RolePermission>().HasKey(rp => new { rp.Roleid, rp.permissionId });

            builder.Entity<RolePermission>().HasOne(rp => rp.Role)
                                            .WithMany(r => r.RolePermissions)
                                            .HasForeignKey(rp => rp.Roleid);


        }

        private void ConfigureIndexes(ModelBuilder builder)
        {
            //ROle
            builder.Entity<PrivateURole>().HasIndex(r => r.Name).IsUnique();
            //User
            builder.Entity<Users>().HasIndex(r => r.Document).IsUnique();
        }

        private void ConfigureRolePermissionRelations(ModelBuilder builder)
        {
            builder.Entity<RolePermission>()
                .HasKey(rp => new { rp.Roleid, rp.permissionId });

            builder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.Roleid);

            builder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.permissionId);
        }

        private void ConfigureUserToRoleRelation(ModelBuilder builder)
        {
            // Evita el conflicto de cascadas múltiples
            builder.Entity<Users>()
                .HasOne(u => u.PrivateURole)
                .WithMany()
                .HasForeignKey(u => u.PrivateURoleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
