using System;

namespace ChudoObuv;

public static class PriceCalculator
{
    private const decimal DiscountRate = 0.25m;

    public static decimal GetPriceWithDiscount(decimal price, bool hadOrdersLastMonth)
    {
        return hadOrdersLastMonth ? price : Math.Round(price * (1 - DiscountRate), 2);
    }
}