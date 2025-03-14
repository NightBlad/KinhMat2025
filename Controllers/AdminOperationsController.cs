using Microsoft.AspNetCore.Authorization; // Thư viện để quản lý quyền truy cập
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shopping_Cart_2.Constants; // Chứa các hằng số như Roles.Admin
using Shopping_Cart_2.Data;
using Shopping_Cart_2.Models;
using Shopping_Cart_2.Services;

namespace Shopping_Cart_2.Controllers
{
    // Chỉ cho phép Admin truy cập vào Controller này
    [Authorize(Roles = nameof(Roles.Admin))]
    public class AdminOperationsController : Controller
    {
        // Các service dùng để xử lý dữ liệu
        private readonly IUserOrderService _userOrderService; // Dịch vụ xử lý đơn hàng
        private readonly IManageItemService _manageItemService; // Dịch vụ quản lý sản phẩm
        private readonly ICategoryService _categoryService; // Dịch vụ quản lý danh mục

        // Inject các service thông qua constructor
        public AdminOperationsController(IUserOrderService userOrderService, IManageItemService manageItemService, ICategoryService categoryService)
        {
            _userOrderService = userOrderService;
            _manageItemService = manageItemService;
            _categoryService = categoryService;
        }

        // Lấy danh sách tất cả đơn hàng
        public async Task<IActionResult> AllOrders()
        {
            var orders = await _userOrderService.AllOrders();
            return View(orders);
        }

        // Hiển thị form cập nhật trạng thái đơn hàng
        [HttpGet]
        public async Task<IActionResult> UpdateOrderStatus(int orderId)
        {
            var order = await _userOrderService.GetOrderById(orderId);
            if (order == null)
            {
                throw new InvalidOperationException($"Order with id:{orderId} does not found.");
            }

            var orderStatusList = _userOrderService.GetSelectLists(); // Lấy danh sách trạng thái đơn hàng có thể chọn
            var data = new UpdateOrderStatusModel
            {
                OrderId = orderId,
                OrderStatusId = order.OrderStatusId, // Lấy trạng thái hiện tại của đơn hàng
                OrderStatusList = orderStatusList
            };
            return View(data);
        }

        // Cập nhật trạng thái đơn hàng
        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(UpdateOrderStatusModel data)
        {
            try
            {
                if (!ModelState.IsValid) // Kiểm tra dữ liệu nhập vào có hợp lệ không
                {
                    data.OrderStatusList = _userOrderService.GetSelectLists(); // Nếu lỗi, tải lại danh sách trạng thái
                    return View(data);
                }

                await _userOrderService.ChangeOrderStatus(data); // Cập nhật trạng thái đơn hàng
                TempData["msg"] = "Updated successfully"; // Gửi thông báo cập nhật thành công
            }
            catch
            {
                TempData["msg"] = "Something went wrong"; // Gửi thông báo lỗi
            }
            return RedirectToAction(nameof(UpdateOrderStatus), new { orderId = data.OrderId }); // Chuyển hướng về trang cập nhật
        }

        // Chuyển đổi trạng thái thanh toán (đã thanh toán hoặc chưa thanh toán)
        public async Task<IActionResult> TogglePaymentStatus(int orderId)
        {
            try
            {
                await _userOrderService.TogglePaymentStatus(orderId);
            }
            catch (Exception ex)
            {
                // Log lỗi ở đây
            }
            return RedirectToAction(nameof(AllOrders)); // Quay lại trang danh sách đơn hàng
        }

        // Trang Dashboard của Admin
        public IActionResult Dashboard()
        {
            return View();
        }

        // Duyệt hoặc hủy duyệt sản phẩm
        public async Task<IActionResult> ToggleApprovementStatus(int ItemId)
        {
            try
            {
                await _manageItemService.ToggleApprovementStatus(ItemId);
            }
            catch (Exception ex)
            {
                // Log lỗi ở đây
            }
            return RedirectToAction(nameof(GetAllItems)); // Quay lại danh sách sản phẩm
        }

        // Lấy danh sách tất cả sản phẩm
        public async Task<IActionResult> GetAllItems()
        {
            var items = await _manageItemService.GetAllItems();
            return View(items);
        }

        // Lấy danh sách tất cả danh mục
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategories();
            return View(categories);
        }

        // Hiển thị form tạo danh mục mới
        public IActionResult CreateCategory()
        {
            return View();
        }

        // Xử lý tạo danh mục mới
        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            if (ModelState.IsValid) // Kiểm tra dữ liệu nhập vào
            {
                await _categoryService.AddCategory(category);
                return RedirectToAction(nameof(GetAllCategories)); // Quay lại danh sách danh mục
            }
            return View(category);
        }

        // Hiển thị form chỉnh sửa danh mục
        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _categoryService.GetCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // Xử lý chỉnh sửa danh mục
        [HttpPost]
        public async Task<IActionResult> EditCategory(Category category)
        {
            if (ModelState.IsValid) // Kiểm tra dữ liệu nhập vào
            {
                await _categoryService.UpdateCategory(category);
                return RedirectToAction(nameof(GetAllCategories)); // Quay lại danh sách danh mục
            }
            return View(category);
        }

        // Hiển thị form xác nhận xóa danh mục
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _categoryService.GetCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // Xóa danh mục khỏi hệ thống
        [HttpPost, ActionName("DeleteCategory")]
        public async Task<IActionResult> DeleteCategoryConfirmed(int id)
        {
            await _categoryService.DeleteCategory(id);
            return RedirectToAction(nameof(GetAllCategories)); // Quay lại danh sách danh mục
        }
    }
}
