using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SneakersShop.Domain.Brands;
using SneakersShop.Domain.Carts;
using SneakersShop.Domain.Catalog;
using SneakersShop.Domain.Common.Entities;
using SneakersShop.Domain.Consumer;
using SneakersShop.Domain.Orders;
using SneakersShop.Domain.Warehouse;
using SneakersShop.Infrastructure.Persistence.Identity;

namespace SneakersShop.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<WarehouseItem> WarehouseItems => Set<WarehouseItem>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        IEnumerable<Type> aggregateTypes = modelBuilder.Model
            .GetEntityTypes()
            .Select(e => e.ClrType)
            .Where(t => !t.IsAbstract && t.IsAssignableTo(typeof(AggregateRoot)));

        foreach (Type aggregateType in aggregateTypes)
            modelBuilder.Entity(aggregateType).Ignore(nameof(AggregateRoot.DomainEvents));

        IEnumerable<Type> entityBaseTypes = modelBuilder.Model
            .GetEntityTypes()
            .Select(e => e.ClrType)
            .Where(t => !t.IsAbstract && t.IsAssignableTo(typeof(EntityBase)));

        foreach (Type entityType in entityBaseTypes)
        {
            EntityTypeBuilder b = modelBuilder.Entity(entityType);
            b.Property(nameof(EntityBase.Id)).ValueGeneratedNever();
            b.Property(nameof(EntityBase.CreatedAt)).IsRequired();
            b.Property(nameof(EntityBase.UpdatedAt)).IsRequired(false);
        }
    }
}