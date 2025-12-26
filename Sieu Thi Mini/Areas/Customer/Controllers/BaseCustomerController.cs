using Microsoft.AspNetCore.Mvc;
using Sieu_Thi_Mini.Filters;

namespace Sieu_Thi_Mini.Areas.Customer.Controllers
{
    [Area("Customer")]
    [RoleAuthorize("Customer")]
    public abstract class BaseCustomerController : Controller
    {
    }
}
