namespace Shopping_Cart_2.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor,
            UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        // Lấy ID của người dùng hiện đang được xác thực
        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User; // Người dùng hiện đang được xác thực
            string userId = _userManager.GetUserId(principal);
            return userId;
        }

        // Lấy giỏ hàng của một người dùng cụ thể
        public async Task<ShoppingCart> GetCart(string userId)
        {
            var cart = await _db.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == userId);
            return cart;
        }

        // Thêm một mặt hàng vào giỏ hàng
        public async Task<int> AddItem(int itmId, int qty)
        {
            // userId => ShCart => cartDItem
            string userId = GetUserId();
            // Bắt đầu giao dịch
            using var transaction = _db.Database.BeginTransaction();

            try
            {
                // 1- Lấy ID của người dùng
                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("Người dùng chưa đăng nhập");
                }

                // 2- Lấy giỏ hàng của người dùng
                var ShCart = await GetCart(userId);
                if (ShCart is null)
                {
                    ShCart = new ShoppingCart
                    {
                        UserId = userId
                    };
                    await _db.ShoppingCarts.AddAsync(ShCart);
                }
                await _db.SaveChangesAsync();

                // 3- Lấy chi tiết giỏ hàng mà ID của nó bằng ID giỏ hàng của người dùng hiện tại
                // và ID mặt hàng bằng ID được truyền vào khi nhấn nút
                var cartDItem = await _db.CartDetails
                                  .FirstOrDefaultAsync(a => a.ShoppingCartId == ShCart.Id && a.ItemId == itmId);
                if (cartDItem is not null)
                {
                    cartDItem.Quantity += qty; // Tăng số lượng nếu mặt hàng đã có trong giỏ
                    await _db.SaveChangesAsync();
                }
                else
                {
                    var item = await _db.items.FindAsync(itmId);

                    cartDItem = new CartDetail
                    {
                        ItemId = itmId,
                        ShoppingCartId = ShCart.Id,
                        Quantity = qty,
                        UnitPrice = item.Price
                    };
                    await _db.CartDetails.AddAsync(cartDItem);
                }
                await _db.SaveChangesAsync();
                transaction.Commit(); // Xác nhận giao dịch
            }
            catch (Exception ex) { }
            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }

        // Xóa một mặt hàng khỏi giỏ hàng
        public async Task<int> RemoveItem(int itmId)
        {
            string userId = GetUserId();
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException("Người dùng chưa đăng nhập");
                var ShCart = await GetCart(userId);
                if (ShCart is null)
                    throw new InvalidOperationException("Giỏ hàng không hợp lệ");
                // Phần chi tiết giỏ hàng
                var cartDItem = _db.CartDetails
                                  .FirstOrDefault(a => a.ShoppingCartId == ShCart.Id && a.ItemId == itmId);
                if (cartDItem is null)
                    throw new InvalidOperationException("Không có mặt hàng nào trong giỏ");
                else if (cartDItem.Quantity == 1)
                    _db.CartDetails.Remove(cartDItem); // Xóa mặt hàng nếu số lượng bằng 1
                else
                    cartDItem.Quantity = cartDItem.Quantity - 1; // Giảm số lượng nếu lớn hơn 1
                _db.SaveChanges();
            }
            catch (Exception ex) { }
            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }

        // Lấy toàn bộ giỏ hàng của người dùng cùng với các liên kết trong các bảng liên quan
        public async Task<ShoppingCart> GetUserCart()
        {
            var userId = GetUserId();
            if (userId == null)
                throw new InvalidOperationException("ID người dùng không hợp lệ");
            var shoppingCart = await _db.ShoppingCarts
                                  .Include(a => a.CartDetails)
                                  .ThenInclude(a => a.Item)
                                  .ThenInclude(a => a.Stock) // Dùng để xem kho hàng
                                  .Include(b => b.CartDetails)
                                  .ThenInclude(b => b.Item)
                                  .ThenInclude(b => b.Category) // Liên kết với danh mục
                                  .Where(a => a.UserId == userId)
                                  .FirstOrDefaultAsync();
            return shoppingCart;
        }

        // Đếm số lượng mặt hàng trong giỏ hàng
        public async Task<int> GetCartItemCount(string userId = "")
        {
            if (string.IsNullOrEmpty(userId)) // Nếu không truyền userId, lấy ID người dùng hiện tại
            {
                userId = GetUserId();
            }
            var sh = await _db.ShoppingCarts.Include(x => x.CartDetails).SingleOrDefaultAsync(x => x.UserId == userId);
            if (sh == null)
            {
                return 0;
            }
            var data = sh.CartDetails.Sum(x => x.Quantity);

            var totalQuantity = await (from cart in _db.ShoppingCarts
                                       join cartDetail in _db.CartDetails
                                       on cart.Id equals cartDetail.ShoppingCartId
                                       where cart.UserId == userId
                                       select cartDetail.Quantity).SumAsync();
            return data;
        }

        // Thực hiện thanh toán
        public async Task<bool> DoCheckout(CheckoutModel model)
        {
            using var transaction = _db.Database.BeginTransaction();
            try
            {
                // 1- Lấy ID người dùng
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException("Người dùng chưa đăng nhập");

                // 2- Lấy giỏ hàng của người dùng
                var ShCart = await GetCart(userId);
                if (ShCart is null)
                    throw new InvalidOperationException("Giỏ hàng không hợp lệ");

                // 3- Lấy tất cả chi tiết giỏ hàng của giỏ hàng đó
                var cartDetail = _db.CartDetails
                                    .Where(a => a.ShoppingCartId == ShCart.Id).ToList();
                if (cartDetail.Count == 0)
                    throw new InvalidOperationException("Giỏ hàng trống");

                // Kiểm tra trạng thái "Đang chờ"
                var pendingRecord = _db.orderStatuses.FirstOrDefault(s => s.StatusName == "Pending");
                if (pendingRecord is null)
                    throw new InvalidOperationException("Trạng thái đơn hàng không có trạng thái Đang chờ");

                // 4- Tạo đơn hàng mới và thêm vào cơ sở dữ liệu
                Order order = new Order
                {
                    UserId = userId,
                    CreateDate = DateTime.UtcNow,
                    OrderStatusId = pendingRecord.Id, // Trạng thái đang chờ
                    Name = model.Name,
                    Email = model.Email,
                    MobileNumber = model.MobileNumber,
                    PaymentMethod = model.PaymentMethod,
                    Address = model.Address,
                    IsPaid = false
                };
                await _db.Orders.AddAsync(order);
                await _db.SaveChangesAsync();

                // 5- Tạo chi tiết đơn hàng cho mỗi chi tiết giỏ hàng
                foreach (var c in cartDetail)
                {
                    var orderDetail = new OrderDetail
                    {
                        ItemId = c.ItemId,
                        OrderId = order.Id,
                        Quantity = c.Quantity,
                        UnitPrice = c.UnitPrice
                    };
                    await _db.OrderDetails.AddAsync(orderDetail);

                    // Cập nhật kho hàng tại đây
                    var stock = await _db.Stocks.FirstOrDefaultAsync(a => a.ItemId == c.ItemId);
                    if (stock is null)
                    {
                        throw new InvalidOperationException("Kho hàng trống");
                    }
                    if (c.Quantity > stock.Quantity)
                    {
                        throw new InvalidOperationException($"Chỉ có {stock.Quantity} mặt hàng trong kho");
                    }
                    // Giảm số lượng trong bảng kho
                    stock.Quantity -= c.Quantity;
                }

                // 6- Xóa chi tiết giỏ hàng
                _db.CartDetails.RemoveRange(cartDetail);
                _db.SaveChanges();
                transaction.Commit(); // Xác nhận giao dịch
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}