using System;
using System.Collections.Generic;

namespace CatalogAPI.Models;

public partial class ProductCategory
{
    public short ProductCategoryId { get; set; }

    public string ProductCategoryName { get; set; } = null!;

    public bool ProductCategoryActive { get; set; }

    public bool ProductCategoryDeleted { get; set; }

}
