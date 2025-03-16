using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping_Cart_2.Models;
using Shopping_Cart_2.Services;

namespace Shopping_Cart_2.Controllers
{
    [Authorize]
    public class UserOrderController : Controller
    {
        private readonly IUserOrderService _userOrderService; // Khai báo service xử lý đơn hàng của người dùng.

        // Constructor: Nhận vào một thể hiện của IUserOrderService để sử dụng trong Controller.
        public UserOrderController(IUserOrderService userOrderService)
        {
            _userOrderService = userOrderService;
        }

        // Phương thức hiển thị danh sách đơn hàng của người dùng.
        public async Task<IActionResult> UserOrders()
        {
            var orders = await _userOrderService.UserOrders(); // Gọi service để lấy danh sách đơn hàng.
            return View(orders); // Trả về View hiển thị danh sách đơn hàng.
        }

        // Phương thức hiển thị chi tiết một đơn hàng cụ thể.
        public async Task<IActionResult> GetDetail(int orderId)
        {
            var detail = await _userOrderService.GetOrderDetail(orderId); // Gọi service để lấy thông tin chi tiết đơn hàng.
            return View(detail); // Trả về View hiển thị chi tiết đơn hàng.
        }
    }
}
