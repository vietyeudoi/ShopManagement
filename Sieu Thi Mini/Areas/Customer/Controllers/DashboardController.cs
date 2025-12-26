using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Sieu_Thi_Mini.Models;
using Sieu_Thi_Mini.Filters;

namespace Sieu_Thi_Mini.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Route("Customer/")]
    public class DashboardController : BaseCustomerController
    {

        public IActionResult Index()
        {
            return View();
        }

    }
}
