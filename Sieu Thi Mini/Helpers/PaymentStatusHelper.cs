using Sieu_Thi_Mini.Models;

namespace Sieu_Thi_Mini.Helpers
{
    public static class PaymentStatusHelper
    {
        public static string ToVietnamese(this Payment.PaymentStatusEnum status)
        {
            return status switch
            {
                Payment.PaymentStatusEnum.Pending => "Chưa thanh toán",
                Payment.PaymentStatusEnum.Paid => "Đã thanh toán",
                Payment.PaymentStatusEnum.Failed => "Thanh toán lỗi",
                _ => status.ToString()
            };
        }

        public static string ToBadge(this Payment.PaymentStatusEnum status)
        {
            return status switch
            {
                Payment.PaymentStatusEnum.Pending => "bg-warning",
                Payment.PaymentStatusEnum.Paid => "bg-success",
                Payment.PaymentStatusEnum.Failed => "bg-danger",
                _ => "bg-secondary"
            };
        }
    }
}
