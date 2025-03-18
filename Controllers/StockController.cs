namespace Shopping_Cart_2.Controllers
{
    // [Authorize] đảm bảo chỉ những người dùng đã đăng nhập mới có thể truy cập controller này.
    [Authorize]
    public class StockController(IStockService stockService) : Controller
    {
        private readonly IStockService _stockService = stockService; // Dịch vụ quản lý kho hàng.

        // Hiển thị danh sách tồn kho của tất cả sản phẩm.
        public async Task<IActionResult> Index()
        {
            var itemStock = await _stockService.GetStocks(); // Lấy danh sách tồn kho từ service.
            return View(itemStock); // Trả về giao diện danh sách tồn kho.
        }

        // Phương thức GET: Hiển thị form quản lý kho cho một sản phẩm cụ thể.
        [HttpGet]
        public async Task<IActionResult> ManangeStock(int itemId)
        {
            var stock = await _stockService.GetStockByItemId(itemId); // Lấy thông tin kho hàng của sản phẩm.

            // Tạo đối tượng DTO để truyền dữ liệu sang giao diện.
            var dtoStock = new StockDTO
            {
                ItemId = itemId, // Gán ID sản phẩm.
                Quantity = stock != null ? stock.Quantity : 0, // Nếu có dữ liệu, lấy số lượng, nếu không thì mặc định là 0.
            };

            return View(dtoStock); // Trả về View với dữ liệu kho hàng.
        }

        // Phương thức POST: Cập nhật số lượng tồn kho của sản phẩm.
        [HttpPost]
        public async Task<IActionResult> ManangeStock(StockDTO model)
        {
            // Kiểm tra nếu dữ liệu không hợp lệ, quay lại form nhập.
            if (!ModelState.IsValid)
                return View(model);
            try
            {
                await _stockService.ManageStock(model); // Gọi service để cập nhật kho hàng.
                TempData["successMessage"] = "Cập nhật kho hàng thành công."; // Thông báo thành công.
            }
            catch (Exception)
            {
                TempData["errorMessage"] = "Đã xảy ra lỗi!"; // Thông báo lỗi nếu có lỗi xảy ra.
            }

            return RedirectToAction(nameof(Index)); // Quay lại trang danh sách kho hàng.
        }
    }
}
