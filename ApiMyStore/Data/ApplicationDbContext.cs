using Microsoft.EntityFrameworkCore;
using ApiMyStore.Models;

namespace ApiMyStore.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts) : base(opts) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<CabeceraPedido> CabeceraPedidos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<Producto>().Property(p => p.Price).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Pedido>().Property(oi => oi.UnitPrice).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<CabeceraPedido>().Property(p => p.Total).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<CabeceraPedido>()
                .HasOne(o => o.User)
                .WithMany(u => u.CabeceraPedidos)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Productos)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Pedido>()
                .HasOne(oi => oi.CabeceraPedido)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<Pedido>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.Pedidos)
                .HasForeignKey(oi => oi.ProductId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
