using CapaData.Interfaces;
using CapaEntidades;
using CapaEntidades.Responses;
using CapaPresentacion.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace CapaPresentacion.Controllers
{
    [Authorize(Roles = "1")] // Protege TODO el controlador automáticamente
    public class UsuariosController : Controller
    {
        private readonly IUsuarioRepositorio _repositorio;
        private readonly IPasswordHelper _passwordHelper;
        public UsuariosController(IUsuarioRepositorio repositorio, IPasswordHelper passwordHelper)
        {
            _repositorio = repositorio;
            _passwordHelper = passwordHelper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaUsuarios()
        {
            var rpt = await _repositorio.Lista();

            return StatusCode(StatusCodes.Status200OK, rpt);
        }

        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] Usuarios objeto)
        {
            try
            {
                if (!DateTime.TryParseExact(objeto.FechaVencimientoStr, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaVencimientoDate))
                {
                    Respuesta<int> rptFormato = new() { Estado = false, Valor = "warning", Data = 0, Mensaje = "El formato de la fecha no es válido. Debe ser dd/MM/yyyy." };
                    return StatusCode(StatusCodes.Status200OK, rptFormato);
                }

                objeto.IdRol = 2; // Forzamos rol de cliente
                objeto.PasswordHash = _passwordHelper.Hash(objeto.Celular); // Encriptamos celular por defecto
                objeto.FechaVencimiento = fechaVencimientoDate;

                var rptbd = await _repositorio.Guardar(objeto);
                return StatusCode(StatusCodes.Status200OK, rptbd);
            }
            catch (Exception ex)
            {
                Respuesta<int> rptEx = new() { Estado = false, Valor = "error", Data = 0, Mensaje = "Error en el servidor: " + ex.Message };
                return StatusCode(StatusCodes.Status200OK, rptEx);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] Usuarios objeto)
        {
            try
            {
                if (!DateTime.TryParseExact(objeto.FechaVencimientoStr, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaVencimientoDate))
                {
                    Respuesta<int> rptFormato = new() { Estado = false, Valor = "warning", Data = 0, Mensaje = "El formato de la fecha no es válido. Debe ser dd/MM/yyyy." };
                    return StatusCode(StatusCodes.Status200OK, rptFormato);
                }

                objeto.IdRol = 2; // Aseguramos que siga siendo cliente
                                  // En la edición no tocamos el PasswordHash, eso lo hace el otro SP
                objeto.FechaVencimiento = fechaVencimientoDate;

                var rptbd = await _repositorio.Editar(objeto);
                return StatusCode(StatusCodes.Status200OK, rptbd);
            }
            catch (Exception ex)
            {
                Respuesta<int> rptEx = new() { Estado = false, Valor = "error", Data = 0, Mensaje = "Error en el servidor: " + ex.Message };
                return StatusCode(StatusCodes.Status200OK, rptEx);
            }
        }

        [HttpPut]
        public async Task<IActionResult> ActualizarEstado(int idUsuario, bool estado)
        {
            try
            {
                // Llamamos al repositorio
                var rptbd = await _repositorio.ActualizarEstado(idUsuario, estado);
                return StatusCode(StatusCodes.Status200OK, rptbd);
            }
            catch (Exception ex)
            {
                Respuesta<int> rptEx = new()
                {
                    Estado = false,
                    Valor = "error",
                    Data = 0,
                    Mensaje = "Error en el servidor: " + ex.Message
                };
                return StatusCode(StatusCodes.Status200OK, rptEx);
            }
        }

    }
}
