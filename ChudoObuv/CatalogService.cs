using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using ChudoObuv.Models;

namespace ChudoObuv;

public static class CatalogService
{
    private const int LowStockLimit = 3;
    private const int ManyPairsLimit = 5;

    public static List<CatalogItem> GetCatalog()
    {
        var today = DateTime.Today;
        var thisMonthStart = new DateOnly(today.Year, today.Month, 1);
        var lastMonthStart = thisMonthStart.AddMonths(-1);

        using var db = new AppDbContext();
        var rows = db.Products.Select(p => new
        {
            p.Image,
            p.ProductName,
            p.Description,
            p.Composition,
            p.Price,
            Manufacturer = p.Manufacturer.ManufacturerName,
            Category = p.Category.CategoryName,
            Total = p.StockItems.Sum(s => s.Quantity),
            HadOrdersLastMonth = p.StockItems.Any(s => s.OrderItems.Any(oi =>
                oi.Order.OrderDate >= lastMonthStart && oi.Order.OrderDate < thisMonthStart))
        }).ToList();

        return rows.Select(r =>
        {
            var finalPrice = PriceCalculator.GetPriceWithDiscount(r.Price, r.HadOrdersLastMonth);
            return new CatalogItem
            {
                ImagePath = GetImagePath(r.Image),
                ProductName = r.ProductName,
                Title = $"{r.Manufacturer} | {r.ProductName}",
                Description = r.Description ?? "",
                Category = r.Category,
                QuantityText = r.Total > ManyPairsLimit ? "много" : "мало",
                Composition = r.Composition ?? "",
                OldPrice = r.Price,
                FinalPrice = finalPrice,
                HasDiscount = finalPrice < r.Price,
                RowBackground = r.Total <= LowStockLimit
                    ? new SolidColorBrush(Color.FromRgb(255, 201, 201))
                    : Brushes.White
            };
        }).ToList();
    }

    public static List<string> GetCategoryNames()
    {
        using var db = new AppDbContext();
        return db.Categories.OrderBy(c => c.CategoryName).Select(c => c.CategoryName).ToList();
    }

    private static string GetImagePath(string? imageName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Images", imageName ?? "");
        return File.Exists(path) ? path : Path.Combine(AppContext.BaseDirectory, "picture.png");
    }
}