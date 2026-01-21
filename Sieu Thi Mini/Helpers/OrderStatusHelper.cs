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
                Order.OrderStatus.Pending => "badge bg-secondary",        
                Order.OrderStatus.Confirmed => "badge bg-primary",          
                Order.OrderStatus.InProgress => "badge bg-info text-dark",   
                Order.OrderStatus.Completed => "badge bg-success",          
                Order.OrderStatus.Cancelled => "badge bg-danger",         
                _ => "badge bg-dark"
            };
        }
    }
}
