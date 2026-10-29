using CapaData.Interfaces;
using CapaPresentacion.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace CapaPresentacion.Controllers
{
    public class AccesoController : Controller
    {
        private readonly IUsuarioRepositorio _repositorio;
        private readonly IPasswordHelper _passwordHelper;
        public AccesoController(IUsuarioRepositorio repositorio, IPasswordHelper passwordHelper)
        {
            _repositorio = repositorio;
            _passwordHelper = passwordHelper;
        }

        public IActionResult Login()
        {
            return View();
        }
    }
}
