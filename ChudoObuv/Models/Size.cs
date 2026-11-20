using System;
using System.Collections.Generic;

namespace ChudoObuv.Models;

public partial class Size
{
    public int SizeId { get; set; }

    public decimal SizeValue { get; set; }

    public virtual ICollection<StockItem> StockItems { get; set; } = new List<StockItem>();
}
