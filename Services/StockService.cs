using Microsoft.EntityFrameworkCore;
using Shopping_Cart_2.Data;

namespace Shopping_Cart_2.Services
{
    public class StockService : IStockService
    {
        private readonly ApplicationDbContext _db;
        private readonly IUserService _userService;

        // Khởi tạo dịch vụ với các dependency cần thiết
        public StockService(ApplicationDbContext db, IUserService userService)
        {
            _db = db;
            _userService = userService;
        }

        // Lấy thông tin kho hàng theo ID của mặt hàng
        public async Task<Stock?> GetStockByItemId(int itemId)
        {
            var st = await _db.Stocks.Include(x => x.Item) // Bao gồm thông tin mặt hàng
                                     .FirstOrDefaultAsync(s => s.ItemId == itemId); // Lấy kho hàng đầu tiên hoặc null
            return st;
        }

        // Lấy danh sách kho hàng dưới dạng StockDisplayModel của người dùng hiện tại
        public async Task<IEnumerable<StockDisplayModel>> GetStocks()
        {
            var userId = _userService.GetUserId(); // Lấy ID người dùng hiện tại
            if (userId == null)
                throw new UnauthorizedAccessException("Người dùng chưa đăng nhập");

            // Lấy danh sách mặt hàng cùng thông tin kho của người dùng và chuyển đổi thành StockDisplayModel
            var itemsWithStock = await _db.Items
                                         .Include(x => x.Stock) // Bao gồm thông tin kho
                                         .Where(x => x.UserId == userId) // Lọc theo ID người dùng
                                         .Select(i => new StockDisplayModel
                                         {
                                             ItemId = i.Id, // Gán ID mặt hàng
                                             Quantity = i.Stock != null ? i.Stock.Quantity : 0, // Gán số lượng (0 nếu không có kho)
                                             ItemName = i.Name // Gán tên mặt hàng
                                         }).ToListAsync(); // Trả về danh sách bất đồng bộ

            return itemsWithStock;
        }

        // Quản lý kho hàng: thêm mới hoặc cập nhật số lượng
        public async Task ManageStock(StockDTO stModel)
        {
            // Kiểm tra xem đã có kho cho mặt hàng này chưa
            var existingStock = await GetStockByItemId(stModel.ItemId);
            if (existingStock is null)
            {
                // Nếu chưa có, tạo mới bản ghi kho
                var stock = new Stock { ItemId = stModel.ItemId, Quantity = stModel.Quantity };
                await _db.Stocks.AddAsync(stock); // Thêm kho mới vào cơ sở dữ liệu
            }
            else
            {
                // Nếu đã có, cập nhật số lượng
                existingStock.Quantity = stModel.Quantity;
            }
            await _db.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu
        }
    }
}