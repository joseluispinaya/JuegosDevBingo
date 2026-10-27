using Microsoft.AspNetCore.Mvc;

namespace CapaPresentacion.Controllers
{
    public class JugarController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
