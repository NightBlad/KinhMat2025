using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shopping_Cart_2.Models;

namespace Shopping_Cart_2.Data
{
    // DbContext chính, quản lý kết nối cơ sở dữ liệu và các bảng
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Các bảng trong cơ sở dữ liệu
        public DbSet<Category> categories { get; set; }
        public DbSet<Item> items { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<CartDetail> CartDetails { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<OrderStatus> orderStatuses { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Gọi cấu hình mặc định của IdentityDbContext

            // Cấu hình quan hệ giữa OrderItem, Order và Item (bị comment, có thể kích hoạt nếu cần)
            //    builder.Entity<OrderItem>()
            //   .HasKey(e => new { e.OrderId,e.ItemId  });

            //    builder.Entity<OrderItem>()
            //    .HasOne<Item>(sc => sc.Item)
            //    .WithMany(s => s.Orders)
            //    .HasForeignKey(sc => sc.ItemId);

            //    builder.Entity<OrderItem>()
            //        .HasOne<Order>(sc => sc.Order)
            //        .WithMany(c => c.Items)
            //        .HasForeignKey(sc => sc.OrderId); 

        }
    }
}
