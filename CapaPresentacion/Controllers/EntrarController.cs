using CapaData.Interfaces;
using CapaPresentacion.Helpers;
using CapaPresentacion.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CapaPresentacion.Controllers
{
    public class EntrarController : Controller
    {
        private readonly IUsuarioRepositorio _repositorio;
        private readonly IPasswordHelper _passwordHelper;
        public EntrarController(IUsuarioRepositorio repositorio, IPasswordHelper passwordHelper)
        {
            _repositorio = repositorio;
            _passwordHelper = passwordHelper;
        }

        public IActionResult Login()
        {
            // Verificamos de forma segura que Identity exista antes de preguntar si está autenticado
            //if (User.Identity?.IsAuthenticated == true) Verificamos de forma segura si el usuario ya tiene una sesión activa

            if (User.Identity is not null && User.Identity.IsAuthenticated)
            {
                // string? idRol = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                // return idRol switch
                // {
                //     "1" => RedirectToAction("Index", "Usuarios"),
                //     "2" => RedirectToAction("Index", "Jugar"),
                //     _ => RedirectToAction("Index", "Home")
                // };

                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(VMLogin modelo)
        {
            if (string.IsNullOrEmpty(modelo.Correo) || string.IsNullOrEmpty(modelo.Clave))
            {
                ViewData["Mensaje"] = "El correo y la contraseña son obligatorios.";
                return View();
            }

            var resp = await _repositorio.Login(modelo.Correo);

            if (!resp.Estado)
            {
                ViewData["Mensaje"] = "Correo o Contraseña incorrectos.";
                return View();
            }

            var objUsuario = resp.Data;

            // 1. Validar cuenta inactiva
            if (objUsuario!.Estado == false)
            {
                ViewData["Mensaje"] = "Su cuenta está inactiva. Contáctese con administración.";
                return View();
            }

            // 2. NUEVO: Validar Fecha de Vencimiento (Solo aplicable a Clientes - Rol 2)
            // Usamos UTC-4 para coincidir con la lógica que usaste en tus procedimientos SQL
            // DateTime fechaActual = DateTime.UtcNow.AddHours(-4); 
            // if (objUsuario.IdRol == 2 && objUsuario.FechaVencimiento < fechaActual.Date)
            // {
            //     ViewData["Mensaje"] = $"Su suscripción venció el {objUsuario.FechaVencimiento:dd/MM/yyyy}. Contáctese para renovar.";
            //     return View();
            // }

            // 3. Validar Hash de Contraseña
            bool passCorrecta = _passwordHelper.Verify(modelo.Clave, objUsuario.PasswordHash!);
            if (!passCorrecta)
            {
                ViewData["Mensaje"] = "Correo o Contraseña incorrectos.";
                return View();
            }

            // Crear Claims new Claim(ClaimTypes.Email, objUsuario.Correo),
            List<Claim> claims =
            [
                new Claim(ClaimTypes.NameIdentifier, objUsuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, objUsuario.NombreCompleto),
                new Claim(ClaimTypes.Email, objUsuario.Correo),
                //new Claim("Correo", objUsuario.Correo),
                new Claim(ClaimTypes.Role, objUsuario.IdRol.ToString())
            ];

            ClaimsIdentity claimsIdentity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new() { AllowRefresh = true };
            //AuthenticationProperties properties = new() { AllowRefresh = true, IsPersistent = modelo.MantenerSesion };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties
            );

            // 4. Redirección Inteligente por Rol
            // return objUsuario.IdRol switch
            // {
            //     1 => RedirectToAction("Index", "Usuarios"),
            //     2 => RedirectToAction("Index", "Jugar"),
            //     _ => RedirectToAction("Index", "Home")
            // };

            return RedirectToAction("Index", "Home");

        }

        public async Task<IActionResult> Salir()
        {
            // Destruye la cookie de sesión autenticada
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Redirecciona al Login
            return RedirectToAction("Login", "Entrar");
        }
    }
}
