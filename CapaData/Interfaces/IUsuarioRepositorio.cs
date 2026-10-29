using CapaEntidades;
using CapaEntidades.Responses;

namespace CapaData.Interfaces
{
    public interface IUsuarioRepositorio
    {
        Task<Respuesta<Usuarios>> Login(string correo);
        Task<Respuesta<List<Usuarios>>> Lista();
        Task<Respuesta<int>> Guardar(Usuarios objeto);
        Task<Respuesta<int>> Editar(Usuarios objeto);
        Task<Respuesta<int>> ActualizarClave(int idUsuario, string nuevaClave);
        Task<Respuesta<int>> ActualizarEstado(int idUsuario, bool estado);
    }
}
