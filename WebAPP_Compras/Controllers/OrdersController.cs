using Microsoft.AspNetCore.Mvc;

namespace WebAPP_Compras.Controllers
{
    public class OrdersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
