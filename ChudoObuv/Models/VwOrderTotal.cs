using System;
using System.Collections.Generic;

namespace ChudoObuv.Models;

public partial class VwOrderTotal
{
    public int OrderId { get; set; }

    public DateOnly OrderDate { get; set; }

    public string ClientFullName { get; set; } = null!;

    public double? TotalAmount { get; set; }
}
