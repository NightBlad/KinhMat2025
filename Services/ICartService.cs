namespace Shopping_Cart_2.Services
{
    public interface ICartService
    {
        // Lấy giỏ hàng của người dùng dựa trên ID người dùng
        Task<ShoppingCart> GetCart(string userId);

        // Thêm một mặt hàng (sách) vào giỏ hàng với số lượng xác định
        Task<int> AddItem(int bookId, int qty);

        // Xóa một mặt hàng (sách) khỏi giỏ hàng
        Task<int> RemoveItem(int bookId);

        // Lấy toàn bộ thông tin giỏ hàng của người dùng hiện tại
        Task<ShoppingCart> GetUserCart();

        // Đếm số lượng mặt hàng trong giỏ hàng, có thể truyền ID người dùng hoặc không
        Task<int> GetCartItemCount(string userId = "");

        // Thực hiện quá trình thanh toán với thông tin từ CheckoutModel
        Task<bool> DoCheckout(CheckoutModel model);
    }
}