namespace Shopping_Cart_2.Services
{
    public class CartService(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor,
        UserManager<IdentityUser> userManager) : ICartService
    {
        private readonly ApplicationDbContext _db = db;
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        // Lấy ID của người dùng hiện đang được xác thực
        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext?.User; // Người dùng hiện đang được xác thực
            if (principal != null)
            {
                string userId = _userManager.GetUserId(principal) ?? throw new InvalidOperationException("User ID is null.");
                return userId;
            }
            throw new InvalidOperationException("No authenticated user found.");
        }

        // Lấy giỏ hàng của một người dùng cụ thể
        public async Task<ShoppingCart> GetCart(string userId)
        {
            var cart = await _db.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == userId);
            if (cart == null)
            {
                cart = new ShoppingCart
                {
                    UserId = userId,
                    CartDetails = []
                };
                await _db.ShoppingCarts.AddAsync(cart);
                await _db.SaveChangesAsync();
            }
            return cart;
        }

        // Thêm một mặt hàng vào giỏ hàng
        public async Task<int> AddItem(int itmId, int qty)
        {
            string userId = GetUserId();
            using var transaction = _db.Database.BeginTransaction();

            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("Người dùng chưa đăng nhập");
                }

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

                var cartDItem = await _db.CartDetails
                                      .FirstOrDefaultAsync(a => a.ShoppingCartId == ShCart.Id && a.ItemId == itmId);
                if (cartDItem is not null)
                {
                    cartDItem.Quantity += qty;
                    await _db.SaveChangesAsync();
                }
                else
                {
                    var item = await _db.Items.FindAsync(itmId);
                    if (item != null)
                    {
                    }
                    else
                    {
                        throw new InvalidOperationException("Item not found.");
                    }

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
                transaction.Commit();
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine(ex.Message);
            }
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
                if (ShCart is not null)
                {
                }
                else
                    throw new InvalidOperationException("Giỏ hàng không hợp lệ");
                var cartDItem = _db.CartDetails
                                  .FirstOrDefault(a => a.ShoppingCartId == ShCart.Id && a.ItemId == itmId);
                if (cartDItem is null)
                    throw new InvalidOperationException("Không có mặt hàng nào trong giỏ");
                else
                    _db.CartDetails.Remove(cartDItem);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine(ex.Message);
            }
            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }

        // Lấy toàn bộ giỏ hàng của người dùng cùng với các liên kết trong các bảng liên quan
        public async Task<ShoppingCart> GetUserCart()
        {
            var userId = GetUserId();
            if (userId != null)
            {
                var shoppingCart = await _db.ShoppingCarts
                                              .Include(a => a.CartDetails)
                                              .ThenInclude(a => a.Item)
                                              .ThenInclude(a => a.Stock) // Dùng để xem kho hàng
                                              .Include(b => b.CartDetails)
                                              .ThenInclude(b => b.Item)
                                              .ThenInclude(b => b.Category) // Liên kết với danh mục
                                              .Where(a => a.UserId == userId)
                                              .FirstOrDefaultAsync();

                if (shoppingCart == null)
                {
                    shoppingCart = new ShoppingCart
                    {
                        UserId = userId,
                        CartDetails = []
                    };
                    await _db.ShoppingCarts.AddAsync(shoppingCart);
                    await _db.SaveChangesAsync();
                }

                return shoppingCart;
            }

            throw new InvalidOperationException("ID người dùng không hợp lệ");
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
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException("Người dùng chưa đăng nhập");

                var ShCart = await GetCart(userId);
                if (ShCart is not null)
                {
                    var cartDetail = _db.CartDetails
                                                    .Where(a => a.ShoppingCartId == ShCart.Id).ToList();
                    if (cartDetail.Count == 0)
                        throw new InvalidOperationException("Giỏ hàng trống");

                    var pendingRecord = _db.OrderStatuses.FirstOrDefault(s => s.StatusName == "Pending");
                    if (pendingRecord is not null)
                    {
                    }
                    else
                        throw new InvalidOperationException("Trạng thái đơn hàng không có trạng thái Đang chờ");

                    Order order = new()
                    {
                        UserId = userId,
                        CreateDate = DateTime.UtcNow,
                        OrderStatusId = pendingRecord.Id,
                        Name = model.Name,
                        Email = model.Email,
                        MobileNumber = model.MobileNumber,
                        PaymentMethod = model.PaymentMethod.ToString(),
                        Address = model.Address,
                        IsPaid = false
                    };
                    await _db.Orders.AddAsync(order);
                    await _db.SaveChangesAsync();

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

                        var stock = await _db.Stocks.FirstOrDefaultAsync(a => a.ItemId == c.ItemId);
                        if (stock is not null)
                        {
                            if (c.Quantity > stock.Quantity)
                            {
                                throw new InvalidOperationException($"Chỉ có {stock.Quantity} mặt hàng trong kho");
                            }
                            stock.Quantity -= c.Quantity;
                        }
                        else
                        {
                            throw new InvalidOperationException("Kho hàng trống");
                        }
                    }

                    _db.CartDetails.RemoveRange(cartDetail);
                    _db.SaveChanges();
                    transaction.Commit();
                    return true;
                }

                throw new InvalidOperationException("Giỏ hàng không hợp lệ");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}