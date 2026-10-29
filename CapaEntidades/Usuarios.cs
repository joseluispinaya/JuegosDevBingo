namespace CapaEntidades
{
    public class Usuarios
    {
        public int IdUsuario { get; set; }
        public int IdRol { get; set; }
        public string NombreCompleto { get; set; } = null!;
        public string Celular { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string? PasswordHash { get; set; }
        //public bool Estado { get; set; }
        public DateTime FechaVencimiento { get; set; }
        // Propiedad auxiliar para el modelo de entrada (binding) desde el frontend
        public string? FechaVencimientoStr { get; set; }
    }
}
