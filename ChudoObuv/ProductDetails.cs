using System.Collections.Generic;

namespace ChudoObuv;

public class ProductSize
{
    public int SizeId { get; init; }
    public decimal SizeValue { get; init; }
    public int Quantity { get; init; }
    public string DisplayText => $"{SizeValue:0.#} (в наличии: {Quantity})";
}

public class ProductDetails
{
    public int ProductId { get; init; }
    public string ImagePath { get; init; } = "";
    public string Manufacturer { get; init; } = "";
    public string ProductName { get; init; } = "";
    public string Category { get; init; } = "";
    public string Composition { get; init; } = "";
    public string Description { get; init; } = "";
    public decimal OldPrice { get; init; }
    public decimal FinalPrice { get; init; }
    public bool HasDiscount { get; init; }
    public List<ProductSize> Sizes { get; init; } = new();
}