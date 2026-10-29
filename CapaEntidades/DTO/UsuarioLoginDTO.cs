namespace CapaEntidades.DTO
{
    public class UsuarioLoginDTO
    {
        public int IdUsuario { get; set; }
        public int IdRol { get; set; }
        public string? NombreRol { get; set; }
        public string NombreCompleto { get; set; } = null!;
        public string Celular { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string? PasswordHash { get; set; }
        public bool Estado { get; set; }

        // Dapper mapea estas propiedades directamente desde SQL
        public DateTime FechaVencimiento { get; set; }
        public DateTime FechaRegistroLocal { get; set; }

        // Propiedades calculadas (getters) que devuelven el formato dd/MM/yyyy automáticamente para tu frontend
        public string FechaVencimientoStr => FechaVencimiento.ToString("dd/MM/yyyy");
        public string FechaRegistroLocalStr => FechaRegistroLocal.ToString("dd/MM/yyyy");
    }
}
