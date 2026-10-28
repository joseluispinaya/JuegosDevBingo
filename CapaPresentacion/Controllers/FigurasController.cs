using CapaData.Interfaces;
using CapaEntidades;
using CapaEntidades.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CapaPresentacion.Controllers
{
    public class FigurasController : Controller
    {
        private readonly ITipoJuegoRepositorio _repositTipoJuego;
        private readonly IFiguraRepositorio _repositorio;

        public FigurasController(ITipoJuegoRepositorio repositTipoJuego, IFiguraRepositorio repositorio)
        {
            _repositTipoJuego = repositTipoJuego;
            _repositorio = repositorio;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaTipoJuego()
        {
            var rpt = await _repositTipoJuego.Lista();

            // Retornamos directamente el objeto rpt. .NET lo serializa a JSON automáticamente.
            return StatusCode(StatusCodes.Status200OK, rpt);
        }

        [HttpGet]
        public async Task<IActionResult> ListaTipoJuegoDapper()
        {
            var rpt = await _repositTipoJuego.ListaDapper();

            // Retornamos directamente el objeto rpt. .NET lo serializa a JSON automáticamente.
            return StatusCode(StatusCodes.Status200OK, rpt);
        }

        [HttpGet]
        public async Task<IActionResult> ListaFiguras()
        {
            var rpt = await _repositorio.Lista();

            // Retornamos directamente el objeto rpt. .NET lo serializa a JSON automáticamente.
            return StatusCode(StatusCodes.Status200OK, rpt);
        }

        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] Figuras objeto)
        {
            // Puedes elegir usar _repositorio.Guardar(objeto) o .GuardarDapper(objeto)
            // El resultado será idéntico para tu frontend
            var rpt = await _repositorio.Guardar(objeto);

            return StatusCode(StatusCodes.Status200OK, rpt);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarPrueba([FromBody] Figuras objeto)
        {
            // Espera asíncrona de 2 segundos. Ideal para probar el LoadingOverlay en el frontend.
            await Task.Delay(2000);

            Respuesta<int> response = new();

            // Pon tu punto de interrupción (breakpoint) en la línea del 'if'
            if (objeto == null)
            {
                response.Estado = false;
                response.Valor = "error";
                response.Mensaje = "Datos vacíos o mal formados.";
                return StatusCode(StatusCodes.Status200OK, response);
            }

            response.Estado = true;
            response.Valor = "success";
            response.Mensaje = "Prueba de registro exitosa. Los datos llegaron correctamente.";
            response.Data = 1; // Simulamos que devolvió el ID 1

            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] Figuras objeto)
        {
            var rpt = await _repositorio.Editar(objeto);
            return StatusCode(StatusCodes.Status200OK, rpt);
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int id)
        {
            var rpt = await _repositorio.Eliminar(id);
            return StatusCode(StatusCodes.Status200OK, rpt);
        }

    }
}
