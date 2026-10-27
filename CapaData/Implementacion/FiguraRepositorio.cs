using CapaData.Configuracion;
using CapaData.Interfaces;
using CapaEntidades;
using CapaEntidades.Responses;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;

namespace CapaData.Implementacion
{
    public class FiguraRepositorio : IFiguraRepositorio
    {
        private readonly ConnectionStrings con;
        public FiguraRepositorio(IOptions<ConnectionStrings> options)
        {
            con = options.Value;
        }

        public async Task<Respuesta<int>> Guardar(Figuras objeto)
        {
            // Sintaxis simplificada
            Respuesta<int> response = new()
            {
                Estado = false,
                Valor = "warning",
                Data = 0,
                Mensaje = "Error desconocido"
            };
            int resultadoCodigo = 0;

            try
            {
                using (var conexion = new SqlConnection(con.CadenaSQL))
                {
                    await conexion.OpenAsync();
                    SqlCommand cmd = new("usp_RegistrarFigura", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    // Blindaje contra nulos usando DBNull.Value
                    cmd.Parameters.AddWithValue("@IdTipo", objeto.IdTipo);
                    //cmd.Parameters.AddWithValue("@Nombre", objeto.Nombre ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Nombre", objeto.Nombre);
                    cmd.Parameters.AddWithValue("@ConfiguracionJSON", objeto.ConfiguracionJSON);
                    //cmd.Parameters.AddWithValue("@ConfiguracionJSON", objeto.ConfiguracionJSON ?? (object)DBNull.Value);

                    SqlParameter outputParam = new("@Resultado", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputParam);

                    await cmd.ExecuteNonQueryAsync();
                    resultadoCodigo = Convert.ToInt32(outputParam.Value);
                }

                response.Data = resultadoCodigo;

                if (resultadoCodigo == 1)
                {
                    response.Estado = true;
                    response.Valor = "success";
                    response.Mensaje = "Registrado correctamente.";
                }
                else if (resultadoCodigo == 2)
                {
                    response.Mensaje = "El nombre ya se encuentra registrado.";
                }
                else
                {
                    response.Valor = "error";
                    response.Mensaje = "Ocurrió un error en la base de datos.";
                }
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Valor = "error";
                response.Mensaje = "Error interno: " + ex.Message;
            }

            return response;
        }

        public async Task<Respuesta<int>> GuardarDapper(Figuras objeto)
        {
            Respuesta<int> response = new()
            {
                Estado = false,
                Valor = "warning",
                Data = 0,
                Mensaje = "Error desconocido"
            };

            try
            {
                using (var conexion = new SqlConnection(con.CadenaSQL))
                {
                    // Preparamos los parámetros para Dapper
                    var parametros = new DynamicParameters();
                    parametros.Add("@IdTipo", objeto.IdTipo);
                    parametros.Add("@Nombre", objeto.Nombre);
                    parametros.Add("@ConfiguracionJSON", objeto.ConfiguracionJSON);

                    // Declaramos el parámetro de salida
                    parametros.Add("@Resultado", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    // Ejecutamos el SP
                    await conexion.ExecuteAsync("usp_RegistrarFigura", parametros, commandType: CommandType.StoredProcedure);

                    // Recuperamos el valor del OUTPUT
                    int resultadoCodigo = parametros.Get<int>("@Resultado");
                    response.Data = resultadoCodigo;

                    if (resultadoCodigo == 1)
                    {
                        response.Estado = true;
                        response.Valor = "success";
                        response.Mensaje = "Registrado correctamente.";
                    }
                    else if (resultadoCodigo == 2)
                    {
                        response.Mensaje = "El nombre ya se encuentra registrado.";
                    }
                    else
                    {
                        response.Valor = "error";
                        response.Mensaje = "Ocurrió un error en la base de datos al registrar.";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Valor = "error";
                response.Mensaje = "Error interno: " + ex.Message;
            }

            return response;
        }

        public async Task<Respuesta<List<Figuras>>> Lista()
        {
            Respuesta<List<Figuras>> response = new()
            {
                Estado = false,
                Mensaje = "Error desconocido",
                Data = []
            };

            try
            {
                using (var conexion = new SqlConnection(con.CadenaSQL))
                {
                    var resultado = await conexion.QueryAsync<Figuras>(
                        "usp_ListaFiguras",
                        commandType: CommandType.StoredProcedure
                    );

                    response.Data = resultado.ToList();
                }

                response.Estado = true;
                response.Mensaje = "Lista obtenida correctamente";
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = $"Error en BD: {ex.Message}";
            }

            return response;
        }
    }
}
