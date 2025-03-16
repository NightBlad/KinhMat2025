using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shopping_Cart_2.Constants;

namespace Shopping_Cart_2.Controllers
{
    [Authorize] // Xác thực người dùng trước khi truy cập controller
    public class RolesController : Controller
    {
        private readonly UserManager<IdentityUser> _user; // Quản lý người dùng
        private readonly RoleManager<IdentityRole> _roles; // Quản lý vai trò

        public RolesController(UserManager<IdentityUser> user, RoleManager<IdentityRole> roles)
        {
            _user = user;
            _roles = roles;
        }

        public async Task<IActionResult> Index()
        {
            var _users = await _user.Users.ToListAsync(); // Lấy danh sách người dùng
            return View(_users);
        }

        public async Task<IActionResult> addRoles(string userId)
        {
            var user = await _user.FindByIdAsync(userId); // Tìm người dùng theo ID
            var userRoles = await _user.GetRolesAsync(user); // Lấy danh sách vai trò của người dùng

            var allRoles = await _roles.Roles.ToListAsync(); // Lấy danh sách tất cả vai trò
            if (allRoles != null)
            {
                var roleList = allRoles.Select(r => new roleViewModel()
                {
                    roleId = r.Id, // Mã vai trò
                    roleName = r.Name, // Tên vai trò
                    useRole = userRoles.Any(x => x == r.Name) // Kiểm tra xem người dùng có vai trò này không
                });

                ViewBag.userName = user.UserName; // Truyền tên người dùng vào ViewBag
                ViewBag.userId = userId; // Truyền ID người dùng vào ViewBag
                return View(roleList);
            }
            else
                return NotFound(); // Trả về lỗi 404 nếu không tìm thấy vai trò nào
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Bảo vệ chống tấn công giả mạo CSRF
        public async Task<IActionResult> addRoles(string userId, string jsonRoles)
        {
            var user = await _user.FindByIdAsync(userId); // Tìm người dùng theo ID

            List<roleViewModel> myRoles =
                JsonConvert.DeserializeObject<List<roleViewModel>>(jsonRoles); // Chuyển đổi chuỗi JSON thành danh sách vai trò

            if (user != null)
            {
                var userRoles = await _user.GetRolesAsync(user); // Lấy danh sách vai trò hiện tại của người dùng

                foreach (var role in myRoles)
                {
                    if (userRoles.Any(x => x == role.roleName.Trim()) && !role.useRole)
                    {
                        await _user.RemoveFromRoleAsync(user, role.roleName.Trim()); // Xóa vai trò nếu người dùng không chọn nữa
                    }

                    if (!userRoles.Any(x => x == role.roleName.Trim()) && role.useRole)
                    {
                        await _user.AddToRoleAsync(user, role.roleName.Trim()); // Thêm vai trò nếu người dùng chọn
                    }
                }

                return RedirectToAction(nameof(Index)); // Quay lại danh sách người dùng
            }
            else
                return NotFound(); // Trả về lỗi 
        }
    }
}
