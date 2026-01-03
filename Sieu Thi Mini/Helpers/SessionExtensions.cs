using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Sieu_Thi_Mini.Helpers
{
    public static class SessionExtensions
    {
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            // Thêm options để hỗ trợ tốt hơn cho việc định dạng JSON
            var options = new JsonSerializerOptions { WriteIndented = true };
            session.SetString(key, JsonSerializer.Serialize(value, options));
        }

        public static T? GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            // Nếu giá trị null thì trả về giá trị mặc định của kiểu T
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}