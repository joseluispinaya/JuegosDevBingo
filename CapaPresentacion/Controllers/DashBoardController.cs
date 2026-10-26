using Microsoft.AspNetCore.Mvc;

namespace CapaPresentacion.Controllers
{
    public class DashBoardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
