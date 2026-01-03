using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sieu_Thi_Mini.Models;

public partial class Category
{
    [Key]
    public int CategoryId { get; set; }
    [Display(Name = "Tên danh mục")]
    [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]

    [StringLength(100)]
    public string CategoryName { get; set; } = null!;

    [StringLength(255)]
    [Display(Name = "Mô tả")]
    public string? Description { get; set; }

    [Display(Name = "Trạng thái hoạt động")]
    public bool IsActive { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
