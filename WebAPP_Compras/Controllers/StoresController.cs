using Microsoft.AspNetCore.Mvc;

namespace WebAPP_Compras.Controllers
{
    public class StoresController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
