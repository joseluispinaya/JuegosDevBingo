namespace CapaPresentacion.Helpers
{
    public interface IPasswordHelper
    {
        string Hash(string password);
        bool Verify(string password, string hash);
    }
}
