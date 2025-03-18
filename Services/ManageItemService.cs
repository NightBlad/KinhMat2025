using Shopping_Cart_2.Models;

namespace Shopping_Cart_2.Services
{
    public class ManageItemService : IManageItemService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IItemService _itemService;

        // Khởi tạo dịch vụ với các dependency cần thiết
        public ManageItemService(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor, IItemService itemService)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _itemService = itemService;
        }

        // Lấy tất cả các mặt hàng từ cơ sở dữ liệu
        public async Task<IEnumerable<Item>> GetAllItems()
        {
            var Item = await _context.Items.Include(x => x.Category) // Bao gồm thông tin danh mục
                                           .Include(x => x.Stock) // Bao gồm thông tin kho
                                           .AsNoTracking() // Không theo dõi để tối ưu hiệu suất
                                           .ToListAsync(); // Trả về danh sách bất đồng bộ

            return Item;
        }

        // Chuyển đổi trạng thái phê duyệt của một mặt hàng
        public async Task ToggleApprovementStatus(int ItemId)
        {
            var item = await _context.Items.FindAsync(ItemId); // Tìm mặt hàng theo ID
            if (item == null)
            {
                throw new InvalidOperationException($"Mặt hàng với ID: {ItemId} không được tìm thấy");
            }
            item.IsApproved = !item.IsApproved; // Đảo ngược trạng thái phê duyệt (true thành false và ngược lại)
            await _context.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu
        }
    }
}