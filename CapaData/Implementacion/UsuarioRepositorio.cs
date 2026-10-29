using CapaData.Configuracion;
using CapaData.Interfaces;
using CapaEntidades;
using CapaEntidades.DTO;
using CapaEntidades.Responses;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaData.Implementacion
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly ConnectionStrings con;
        public UsuarioRepositorio(IOptions<ConnectionStrings> options)
        {
            con = options.Value;
        }

        public async Task<Respuesta<int>> ActualizarClave(int idUsuario, string nuevaClave)
        {
            Respuesta<int> response = new() { Estado = false, Valor = "warning", Data = 0, Mensaje = "Error desconocido" };

            try
            {
                using (var conexion = new SqlConnection(con.CadenaSQL))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add("@IdUsuario", idUsuario);
                    parametros.Add("@NuevaClave", nuevaClave);
                    parametros.Add("@Resultado", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    await conexion.ExecuteAsync("usp_ActualizarClave", parametros, commandType: CommandType.StoredProcedure);

                    int resultadoCodigo = parametros.Get<int>("@Resultado");
                    response.Data = resultadoCodigo;

                    if (resultadoCodigo == 1)
                    {
                        response.Estado = true; response.Valor = "success"; response.Mensaje = "Contraseña actualizada correctamente.";
                    }
                    else
                    {
                        response.Valor = "error"; response.Mensaje = "El usuario no existe o no se pudo actualizar.";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Valor = "error"; response.Mensaje = "Error interno: " + ex.Message;
            }

            return response;
        }

        public async Task<Respuesta<int>> ActualizarEstado(int idUsuario, bool estado)
        {
            Respuesta<int> response = new() { Estado = false, Valor = "warning", Data = 0, Mensaje = "Error desconocido" };

            try
            {
                using (var conexion = new SqlConnection(con.CadenaSQL))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add("@IdUsuario", idUsuario);
                    parametros.Add("@Estado", estado);
                    parametros.Add("@Resultado", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    await conexion.ExecuteAsync("usp_ActualizarEstado", parametros, commandType: CommandType.StoredProcedure);

                    int resultadoCodigo = parametros.Get<int>("@Resultado");
                    response.Data = resultadoCodigo;

                    if (resultadoCodigo == 1)
                    {
                        response.Estado = true; response.Valor = "success"; response.Mensaje = "Estado actualizado correctamente.";
                    }
                    else
                    {
                        response.Valor = "error"; response.Mensaje = "El usuario no existe o no se pudo actualizar.";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Valor = "error"; response.Mensaje = "Error interno: " + ex.Message;
            }

            return response;
        }

        public async Task<Respuesta<int>> Editar(Usuarios objeto)
        {
            Respuesta<int> response = new() { Estado = false, Valor = "warning", Data = 0, Mensaje = "Error desconocido" };

            try
            {
                using (var conexion = new SqlConnection(con.CadenaSQL))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add("@IdUsuario", objeto.IdUsuario);
                    parametros.Add("@IdRol", objeto.IdRol);
                    parametros.Add("@NombreCompleto", objeto.NombreCompleto);
                    parametros.Add("@Celular", objeto.Celular);
                    parametros.Add("@Correo", objeto.Correo);
                    //parametros.Add("@Estado", objeto.Estado);
                    parametros.Add("@FechaVencimiento", objeto.FechaVencimiento);
                    parametros.Add("@Resultado", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    await conexion.ExecuteAsync("usp_EditarUsuario", parametros, commandType: CommandType.StoredProcedure);

                    int resultadoCodigo = parametros.Get<int>("@Resultado");
                    response.Data = resultadoCodigo;

                    if (resultadoCodigo == 1)
                    {
                        response.Estado = true; response.Valor = "success"; response.Mensaje = "Editado correctamente.";
                    }
                    else if (resultadoCodigo == 2)
                    {
                        response.Mensaje = "El correo ya pertenece a otro usuario.";
                    }
                    else
                    {
                        response.Valor = "error"; response.Mensaje = "Ocurrió un error en la base de datos.";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Valor = "error"; response.Mensaje = "Error interno: " + ex.Message;
            }

            return response;
        }

        public async Task<Respuesta<int>> Guardar(Usuarios objeto)
        {
            Respuesta<int> response = new() { Estado = false, Valor = "warning", Data = 0, Mensaje = "Error desconocido" };

            try
            {
                using (var conexion = new SqlConnection(con.CadenaSQL))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add("@IdRol", objeto.IdRol);
                    parametros.Add("@NombreCompleto", objeto.NombreCompleto);
                    parametros.Add("@Celular", objeto.Celular);
                    parametros.Add("@Correo", objeto.Correo);
                    parametros.Add("@PasswordHash", objeto.PasswordHash);
                    //parametros.Add("@Estado", objeto.Estado);
                    parametros.Add("@FechaVencimiento", objeto.FechaVencimiento);
                    parametros.Add("@Resultado", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    await conexion.ExecuteAsync("usp_RegistrarUsuario", parametros, commandType: CommandType.StoredProcedure);

                    int resultadoCodigo = parametros.Get<int>("@Resultado");
                    response.Data = resultadoCodigo;

                    if (resultadoCodigo == 1)
                    {
                        response.Estado = true; response.Valor = "success"; response.Mensaje = "Registrado correctamente.";
                    }
                    else if (resultadoCodigo == 2)
                    {
                        response.Mensaje = "El correo ya se encuentra registrado.";
                    }
                    else
                    {
                        response.Valor = "error"; response.Mensaje = "Ocurrió un error en la base de datos.";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Valor = "error"; response.Mensaje = "Error interno: " + ex.Message;
            }

            return response;
        }

        public async Task<Respuesta<List<UsuariosListDTO>>> Lista()
        {
            // Inicializamos por defecto como false y con la lista vacía
            Respuesta<List<UsuariosListDTO>> rpt = new()
            {
                Estado = false,
                Mensaje = "Error desconocido",
                Data = []
            };

            try
            {
                using (var conexion = new SqlConnection(con.CadenaSQL))
                {
                    await conexion.OpenAsync();
                    SqlCommand cmd = new("usp_ListaUsuarios", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    using var dr = await cmd.ExecuteReaderAsync();
                    while (await dr.ReadAsync())
                    {
                        rpt.Data.Add(new UsuariosListDTO()
                        {
                            IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                            IdRol = Convert.ToInt32(dr["IdRol"]),
                            NombreRol = dr["NombreRol"].ToString(),
                            NombreCompleto = dr["NombreCompleto"].ToString()!,
                            Celular = dr["Celular"].ToString()!,
                            Correo = dr["Correo"].ToString()!,
                            Estado = Convert.ToBoolean(dr["Estado"]),
                            FechaVencimiento = Convert.ToDateTime(dr["FechaVencimiento"]),
                            FechaRegistroLocal = Convert.ToDateTime(dr["FechaRegistroLocal"])
                        });
                    }
                }

                rpt.Estado = true;
                rpt.Mensaje = "Lista obtenida correctamente";
            }
            catch (Exception ex)
            {
                rpt.Estado = false;
                rpt.Mensaje = $"Error en BD: {ex.Message}";
            }

            return rpt;
        }

        public async Task<Respuesta<List<UsuariosListDTO>>> ListaDapper()
        {
            Respuesta<List<UsuariosListDTO>> rpt = new()
            {
                Estado = false,
                Mensaje = "Error desconocido",
                Data = []
            };

            try
            {
                using (var conexion = new SqlConnection(con.CadenaSQL))
                {
                    // Dapper mapea directamente los resultados a tu nueva clase DTO
                    var resultado = await conexion.QueryAsync<UsuariosListDTO>(
                        "usp_ListaUsuarios",
                        commandType: CommandType.StoredProcedure
                    );

                    rpt.Data = [.. resultado];
                }

                rpt.Estado = true;
                rpt.Mensaje = "Lista obtenida correctamente";
            }
            catch (Exception ex)
            {
                rpt.Estado = false;
                rpt.Mensaje = $"Error en BD: {ex.Message}";
            }

            return rpt;
        }

        public async Task<Respuesta<UsuarioLoginDTO>> Login(string correo)
        {
            Respuesta<UsuarioLoginDTO> response = new() { Estado = false, Mensaje = "Error desconocido" };

            try
            {
                using (var conexion = new SqlConnection(con.CadenaSQL))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add("@Correo", correo);

                    var resultado = await conexion.QueryFirstOrDefaultAsync<UsuarioLoginDTO>(
                        "usp_LoginUsuario",
                        parametros,
                        commandType: CommandType.StoredProcedure
                    );

                    if (resultado != null)
                    {
                        response.Estado = true;
                        response.Data = resultado;
                        response.Mensaje = "Usuario encontrado";
                    }
                    else
                    {
                        response.Mensaje = "El correo no está registrado.";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Mensaje = $"Error en BD: {ex.Message}";
            }

            return response;
        }
    }
}
