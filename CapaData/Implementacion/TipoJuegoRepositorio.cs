using CapaData.Configuracion;
using CapaData.Interfaces;
using CapaEntidades;
using CapaEntidades.Responses;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;

namespace CapaData.Implementacion
{
    public class TipoJuegoRepositorio : ITipoJuegoRepositorio
    {
        private readonly ConnectionStrings con;

        public TipoJuegoRepositorio(IOptions<ConnectionStrings> options)
        {
            con = options.Value;
        }

        public async Task<Respuesta<List<TipoJuegos>>> Lista()
        {
            // Inicializamos por defecto como false y con la lista vacía
            Respuesta<List<TipoJuegos>> rpt = new()
            {
                Estado = false,
                Mensaje = "Error desconocido",
                Data = [] // Sintaxis simplificada de C# 12 para new List<TipoJuegos>()
            };

            try
            {
                using (var conexion = new SqlConnection(con.CadenaSQL))
                {
                    await conexion.OpenAsync();
                    SqlCommand cmd = new("usp_ListaTipoJuegos", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    using var dr = await cmd.ExecuteReaderAsync();
                    while (await dr.ReadAsync())
                    {
                        rpt.Data.Add(new TipoJuegos()
                        {
                            IdTipo = Convert.ToInt32(dr["IdTipo"]),
                            Nombre = dr["Nombre"].ToString()
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
    }
}
