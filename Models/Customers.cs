using System;
using System.Collections.Generic;

namespace TeamHAMM.Models;

public partial class Customers
{
    public int CustomerId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual ICollection<Orders> Orders { get; set; } = new List<Orders>();
}
