using System.Windows.Media;

namespace ChudoObuv;

public class CatalogItem
{
    public string ImagePath { get; init; } = "";
    public int ProductId { get; init; }
    public string ProductName { get; init; } = "";
    public string Title { get; init; } = "";
    public string Description { get; init; } = "";
    public string Category { get; init; } = "";
    public string QuantityText { get; init; } = "";
    public string Composition { get; init; } = "";
    public decimal OldPrice { get; init; }
    public decimal FinalPrice { get; init; }
    public bool HasDiscount { get; init; }
    public Brush RowBackground { get; init; } = Brushes.White;
}