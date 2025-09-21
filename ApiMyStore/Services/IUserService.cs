using ApiMyStore.Models;

namespace ApiMyStore.Services
{
    public interface IUserService
    {
        int GetUserId(); // devuelve el Id del usuario autenticado
        string GetUserName();
    }
}
