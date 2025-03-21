﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Cart_2.Services;
using Shopping_Cart_2.ViewModels;

namespace Shopping_Cart_2.Controllers
{
    public class ItemsController(ICategoryService categoryService, IItemService itemService, IRatingService ratingService, IUserService userService) : Controller
    {
        private readonly ICategoryService _categoryService = categoryService;
        private readonly IItemService _itemService = itemService;
        private readonly IRatingService _ratingService = ratingService;
        private readonly IUserService _userService = userService;

        [Authorize] // Yêu cầu người dùng phải đăng nhập để truy cập danh sách sản phẩm
        public IActionResult Index()
        {
            var items = _itemService.GetItemsByUserId();
            return View(items);
        }

        // Lấy thông tin chi tiết của một sản phẩm dựa trên itemId
        public IActionResult Details(int itemId)
        {
            var item = _itemService.GetById(itemId);
            if (item is null) return NotFound();

            // Lấy đánh giá của người dùng hiện tại cho sản phẩm
            var usertRate = _ratingService.GetUserRate(_userService.GetUserId(), itemId);
            ViewBag.userRate = usertRate;

            // Lấy đánh giá trung bình của sản phẩm
            var productRate = _ratingService.GetProductRate(itemId);
            ViewBag.productRate = productRate;

            return View(item);
        }

        [HttpGet]
        public IActionResult Create()
        {
            CreateItemVM vm = new()
            {
                Categories = _categoryService.GetSelectList(), // Lấy danh sách danh mục để hiển thị trong dropdown
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateItemVM model, Stock stock)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            await _itemService.Create(model, stock); // Tạo mới một sản phẩm
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var item = _itemService.GetById(id);
            if (item is null) return NotFound();

            // Khởi tạo model chỉnh sửa sản phẩm
            EditItemVM model = new()
            {
                Id = id,
                Name = item.Name,
                Description = item.Description,
                CategoryId = item.CategoryId,
                Price = item.Price,
                Categories = _categoryService.GetSelectList(),
                CurrentCover = item.Cover, // Hình ảnh hiện tại của sản phẩm
                Quantity = item.Stock.Quantity
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditItemVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var item = await _itemService.Update(model); // Cập nhật thông tin sản phẩm
            return RedirectToAction(nameof(Index));
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var isDeleted = _itemService.Delete(id); // Xóa sản phẩm dựa trên ID
            return isDeleted ? Ok() : BadRequest();
        }

        // Xử lý đánh giá sản phẩm
        public IActionResult RateProduct(string ratingValue, int itemId)
        {
            var isRated = _ratingService.RateProduct(Int32.Parse(ratingValue), itemId, _userService.GetUserId());
            if (isRated < 0)
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}
