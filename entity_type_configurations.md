# Entity Framework Core Entity Type Configurations

This document contains all the `IEntityTypeConfiguration<T>` implementations for the domain entities of **SimplePos**. 

These configurations define table names, primary keys, foreign key constraints, decimal precision, backing field access modes for encapsulated collections, and global soft-delete query filters.

---

## 1. Users & Permissions Domain

### `PermissionConfiguration`
- **Table**: `permissions`
- **Primary Key**: `PermissionId`
- **Unique Index**: `Name`

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePos.Domain.Permissions;

namespace SimplePos.Infrastructure.Persistence.Permissions;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");

        builder.HasKey(p => p.PermissionId);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(p => p.Name)
            .IsUnique();
    }
}
```

### `UserPermissionConfiguration`
- **Table**: `user_permissions`
- **Primary Key**: Composite (`UserId`, `PermissionId`)
- **Foreign Keys**: 
  - `UserId` -> `User` (`OnDelete: Cascade`)
  - `PermissionId` -> `Permission` (`OnDelete: Restrict`)

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePos.Domain.Permissions;
using SimplePos.Domain.Users;

namespace SimplePos.Infrastructure.Persistence.Users;

public class UserPermissionConfiguration : IEntityTypeConfiguration<UserPermission>
{
    public void Configure(EntityTypeBuilder<UserPermission> builder)
    {
        builder.ToTable("user_permissions");

        // Composite primary key for join table
        builder.HasKey(up => new { up.UserId, up.PermissionId });

        builder.HasOne<User>()
            .WithMany(u => u.UserPermissions)
            .HasForeignKey(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Permission>()
            .WithMany()
            .HasForeignKey(up => up.PermissionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

---

## 2. Categories Domain

### `CategoryConfiguration`
- **Table**: `categories`
- **Primary Key**: `CategoryId`
- **Foreign Keys**: `CompanyId` -> `Company` (`OnDelete: Restrict`)
- **Global Filter**: `!SoftDeleted`

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePos.Domain.Categories;
using SimplePos.Domain.Companies;

namespace SimplePos.Infrastructure.Persistence.Categories;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");

        builder.HasKey(c => c.CategoryId);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.IsActive)
            .IsRequired();

        builder.Property(c => c.SoftDeleted)
            .IsRequired();

        builder.Property(c => c.DateTimeSoftDeleted);

        // FK: Category belongs to Company
        builder.HasOne<Company>()
            .WithMany()
            .HasForeignKey(c => c.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Soft delete global filter
        builder.HasQueryFilter(c => !c.SoftDeleted);
    }
}
```

---

## 3. Taxes Domain

### `TaxConfiguration`
- **Table**: `taxes`
- **Primary Key**: `TaxId`
- **Foreign Keys**: `CompanyId` -> `Company` (`OnDelete: Restrict`)
- **Decimal Precision**: `TaxRate` -> `decimal(5, 4)`
- **Global Filter**: `!SoftDeleted`

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePos.Domain.Companies;
using SimplePos.Domain.Taxes;

namespace SimplePos.Infrastructure.Persistence.Taxes;

