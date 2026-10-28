using CapaEntidades;
using CapaEntidades.Responses;

namespace CapaData.Interfaces
{
    public interface IFiguraRepositorio
    {
        Task<Respuesta<List<Figuras>>> Lista();
        Task<Respuesta<int>> Guardar(Figuras objeto);
        Task<Respuesta<int>> GuardarDapper(Figuras objeto);
        Task<Respuesta<int>> Editar(Figuras objeto);
        Task<Respuesta<int>> Eliminar(int id);
        //Task<Respuesta<int>> GuardarDapper(Figuras objeto);
    }
}
