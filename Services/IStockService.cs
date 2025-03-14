namespace Shopping_Cart_2.Services
{
    public interface IStockService
    {
        // Lấy danh sách các kho hàng dưới dạng StockDisplayModel
        Task<IEnumerable<StockDisplayModel>> GetStocks();

        // Lấy thông tin kho hàng theo ID của mặt hàng
        Task<Stock?> GetStockByItemId(int itemId);

        // Quản lý kho hàng dựa trên dữ liệu từ StockDTO
        Task ManageStock(StockDTO stockToManage);
    }
}