public class TaxConfiguration : IEntityTypeConfiguration<Tax>
{
    public void Configure(EntityTypeBuilder<Tax> builder)
    {
        builder.ToTable("taxes");

        builder.HasKey(t => t.TaxId);

        builder.Property(t => t.TaxName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.TaxRate)
            .HasPrecision(5, 4) // e.g., 0.1000 for 10%
            .IsRequired();

        builder.Property(t => t.IsActive)
            .IsRequired();

        builder.Property(t => t.SoftDeleted)
            .IsRequired();

        builder.Property(t => t.DateTimeSoftDeleted);

        // FK: Tax belongs to Company
        builder.HasOne<Company>()
            .WithMany()
            .HasForeignKey(t => t.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(t => !t.SoftDeleted);
    }
}
```

---

## 4. Products Domain

### `ProductConfiguration`
- **Table**: `products`
- **Primary Key**: `ProductId`
- **Foreign Keys**:
  - `CompanyId` -> `Company` (`OnDelete: Restrict`)
  - `CategoryId` -> `Category` (`OnDelete: Restrict`)
- **Decimal Precision**: `CostPrice` (`18, 2`), `BasePrice` (`18, 2`)
- **Backing Field Access**: `_productTaxes` collection set to `PropertyAccessMode.Field`
- **Global Filter**: `!SoftDeleted`

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePos.Domain.Categories;
using SimplePos.Domain.Companies;
using SimplePos.Domain.Products;

namespace SimplePos.Infrastructure.Persistence.Products;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.HasKey(p => p.ProductId);

        builder.Property(p => p.SKU)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.ProductName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.CostPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.BasePrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.IsActive)
            .IsRequired();

        builder.Property(p => p.SoftDeleted)
            .IsRequired();

        builder.Property(p => p.DateTimeSoftDeleted);

        // Foreign Keys
        builder.HasOne<Company>()
            .WithMany()
            .HasForeignKey(p => p.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure private field access for _productTaxes collection
        builder.Metadata.FindNavigation(nameof(Product.ProductTaxes))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasQueryFilter(p => !p.SoftDeleted);
    }
}
```

### `ProductTaxConfiguration`
- **Table**: `product_taxes`
- **Primary Key**: Composite (`ProductId`, `TaxId`)
- **Foreign Keys**:
  - `ProductId` -> `Product` (`OnDelete: Cascade`)
  - `TaxId` -> `Tax` (`OnDelete: Restrict`)

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePos.Domain.Products;
using SimplePos.Domain.Taxes;

namespace SimplePos.Infrastructure.Persistence.Products;

public class ProductTaxConfiguration : IEntityTypeConfiguration<ProductTax>
{
    public void Configure(EntityTypeBuilder<ProductTax> builder)
    {
        builder.ToTable("product_taxes");

        // Composite primary key
        builder.HasKey(pt => new { pt.ProductId, pt.TaxId });

        builder.HasOne<Product>()
            .WithMany(p => p.ProductTaxes)
            .HasForeignKey(pt => pt.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Tax>()
            .WithMany()
            .HasForeignKey(pt => pt.TaxId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

---

## 5. Payments Domain

### `PaymentMethodConfiguration`
- **Table**: `payment_methods`
- **Primary Key**: `PaymentMethodId`
- **Foreign Keys**: `CompanyId` -> `Company` (`OnDelete: Restrict`)
- **Global Filter**: `!SoftDeleted`

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePos.Domain.Companies;
using SimplePos.Domain.Payments;

namespace SimplePos.Infrastructure.Persistence.Payments;

public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.ToTable("payment_methods");

        builder.HasKey(pm => pm.PaymentMethodId);

        builder.Property(pm => pm.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(pm => pm.IsActive)
            .IsRequired();

        builder.Property(pm => pm.SoftDeleted)
            .IsRequired();

        builder.Property(pm => pm.DateTimeSoftDeleted);

        builder.HasOne<Company>()
            .WithMany()
            .HasForeignKey(pm => pm.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(pm => !pm.SoftDeleted);
    }
}
```

---

## 6. Sales Domain

### `SaleConfiguration`
- **Table**: `sales`
- **Primary Key**: `SaleId`
- **Foreign Keys**: `OutletId` -> `Outlet` (`OnDelete: Restrict`)
- **Ignored Property**: `IsFullyPaid` (Computed read-only domain logic)
- **Decimal Precision**: Monetary values set to `18, 2`
- **Backing Field Access**: `_saleItems` & `_salePayments` collections set to `PropertyAccessMode.Field`
- **Global Filter**: `!SoftDeleted`

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePos.Domain.Outlets;
using SimplePos.Domain.Sales;

namespace SimplePos.Infrastructure.Persistence.Sales;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("sales");

        builder.HasKey(s => s.SaleId);

        builder.Property(s => s.InvoiceNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.SubTotal).HasPrecision(18, 2).IsRequired();
        builder.Property(s => s.TaxAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(s => s.NetAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(s => s.TotalAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(s => s.TotalPaid).HasPrecision(18, 2).IsRequired();
        builder.Property(s => s.TotalChange).HasPrecision(18, 2).IsRequired();
        builder.Property(s => s.TotalOutstanding).HasPrecision(18, 2).IsRequired();
        builder.Property(s => s.TotalDiscount).HasPrecision(18, 2).IsRequired();

        builder.Property(s => s.DateTimeCreated).IsRequired();
        builder.Property(s => s.Void).IsRequired();
        builder.Property(s => s.SoftDeleted).IsRequired();
        builder.Property(s => s.DateTimeSoftDeleted);

        // Ignore computed Domain property
        builder.Ignore(s => s.IsFullyPaid);

        // FK: Sale belongs to Outlet
        builder.HasOne<Outlet>()
            .WithMany()
            .HasForeignKey(s => s.OutletId)
            .OnDelete(DeleteBehavior.Restrict);

        // Access back-field collections
        builder.Metadata.FindNavigation(nameof(Sale.SaleItems))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata.FindNavigation(nameof(Sale.SalePayments))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasQueryFilter(s => !s.SoftDeleted);
    }
}
```

### `SaleItemConfiguration`
- **Table**: `sale_items`
- **Primary Key**: `SaleItemId`
- **Foreign Keys**:
  - `SaleId` -> `Sale` (`OnDelete: Cascade`)
  - `ProductId` -> `Product` (`OnDelete: Restrict`)
- **Decimal Precision**: `Quantity` (`18, 3`), `UnitDiscount` (`18, 4`), `TaxRate` (`5, 4`), Amount fields (`18, 2`)

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePos.Domain.Products;
using SimplePos.Domain.Sales;

namespace SimplePos.Infrastructure.Persistence.Sales;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("sale_items");

        builder.HasKey(si => si.SaleItemId);

        builder.Property(si => si.Quantity).HasPrecision(18, 3).IsRequired();
        builder.Property(si => si.UnitPrice).HasPrecision(18, 2).IsRequired();
        builder.Property(si => si.GrossAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(si => si.UnitDiscount).HasPrecision(18, 4).IsRequired();
        builder.Property(si => si.DiscountedUnitPrice).HasPrecision(18, 2).IsRequired();
        builder.Property(si => si.NetAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(si => si.TotalDiscount).HasPrecision(18, 2).IsRequired();
        builder.Property(si => si.TotalLineAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(si => si.TaxRate).HasPrecision(5, 4).IsRequired();
        builder.Property(si => si.TaxAmount).HasPrecision(18, 2).IsRequired();

        builder.Property(si => si.Remark).HasMaxLength(500);

        // Relationships
        builder.HasOne<Sale>()
            .WithMany(s => s.SaleItems)
            .HasForeignKey(si => si.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(si => si.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
        // Configure private field access for _saleItemTaxes collection
        builder.Metadata.FindNavigation(nameof(SaleItem.SaleItemTaxes))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
```

### `SalePaymentConfiguration`
- **Table**: `sale_payments`
- **Primary Key**: `SalePaymentId`
- **Foreign Keys**:
  - `SaleId` -> `Sale` (`OnDelete: Cascade`)
  - `PaymentMethodId` -> `PaymentMethod` (`OnDelete: Restrict`)
- **Decimal Precision**: `AmountPaid` (`18, 2`)

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePos.Domain.Payments;
using SimplePos.Domain.Sales;

namespace SimplePos.Infrastructure.Persistence.Sales;

public class SalePaymentConfiguration : IEntityTypeConfiguration<SalePayment>
{
    public void Configure(EntityTypeBuilder<SalePayment> builder)
    {
        builder.ToTable("sale_payments");

        builder.HasKey(sp => sp.SalePaymentId);

        builder.Property(sp => sp.AmountPaid).HasPrecision(18, 2).IsRequired();
        builder.Property(sp => sp.PaymentDate).IsRequired();
        builder.Property(sp => sp.ReferenceNumber).HasMaxLength(100);

        // Relationships
        builder.HasOne<Sale>()
            .WithMany(s => s.SalePayments)
            .HasForeignKey(sp => sp.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<PaymentMethod>()
            .WithMany()
            .HasForeignKey(sp => sp.PaymentMethodId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

### `SaleItemTaxConfiguration`
- **Table**: `sale_item_taxes`
- **Primary Key**: `SaleItemTaxId`
- **Foreign Keys**:
  - `SaleItemId` -> `SaleItem` (`OnDelete: Cascade`)
  - `TaxId` -> `Tax` (`OnDelete: Restrict`)
- **Decimal Precision**: `TaxRate` (`5, 4`), `TaxAmount` (`18, 2`)

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePos.Domain.Sales;
using SimplePos.Domain.Taxes;

namespace SimplePos.Infrastructure.Persistence.Sales;

public class SaleItemTaxConfiguration : IEntityTypeConfiguration<SaleItemTax>
{
    public void Configure(EntityTypeBuilder<SaleItemTax> builder)
    {
        builder.ToTable("sale_item_taxes");

        builder.HasKey(sit => sit.SaleItemTaxId);

        builder.Property(sit => sit.TaxName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(sit => sit.TaxRate)
            .HasPrecision(5, 4)
            .IsRequired();

        builder.Property(sit => sit.TaxAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        // Foreign Keys
        builder.HasOne<SaleItem>()
            .WithMany(si => si.SaleItemTaxes)
            .HasForeignKey(sit => sit.SaleItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Tax>()
            .WithMany()
            .HasForeignKey(sit => sit.TaxId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

