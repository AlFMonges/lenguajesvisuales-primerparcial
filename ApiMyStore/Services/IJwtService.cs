namespace ApiMyStore.Services
{
    public interface IJwtService
    {
        string GenerateToken(ApiMyStore.Models.Usuario user);
    }
}
