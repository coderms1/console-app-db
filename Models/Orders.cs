using System;
using System.Collections.Generic;

namespace TeamHAMM.Models;

public partial class Orders
{
    public int OrderId { get; set; }

    public int CustomerId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public DateTime OrderDate { get; set; }

    public virtual Customers Customer { get; set; } = null!;

    public virtual Products Product { get; set; } = null!;
}
