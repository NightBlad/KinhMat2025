using Microsoft.AspNetCore.Identity;
using Shopping_Cart_2.Constants;

namespace Shopping_Cart_2.Data
{
    public class DbSeeder
    {
        public static async Task SeedDefaultData(IServiceProvider service)
        {
            // Lấy dịch vụ UserManager và RoleManager từ Dependency Injection (DI)
            var userMgr = service.GetService<UserManager<IdentityUser>>();
            var roleMgr = service.GetService<RoleManager<IdentityRole>>();

            if (userMgr == null || roleMgr == null)
            {
                throw new InvalidOperationException("UserManager or RoleManager is not configured properly.");
            }

            // Thêm các vai trò (roles) mặc định vào cơ sở dữ liệu
            await roleMgr.CreateAsync(new(Roles.Admin.ToString())); // Tạo role Admin
            await roleMgr.CreateAsync(new(Roles.User.ToString()));  // Tạo role User

            // Tạo tài khoản Admin mặc định
            var admin = new IdentityUser
            {
                UserName = "admin@gmail.com",  // Tên đăng nhập Admin
                Email = "admin@gmail.com",     // Email Admin
                EmailConfirmed = true         // Đánh dấu email đã xác nhận
            };

            // Kiểm tra xem tài khoản Admin đã tồn tại trong DB chưa
            var userInDb = await userMgr.FindByEmailAsync(admin.Email);
            if (userInDb is null) // Nếu chưa có, thì tạo mới
            {
                await userMgr.CreateAsync(admin, "Admin@123"); // Tạo tài khoản Admin với mật khẩu mặc định
                await userMgr.AddToRoleAsync(admin, Roles.Admin.ToString()); // Gán tài khoản Admin vào role "Admin"
            }

            // Call SeedOrderStatusData to ensure order statuses are seeded
            var dbContext = service.GetService<ApplicationDbContext>();
            if (dbContext != null)
            {
                await SeedOrderStatusData(dbContext);
            }
        }

        private static async Task SeedOrderStatusData(ApplicationDbContext dbContext)
        {
            if (!await dbContext.OrderStatuses.AnyAsync())
            {
                var orderStatuses = new List<OrderStatus>
                        {
                            new() { StatusName = "Pending" },
                            new() { StatusName = "Processing" },
                            new() { StatusName = "Shipped" },
                            new() { StatusName = "Delivered" },
                            new() { StatusName = "Cancelled" }
                        };

                await dbContext.OrderStatuses.AddRangeAsync(orderStatuses);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
