using System;
using System.Collections.Generic;

namespace CatalogAPI.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string ProductDescription { get; set; } = null!;

    public short ProductCategoryId { get; set; }

    public byte[]? ProductImage { get; set; } = null!;

    public virtual ProductCategory? ProductCategory { get; set; } = null!;
}
