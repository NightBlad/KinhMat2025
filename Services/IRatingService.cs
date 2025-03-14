namespace Shopping_Cart_2.Services
{
    public interface IRatingService
    {
        // Đánh giá một sản phẩm với điểm số, ID sản phẩm và ID người dùng
        int RateProduct(int rate, int itemId, string userId);

        // Cập nhật đánh giá cho một sản phẩm với điểm số mới, ID sản phẩm và ID người dùng
        int UpdateRateProduct(int rate, int itemId, string userId);

        // Lấy điểm đánh giá của người dùng cho một sản phẩm cụ thể
        int GetUserRate(string userId, int itemId);

        // Lấy điểm đánh giá trung bình của một sản phẩm dựa trên ID
        double GetProductRate(int itemId);
    }
}