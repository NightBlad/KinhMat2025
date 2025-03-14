using Microsoft.CodeAnalysis;
using System;

namespace Shopping_Cart_2.Services
{
    public class RatingService : IRatingService
    {
        private readonly ApplicationDbContext _db;
        private readonly IUserService _userService;

        // Khởi tạo dịch vụ với các dependency cần thiết
        public RatingService(IUserService userService, ApplicationDbContext db)
        {
            _userService = userService;
            _db = db;
        }

        // Đánh giá một sản phẩm với điểm số, ID sản phẩm và ID người dùng
        public int RateProduct(int rate, int itemId, string userId)
        {
            var rated = _db.Ratings?.Include(x => x.Item) // Bao gồm thông tin sản phẩm
                                   .Where(r => r.UserId == userId && r.ItemId == itemId) // Lọc theo người dùng và sản phẩm
                                   .FirstOrDefault(); // Lấy đánh giá đầu tiên hoặc null
            if (rated != null)
            {
                UpdateRateProduct(rate, itemId, userId); // Nếu đã có đánh giá, cập nhật nó
                return 1; // Trả về 1 để biểu thị cập nhật thành công
            }
            else
            {
                var rating = new Rating()
                {
                    RatingValue = rate, // Gán điểm đánh giá
                    UserId = userId, // Gán ID người dùng
                    ItemId = itemId, // Gán ID sản phẩm
                };
                _db.Ratings.Add(rating); // Thêm đánh giá mới vào cơ sở dữ liệu
            }
            return _db.SaveChanges(); // Lưu thay đổi và trả về số dòng bị ảnh hưởng
        }

        // Cập nhật đánh giá cho một sản phẩm
        public int UpdateRateProduct(int rate, int itemId, string userId)
        {
            var rating = _db.Ratings.Include(x => x.Item) // Bao gồm thông tin sản phẩm
                                    .Where(r => r.UserId == userId && r.ItemId == itemId) // Lọc theo người dùng và sản phẩm
                                    .SingleOrDefault(); // Lấy đánh giá duy nhất hoặc null
            if (rating == null)
            {
                return 0; // Trả về 0 nếu không tìm thấy đánh giá
            }
            rating.RatingValue = rate; // Cập nhật điểm đánh giá
            rating.Item.ProductAverageRate = GetProductRate(itemId); // Cập nhật điểm trung bình của sản phẩm
            _db.Ratings.Update(rating); // Cập nhật đánh giá trong cơ sở dữ liệu
            return _db.SaveChanges(); // Lưu thay đổi và trả về số dòng bị ảnh hưởng
        }

        // Lấy điểm đánh giá của người dùng cho một sản phẩm
        public int GetUserRate(string userId, int itemId)
        {
            var rate = _db.Ratings?.Include(x => x.Item) // Bao gồm thông tin sản phẩm
                                  .Where(r => r.UserId == userId && r.ItemId == itemId) // Lọc theo người dùng và sản phẩm
                                  .SingleOrDefault(); // Lấy đánh giá duy nhất hoặc null
            if (rate == null)
            {
                return -1; // Trả về -1 nếu không tìm thấy đánh giá
            }
            return rate.RatingValue; // Trả về điểm đánh giá của người dùng
        }

        // Lấy điểm đánh giá trung bình của một sản phẩm
        public double GetProductRate(int itemId)
        {
            var productRate = _db.Ratings.Include(x => x.Item) // Bao gồm thông tin sản phẩm
                                         .Where(r => r.ItemId == itemId) // Lọc theo ID sản phẩm
                                         .ToList(); // Lấy danh sách các đánh giá
            if (productRate.Count() == 0)
            {
                return -1; // Trả về -1 nếu không có đánh giá nào
            }
            var av = productRate.Average(r => r.RatingValue); // Tính trung bình điểm đánh giá
            return av; // Trả về điểm trung bình
        }
    }
}