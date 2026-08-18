using Microsoft.EntityFrameworkCore;

using SneakersShop.Domain.Brands;
using SneakersShop.Domain.Carts;
using SneakersShop.Domain.Catalog;
using SneakersShop.Domain.Consumer;
using SneakersShop.Domain.Orders;
using SneakersShop.Domain.Warehouse;

namespace SneakersShop.Application.Abstractions.ApplicatonDbContext;

public interface IApplicationDbContext
{
    public DbSet<Brand> Brands { get; }
    public DbSet<Cart> Carts { get; }
    public DbSet<Product> Products { get; }
    public DbSet<Order> Orders { get; }
    public DbSet<WarehouseItem> WarehouseItems { get; }
    public DbSet<UserProfile> UserProfiles { get; }
    public DbSet<Category> Categories { get; }
    public DbSet<ProductVariant> ProductVariants { get; }
}