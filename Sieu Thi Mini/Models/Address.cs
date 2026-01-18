using System;
using System.Collections.Generic;

namespace Sieu_Thi_Mini.Models;

public partial class Address
{
    public int AddressId { get; set; }

    public int CustomerId { get; set; }

    public string? Address1 { get; set; }

    public virtual Customer Customer { get; set; } = null!;
}
