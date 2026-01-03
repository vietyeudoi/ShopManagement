using Microsoft.AspNetCore.Http;
using Sieu_Thi_Mini.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Sieu_Thi_Mini.Helpers
{
    public static class CartHelper
    {
        public static int GetCartCount(this HttpContext context)
        {
            var json = context.Session.GetString("CART");
            if (string.IsNullOrEmpty(json)) return 0;

            var cart = JsonSerializer.Deserialize<List<CartItem>>(json);
            return cart?.Sum(x => x.Quantity) ?? 0;
        }
    }
}
