using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;


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

            // DEBUG: Mostrar TODOS los claims
            Console.WriteLine("=== TODOS LOS CLAIMS ===");
            foreach (var claim in user.Claims)
            {
                Console.WriteLine($"Type: '{claim.Type}' | Value: '{claim.Value}'");
            }
            Console.WriteLine("========================");

            // Buscar específicamente cada posible claim
            var nameIdentifierClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            var idClaim = user.FindFirst("id");


            // Usar la lógica original
            var idValue =
                user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user.FindFirst("id")?.Value;

            Console.WriteLine($"ID Value seleccionado: '{idValue}'");

            if (string.IsNullOrEmpty(idValue))
                throw new InvalidOperationException("Claim 'id' no encontrada en el token.");

            if (!int.TryParse(idValue, out var id))
                throw new InvalidOperationException($"Claim 'id' con valor '{idValue}' no tiene formato numérico.");

            return id;
        }

        public string GetUserName() =>
            _httpContext.HttpContext?.User?.Identity?.Name ?? string.Empty;
    }
}

