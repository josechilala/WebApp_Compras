using Microsoft.AspNetCore.Mvc;

namespace WebAPP_Compras.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
