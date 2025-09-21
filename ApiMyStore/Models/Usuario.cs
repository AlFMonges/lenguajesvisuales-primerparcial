namespace ApiMyStore.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? Role { get; set; }   // "Admin" / "User"
        public ICollection<CabeceraPedido>? CabeceraPedidos { get; set; }
    }
}
