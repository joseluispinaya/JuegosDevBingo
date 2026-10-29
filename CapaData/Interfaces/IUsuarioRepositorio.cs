using CapaEntidades;
using CapaEntidades.DTO;
using CapaEntidades.Responses;

namespace CapaData.Interfaces
{
    public interface IUsuarioRepositorio
    {
        Task<Respuesta<UsuarioLoginDTO>> Login(string correo);
        Task<Respuesta<List<UsuariosListDTO>>> ListaDapper();
        Task<Respuesta<List<UsuariosListDTO>>> Lista();
        Task<Respuesta<int>> Guardar(Usuarios objeto);
        Task<Respuesta<int>> Editar(Usuarios objeto);
        Task<Respuesta<int>> ActualizarClave(int idUsuario, string nuevaClave);
        Task<Respuesta<int>> ActualizarEstado(int idUsuario, bool estado);
    }
}
