using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;

namespace Sieu_Thi_Mini.Models;

[Index("Email", Name = "UQ__Customer__A9D10534554E8A5C", IsUnique = true)]
public partial class Customer
{
    [Key]
    public int CustomerId { get; set; }

    [StringLength(100)]
    public string FullName { get; set; } = null!;

    [StringLength(100)]
    public string Email { get; set; } = null!;

    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(255)]
    public string Password { get; set; } = null!;

    [StringLength(255)]
    public string? Address { get; set; }

    public bool? IsActive { get; set; }

    [InverseProperty("Customer")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [NotMapped]
    public List<string> ListAdress { get; set; }
    
    public void JAdress()
    {
        Address = System.Text.Json.JsonSerializer.Serialize(ListAdress);
    }
    public void LoadAdress()
    {
        //ListAdress = System.Text.Json.JsonSerializer.Deserialize<List<string>>(Address) ?? new List<string>();
        ListAdress = string.IsNullOrEmpty(Address)
           ? new List<string>()
           : System.Text.Json.JsonSerializer.Deserialize<List<string>>(Address)
             ?? new List<string>();
    }

}
