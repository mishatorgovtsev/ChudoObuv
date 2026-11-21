using System.Collections.Generic;
using System.Linq;

namespace ChudoObuv;

public class OrderLine
{
    public int ProductId { get; init; }
    public string ProductTitle { get; init; } = "";
    public int SizeId { get; init; }
    public decimal SizeValue { get; init; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; init; }
    public decimal Total => Quantity * UnitPrice;
}

public static class OrderCart
{
    private static readonly List<OrderLine> lines = new();

    public static IReadOnlyList<OrderLine> Lines => lines;

    public static int GetQuantity(int productId, int sizeId)
    {
        return lines.Where(l => l.ProductId == productId && l.SizeId == sizeId).Sum(l => l.Quantity);
    }

    public static void Add(OrderLine line)
    {
        var existing = lines.FirstOrDefault(l => l.ProductId == line.ProductId && l.SizeId == line.SizeId);
        if (existing == null)
        {
            lines.Add(line);
        }
        else
        {
            existing.Quantity += line.Quantity;
        }
    }

    public static void Remove(OrderLine line)
    {
        lines.Remove(line);
    }

    public static void Clear()
    {
        lines.Clear();
    }
}