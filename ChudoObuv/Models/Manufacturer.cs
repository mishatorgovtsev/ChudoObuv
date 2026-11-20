using System;
using System.Collections.Generic;

namespace ChudoObuv.Models;

public partial class Manufacturer
{
    public int ManufacturerId { get; set; }

    public string ManufacturerName { get; set; } = null!;

    public string? City { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
