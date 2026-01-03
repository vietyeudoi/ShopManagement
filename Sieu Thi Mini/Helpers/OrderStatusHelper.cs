using Sieu_Thi_Mini.Models;

namespace Sieu_Thi_Mini.Helpers
{
    public static class OrderStatusHelper
    {
        // Hiển thị tiếng Việt
        public static string ToVietnamese(this Order.OrderStatus status)
        {
            return status switch
            {
                Order.OrderStatus.Pending => "Chờ xác nhận",
                Order.OrderStatus.Confirmed => "Đã xác nhận",
                Order.OrderStatus.InProgress => "Đang giao",
                Order.OrderStatus.Completed => "Hoàn thành",
                Order.OrderStatus.Cancelled => "Huỷ",
                _ => "Không xác định"
            };
        }

        // Badge bootstrap
        public static string ToBadge(this Order.OrderStatus status)
        {
            return status switch
            {
                Order.OrderStatus.Pending => "badge bg-secondary",        // Chờ xác nhận (xám)
                Order.OrderStatus.Confirmed => "badge bg-primary",          // Đã xác nhận (xanh dương)
                Order.OrderStatus.InProgress => "badge bg-info text-dark",   // Đang giao (xanh nhạt)
                Order.OrderStatus.Completed => "badge bg-success",          // Thành công (xanh lá)
                Order.OrderStatus.Cancelled => "badge bg-danger",           // Đã hủy (đỏ)
                _ => "badge bg-dark"
            };
        }
    }
}
