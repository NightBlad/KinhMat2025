namespace Shopping_Cart_2.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;

        // Khởi tạo dịch vụ với các dependency cần thiết
        public UserService(IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        // Lấy ID của người dùng hiện đang được xác thực
        public string GetUserId()
        {
            return _userManager.GetUserId(_httpContextAccessor.HttpContext.User); // Trả về ID người dùng từ HttpContext
        }
    }
}