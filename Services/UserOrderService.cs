using Hangfire.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shopping_Cart_2.Data;
using Shopping_Cart_2.Models;

namespace Shopping_Cart_2.Services
{
    public class UserOrderService : IUserOrderService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Khởi tạo dịch vụ với các dependency cần thiết
        public UserOrderService(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        // Lấy ID của người dùng hiện đang được xác thực
        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User; // Người dùng hiện tại
            string userId = _userManager.GetUserId(principal);
            return userId;
        }

        // Lấy tất cả đơn hàng của người dùng hiện tại
        public async Task<IEnumerable<Order>> UserOrders()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Người dùng chưa đăng nhập");

            var orders = await _db.Orders
                           .Include(x => x.OrderStatus) // Bao gồm trạng thái đơn hàng
                           .Include(x => x.OrderDetail) // Bao gồm chi tiết đơn hàng
                           .ThenInclude(x => x.Item) // Bao gồm thông tin mặt hàng
                           .ThenInclude(x => x.Category) // Bao gồm thông tin danh mục
                           .Where(a => a.UserId == userId) // Lọc theo ID người dùng
                           .ToListAsync(); // Trả về danh sách bất đồng bộ
            return orders;
        }

        // Lấy tất cả đơn hàng trong hệ thống
        public async Task<IEnumerable<Order>> AllOrders()
        {
            var orders = await _db.Orders
                          .Include(x => x.OrderStatus) // Bao gồm trạng thái đơn hàng
                          .Include(x => x.OrderDetail) // Bao gồm chi tiết đơn hàng
                          .ThenInclude(x => x.Item) // Bao gồm thông tin mặt hàng
                          .ThenInclude(x => x.Category) // Bao gồm thông tin danh mục
                          .ToListAsync(); // Trả về danh sách bất đồng bộ
            return orders;
        }

        // Lấy thông tin một đơn hàng theo ID
        public async Task<Order?> GetOrderById(int id)
        {
            return await _db.Orders.SingleOrDefaultAsync(x => x.Id == id); // Lấy đơn hàng duy nhất hoặc null
        }

        // Lấy chi tiết một đơn hàng của người dùng hiện tại
        public async Task<Order> GetOrderDetail(int orderId)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Người dùng chưa đăng nhập");

            var order = await _db.Orders
                           .Include(x => x.OrderStatus) // Bao gồm trạng thái đơn hàng
                           .Include(x => x.OrderDetail) // Bao gồm chi tiết đơn hàng
                           .ThenInclude(x => x.Item) // Bao gồm thông tin mặt hàng
                           .ThenInclude(x => x.Category) // Bao gồm thông tin danh mục
                           .Where(a => a.UserId == userId) // Lọc theo ID người dùng
                           .SingleOrDefaultAsync(x => x.Id == orderId); // Lấy đơn hàng duy nhất hoặc null
            return order;
        }

        // Thay đổi trạng thái của một đơn hàng
        public async Task ChangeOrderStatus(UpdateOrderStatusModel data)
        {
            var order = await _db.Orders.FindAsync(data.OrderId); // Tìm đơn hàng theo ID
            if (order == null)
                throw new InvalidOperationException($"Đơn hàng với ID: {data.OrderId} không được tìm thấy");

            order.OrderStatusId = data.OrderStatusId; // Cập nhật trạng thái đơn hàng
            await _db.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu
        }

        // Chuyển đổi trạng thái thanh toán của một đơn hàng
        public async Task TogglePaymentStatus(int orderId)
        {
            var order = await _db.Orders.FindAsync(orderId); // Tìm đơn hàng theo ID
            if (order == null)
            {
                throw new InvalidOperationException($"Đơn hàng với ID: {orderId} không được tìm thấy");
            }
            order.IsPaid = !order.IsPaid; // Đảo ngược trạng thái thanh toán (true thành false và ngược lại)
            await _db.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu
        }

        // Lấy danh sách trạng thái đơn hàng dưới dạng SelectListItem để sử dụng trong dropdown
        public IEnumerable<SelectListItem> GetSelectLists()
        {
            return _db.orderStatuses.Select(os => new SelectListItem
            {
                Value = os.Id.ToString(), // Gán ID trạng thái
                Text = os.StatusName // Gán tên trạng thái
            }).ToList(); // Trả về danh sách
        }
    }
}