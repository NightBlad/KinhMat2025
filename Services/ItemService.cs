using Humanizer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shopping_Cart_2.Data;
using Shopping_Cart_2.Models;
using Shopping_Cart_2.ViewModels;

namespace Shopping_Cart_2.Services
{
    public class ItemService : IItemService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _imagesPath;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRatingService _ratingService;

        // Khởi tạo dịch vụ với các dependency cần thiết
        public ItemService(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor, IRatingService ratingService)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _imagesPath = $"{_webHostEnvironment.WebRootPath}/assets/images/items"; // Đường dẫn lưu ảnh mặt hàng
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _ratingService = ratingService;
        }

        // Lấy ID của người dùng hiện đang được xác thực
        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User; // Người dùng hiện tại
            string userId = _userManager.GetUserId(principal);
            return userId;
        }

        // Lưu file ảnh bìa và trả về tên file
        public async Task<string> SaveCover(IFormFile cover)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(cover.FileName)}"; // Tạo tên file duy nhất
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "images", "items", fileName);

            // Đảm bảo thư mục tồn tại
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using var stream = new FileStream(path, FileMode.Create);
            await cover.CopyToAsync(stream); // Lưu file vào hệ thống

            return fileName;
        }

        // Lấy tất cả các mặt hàng đã được phê duyệt
        public IEnumerable<Item> GetAll()
        {
            var Item = _context.Items.Include(x => x.Category) // Bao gồm thông tin danh mục
                                     .Include(x => x.Stock) // Bao gồm thông tin kho
                                     .Include(x => x.Ratings) // Bao gồm đánh giá
                                     .Where(x => x.IsApproved == true) // Chỉ lấy mặt hàng đã phê duyệt
                                     .AsNoTracking() // Không theo dõi để tối ưu hiệu suất
                                     .ToList();

            // Tính điểm đánh giá trung bình cho từng sản phẩm
            foreach (var d in Item)
            {
                d.ProductAverageRate = _ratingService.GetProductRate(d.Id);
            }
            return Item;
        }

        // Lấy các mặt hàng theo ID người dùng
        public IEnumerable<Item> GetItemsByUserId()
        {
            var userId = GetUserId();
            if (userId == null)
                throw new UnauthorizedAccessException("Người dùng chưa đăng nhập");

            var Item = _context.Items.Include(x => x.Category) // Bao gồm thông tin danh mục
                                     .Include(x => x.Stock) // Bao gồm thông tin kho
                                     .Include(x => x.Ratings) // Bao gồm đánh giá
                                     .Where(x => x.UserId == userId) // Lọc theo ID người dùng
                                     .AsNoTracking() // Không theo dõi để tối ưu hiệu suất
                                     .ToList();

            return Item;
        }

        // Lấy thông tin một mặt hàng theo ID
        public Item? GetById(int id)
        {
            var Item = _context.Items.Include(x => x.Category) // Bao gồm thông tin danh mục
                                     .Include(x => x.Stock) // Bao gồm thông tin kho
                                     .Include(x => x.Ratings) // Bao gồm đánh giá
                                     .AsNoTracking() // Không theo dõi để tối ưu hiệu suất
                                     .SingleOrDefault(g => g.Id == id); // Lấy mặt hàng duy nhất hoặc null
            return Item;
        }

        // Tạo một mặt hàng mới
        public async Task Create(CreateItemVM vmItem, Stock st)
        {
            var userId = GetUserId();
            if (userId == null)
                throw new InvalidOperationException("ID người dùng không hợp lệ");

            var coverName = await SaveCover(vmItem.Cover); // Lưu ảnh bìa
            st.Quantity = vmItem.Quantity; // Gán số lượng từ VM vào Stock

            Item item = new()
            {
                Name = vmItem.Name,
                Description = vmItem.Description,
                Price = vmItem.Price,
                Cover = coverName,
                CategoryId = vmItem.CategoryId,
                UserId = userId,
                Stock = st // Gán thông tin kho
            };
            await _context.Items.AddAsync(item); // Thêm mặt hàng vào cơ sở dữ liệu
            await _context.SaveChangesAsync(); // Lưu thay đổi
        }

        // Cập nhật thông tin một mặt hàng
        public async Task<Item?> Update(EditItemVM vmItem)
        {
            // Lấy mặt hàng từ cơ sở dữ liệu
            var item = await _context.Items.Include(g => g.Category) // Bao gồm thông tin danh mục
                                            .Include(x => x.Stock) // Bao gồm thông tin kho
                                            .SingleOrDefaultAsync(g => g.Id == vmItem.Id);
            if (item == null) return null;

            // Cập nhật thông tin mặt hàng
            item.Name = vmItem.Name;
            item.Description = vmItem.Description;
            item.Price = vmItem.Price;
            item.CategoryId = vmItem.CategoryId;
            item.Stock.Quantity = vmItem.Quantity;

            // Kiểm tra và cập nhật ảnh bìa nếu có
            var hasNewCover = vmItem.Cover is not null;
            var oldCover = item.Cover; // Lưu tên ảnh bìa cũ để xóa nếu cần

            if (hasNewCover)
            {
                item.Cover = await SaveCover(vmItem.Cover!); // Lưu ảnh bìa mới
            }

            var effectedRows = _context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu

            if (effectedRows > 0)
            {
                if (hasNewCover)
                {
                    var cover = Path.Combine(_imagesPath, oldCover);
                    File.Delete(cover); // Xóa ảnh bìa cũ
                }
                return item;
            }
            else
            {
                // Nếu cập nhật thất bại, xóa ảnh bìa mới vừa lưu
                var cover = Path.Combine(_imagesPath, item.Cover);
                File.Delete(cover);
                return null;
            }
        }

        // Xóa một mặt hàng
        public bool Delete(int id)
        {
            var isDeleted = false;

            var item = _context.Items.Find(id); // Tìm mặt hàng theo ID
            if (item is null)
                return isDeleted;

            _context.Remove(item); // Xóa mặt hàng khỏi cơ sở dữ liệu
            var effectedRows = _context.SaveChanges(); // Lưu thay đổi

            if (effectedRows > 0)
            {
                isDeleted = true;
                var cover = Path.Combine(_imagesPath, item.Cover);
                File.Delete(cover); // Xóa ảnh bìa của mặt hàng
            }

            return isDeleted;
        }
    }
}