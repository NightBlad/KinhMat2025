using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Cart_2.Data;
using Shopping_Cart_2.Models;
using Shopping_Cart_2.Services;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Shopping_Cart_2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger; // Đối tượng dùng để ghi log hệ thống
        private readonly ApplicationDbContext _context; // Kết nối database
        private readonly IItemService _itemService; // Service quản lý sản phẩm

        // Constructor, khởi tạo các dịch vụ
        public HomeController(ILogger<HomeController> logger, IItemService itemService, ApplicationDbContext context)
        {
            _logger = logger;
            _itemService = itemService;
            _context = context;
        }

        // Trang chủ
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // Trang hiển thị danh sách sản phẩm, có tìm kiếm và lọc danh mục
        public async Task<IActionResult> Products(string? seachName, string? categoryName)
        {
            var item = _itemService.GetAll(); // Lấy toàn bộ sản phẩm

            // Tìm kiếm theo tên hoặc mô tả sản phẩm
            if (!string.IsNullOrEmpty(seachName))
            {
                item = item.Where(g => g.Name.ToLower().Contains(seachName.ToLower())
                    || g.Description.ToLower().Contains(seachName.ToLower())).ToList();
            }
            // Lọc theo danh mục sản phẩm
            else if (categoryName != null)
            {
                item = item.Where(g => g.Category.Name.ToLower() == categoryName.ToLower()).ToList();
            }

            ViewBag.seachName = seachName; // Lưu giá trị tìm kiếm để hiển thị lại trên giao diện
            ViewBag.categories = _context.categories.ToList(); // Lấy danh sách danh mục sản phẩm

            return View(item); // Trả về danh sách sản phẩm phù hợp
        }

        // Trang chính sách bảo mật
        public IActionResult Privacy()
        {
            return View();
        }

        // Xử lý lỗi và hiển thị trang lỗi
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}