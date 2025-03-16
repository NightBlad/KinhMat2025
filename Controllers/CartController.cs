using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping_Cart_2.Services;
using Shopping_Cart_2.Models;

namespace Shopping_Cart_2.Controllers
{
    [Authorize] // Yêu cầu đăng nhập để truy cập giỏ hàng
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // Thêm sản phẩm vào giỏ hàng với số lượng mặc định là 1
        public async Task<IActionResult> AddItem(int itemId, int qty = 1, int redirect = 0)
        {
            var cartCount = await _cartService.AddItem(itemId, qty);
            if (redirect == 0)
                return Ok(cartCount); // Trả về số lượng sản phẩm trong giỏ hàng
            return RedirectToAction("GetUserCart");
        }

        // Xóa sản phẩm khỏi giỏ hàng
        public async Task<IActionResult> RemoveItem(int itemId)
        {
            var cartCount = await _cartService.RemoveItem(itemId);
            return RedirectToAction("GetUserCart");
        }

        // Lấy giỏ hàng của người dùng hiện tại
        public async Task<IActionResult> GetUserCart()
        {
            var cart = await _cartService.GetUserCart();
            return View(cart);
        }

        // for script in _layout.cshtml // to get total number of items in this cart // then pass it to other place in html
        public async Task<IActionResult> GetTotalItemInCart()
        {
            int cartItem = await _cartService.GetCartItemCount();
            return Ok(cartItem);
        }

        // Hiển thị trang thanh toán
        public IActionResult Checkout()
        {
            return View();
        }

        // Xử lý thanh toán
        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            bool isCheckedOut = await _cartService.DoCheckout(model);
            if (!isCheckedOut)
                return RedirectToAction(nameof(OrderFailure));

            return RedirectToAction(nameof(OrderSuccess));
        }

        // Hiển thị trang thanh toán thành công
        public IActionResult OrderSuccess()
        {
            return View();
        }

        // Hiển thị trang thanh toán thất bại
        public IActionResult OrderFailure()
        {
            return View();
        }
    }
}
