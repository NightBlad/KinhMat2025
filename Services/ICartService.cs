namespace Shopping_Cart_2.Services
{
    public interface ICartService
    {
        // Lấy giỏ hàng của người dùng dựa trên ID người dùng
        Task<ShoppingCart> GetCart(string userId);


        Task<int> AddItem(int ItemId, int qty);

        Task<int> RemoveItem(int ItemId);

        // Lấy toàn bộ thông tin giỏ hàng của người dùng hiện tại
        Task<ShoppingCart> GetUserCart();


        Task<int> GetCartItemCount(string userId = "");

        // Thực hiện quá trình thanh toán với thông tin từ CheckoutModel
        Task<bool> DoCheckout(CheckoutModel model);
    }
}