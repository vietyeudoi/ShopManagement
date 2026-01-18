using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sieu_Thi_Mini.Models;

[Index("OrderId", Name = "UQ__Payments__C3905BCE71CEF21F", IsUnique = true)]
public partial class Payment
{
    [Key]
    public int PaymentId { get; set; }

    public int OrderId { get; set; }

    [StringLength(50)]
    public PaymentMethodEnum PaymentMethod { get; set; }

    public enum PaymentMethodEnum
    {
        COD,
        Bank,
        Cash

    }

    [Column(TypeName = "datetime")]
    public DateTime? PaymentDate { get; set; }

    [StringLength(50)]
    public PaymentStatusEnum PaymentStatus { get; set; } = PaymentStatusEnum.Pending;

    [ForeignKey("OrderId")]
    [InverseProperty("Payment")]
    public virtual Order Order { get; set; } = null!;

    public enum PaymentStatusEnum
    {
        Pending,    // Chưa thanh toán
        Paid,       // Đã thanh toán
        Failed      // Thất bại
    }
}