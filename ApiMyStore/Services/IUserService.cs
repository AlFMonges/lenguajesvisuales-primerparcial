using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ApiMyStore.Services
{
    public interface IUserService
    {
        int GetUserId();
        string GetUserName();
    }

    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContext;

        public UserService(IHttpContextAccessor httpContext) => _httpContext = httpContext;

        public int GetUserId()
        {
            var user = _httpContext.HttpContext?.User;
            if (user == null || !user.Identity!.IsAuthenticated)
                throw new InvalidOperationException("Usuario no autenticado.");

            // Buscar de forma robusta varias posibles claims que podrían contener el id
            var idValue =
                user.FindFirst(ClaimTypes.NameIdentifier)?.Value // estándar
                ?? user.FindFirst("id")?.Value;               // tu implementación actual
      //          ?? user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value; // a veces se usa sub

            if (string.IsNullOrEmpty(idValue))
                throw new InvalidOperationException("Claim 'id' no encontrada en el token.");

            if (!int.TryParse(idValue, out var id))
                throw new InvalidOperationException("Claim 'id' no tiene formato numérico.");

            return id;
        }

        public string GetUserName() =>
            _httpContext.HttpContext?.User?.Identity?.Name ?? string.Empty;
    }
}

