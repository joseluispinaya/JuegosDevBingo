using CapaEntidades;
using CapaEntidades.Responses;

namespace CapaData.Interfaces
{
    public interface ITipoJuegoRepositorio
    {
        Task<Respuesta<List<TipoJuegos>>> Lista();
    }
}
