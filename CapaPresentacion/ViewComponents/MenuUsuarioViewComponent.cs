using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CapaPresentacion.ViewComponents
{
    public class MenuUsuarioViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var nombreCompleto = string.Empty;
            var correoUsuario = string.Empty;

            // Obtenemos el usuario de la petición web actual (como en tu guía)
            ClaimsPrincipal usuarioActual = HttpContext.User;

            // Usamos el operador seguro ?. para evitar errores si viene nulo
            if (usuarioActual?.Identity?.IsAuthenticated == true)
            {
                nombreCompleto = usuarioActual.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
                correoUsuario = usuarioActual.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
            }

            ViewData["nombreCompleto"] = nombreCompleto;
            ViewData["correoUsuario"] = correoUsuario;

            return View();
        }
    }
}
