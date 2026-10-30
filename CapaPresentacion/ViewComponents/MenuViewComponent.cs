using CapaPresentacion.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CapaPresentacion.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            UserVM userLog = new();

            // Obtenemos el usuario de la petición web actual (como en tu guía)
            ClaimsPrincipal usuarioActual = HttpContext.User;

            // Usamos el operador seguro ?. para evitar errores si viene nulo
            if (usuarioActual?.Identity?.IsAuthenticated == true)
            {
                userLog.IdRol = UserClaimsPrincipal.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
                userLog.Correo = UserClaimsPrincipal.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
            }

            // Enviamos el modelo fuertemente tipado a la vista
            return View(userLog);
        }
    }
}
