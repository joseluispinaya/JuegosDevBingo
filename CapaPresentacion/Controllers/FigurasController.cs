using CapaData.Interfaces;
using CapaEntidades;
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

    }
}
