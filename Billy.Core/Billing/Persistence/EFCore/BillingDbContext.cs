using Billy.Billing.Persistence.EFCore.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billy.Billing.Persistence.EFCore
{
    public class BillingDbContext : DbContext
    {
        public BillingDbContext([NotNull] DbContextOptions options) : base(options)
        {
        }

        public DbSet<EFCoreSupplier> Suppliers { get; }
        public DbSet<EFCoreBill> Bills { get; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EFCoreSupplier>(
                supplier =>
                {
                    supplier.HasKey(x => x.Id);
                    supplier.Property(x => x.Name).IsRequired();
                    supplier.HasIndex(x => x.Name).IsUnique();
                    // owned
                    supplier.OwnsOne(x => x.Address);
                    supplier.OwnsOne(x => x.Agent);
                    // foreign
                    supplier
                        .HasMany(x => x.Bills)
                        .WithOne(x => x.Supplier)
                        .HasForeignKey(x => x.SupplierId)
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity<EFCoreBill>(
                bill =>
                {
                    bill.HasKey(x => x.Id);
                    // unique
                    bill.HasIndex(x => new { x.SupplierId, x.Code }).IsUnique();
                    // required
                    bill.Property(x => x.AdditionalCosts).IsRequired();
                    bill.Property(x => x.Agio).IsRequired();
                    bill.Property(x => x.Amount).IsRequired();
                    bill.Property(x => x.Code).IsRequired();
                    bill.Property(x => x.DueDate).IsRequired();
                    bill.Property(x => x.RegistrationDate).IsRequired();
                    bill.Property(x => x.ReleaseDate).IsRequired();
                    bill.Property(x => x.SupplierId).IsRequired();
                });
        }
    }
}
