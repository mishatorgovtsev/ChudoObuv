using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ChudoObuv.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Manufacturer> Manufacturers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Size> Sizes { get; set; }

    public virtual DbSet<StockItem> StockItems { get; set; }

    public virtual DbSet<Subcategory> Subcategories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VwOrderTotal> VwOrderTotals { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=CHUDO_OBUV;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(e => e.CategoryId)
                .ValueGeneratedNever()
                .HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(100);
        });

        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.Property(e => e.ManufacturerId)
                .ValueGeneratedNever()
                .HasColumnName("ManufacturerID");
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.ManufacturerName).HasMaxLength(150);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.OrderId)
                .ValueGeneratedNever()
                .HasColumnName("OrderID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Users");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.Property(e => e.OrderItemId)
                .ValueGeneratedNever()
                .HasColumnName("OrderItemID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.SizeId).HasColumnName("SizeID");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItems_Orders");

            entity.HasOne(d => d.StockItem).WithMany(p => p.OrderItems)
                .HasForeignKey(d => new { d.ProductId, d.SizeId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItems_StockItems");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.ProductId)
                .ValueGeneratedNever()
                .HasColumnName("ProductID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.Composition).HasMaxLength(500);
            entity.Property(e => e.Image).HasMaxLength(100);
            entity.Property(e => e.ManufacturerId).HasColumnName("ManufacturerID");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ProductName).HasMaxLength(300);
            entity.Property(e => e.SubcategoryId).HasColumnName("SubcategoryID");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Categories");

            entity.HasOne(d => d.Manufacturer).WithMany(p => p.Products)
                .HasForeignKey(d => d.ManufacturerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Manufacturers");

            entity.HasOne(d => d.Subcategory).WithMany(p => p.Products)
                .HasForeignKey(d => d.SubcategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Subcategories");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.RoleId)
                .ValueGeneratedNever()
                .HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(100);
        });

        modelBuilder.Entity<Size>(entity =>
        {
            entity.Property(e => e.SizeId)
                .ValueGeneratedNever()
                .HasColumnName("SizeID");
            entity.Property(e => e.SizeValue).HasColumnType("decimal(3, 1)");
        });

        modelBuilder.Entity<StockItem>(entity =>
        {
            entity.HasKey(e => new { e.ProductId, e.SizeId });

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.SizeId).HasColumnName("SizeID");

            entity.HasOne(d => d.Product).WithMany(p => p.StockItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockItems_Products");

            entity.HasOne(d => d.Size).WithMany(p => p.StockItems)
                .HasForeignKey(d => d.SizeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockItems_Sizes");
        });

        modelBuilder.Entity<Subcategory>(entity =>
        {
            entity.Property(e => e.SubcategoryId)
                .ValueGeneratedNever()
                .HasColumnName("SubcategoryID");
            entity.Property(e => e.SubcategoryName).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Login, "UQ_Users_Login").IsUnique();

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("UserID");
            entity.Property(e => e.Login).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Patronymic).HasMaxLength(100);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Surname).HasMaxLength(100);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        modelBuilder.Entity<VwOrderTotal>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_OrderTotals");

            entity.Property(e => e.ClientFullName).HasMaxLength(302);
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
