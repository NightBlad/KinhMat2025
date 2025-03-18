using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Cart_2.Data;
using Shopping_Cart_2.Models;
using Shopping_Cart_2.Services;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Shopping_Cart_2.Controllers
{
    public class HomeController(ILogger<HomeController> logger, IItemService itemService, ApplicationDbContext context) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly IItemService _itemService = itemService;
        private readonly ApplicationDbContext _context = context;

        // Trang chủ
        public async Task<IActionResult> Index()
        {
            await Task.CompletedTask;
            return View();
        }

        // Trang hiển thị danh sách sản phẩm, có tìm kiếm và lọc danh mục
        public async Task<IActionResult> Products(string? seachName, string? categoryName)
        {
            var item = await Task.Run(() => _itemService.GetAll()); // Lấy toàn bộ sản phẩm

            // Tìm kiếm theo tên hoặc mô tả sản phẩm
            if (!string.IsNullOrEmpty(seachName))
            {
                item = item.Where(g => g.Name != null && g.Name.Contains(seachName, StringComparison.OrdinalIgnoreCase)
                    || g.Description != null && g.Description.Contains(seachName, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            // Lọc theo danh mục sản phẩm
            else if (categoryName != null)
            {
                item = item.Where(g => g.Category != null && g.Category.Name != null && g.Category.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            ViewBag.seachName = seachName; // Lưu giá trị tìm kiếm để hiển thị lại trên giao diện
            ViewBag.categories = await _context.Categories.ToListAsync(); // Lấy danh sách danh mục sản phẩm

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