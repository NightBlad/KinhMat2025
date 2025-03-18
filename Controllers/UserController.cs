using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Shopping_Cart_2.Models;
using Shopping_Cart_2.ViewModels;

namespace Shopping_Cart_2.Controllers
{
    public class UserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager) : Controller
    {
        private readonly UserManager<IdentityUser> _userManager = userManager; // Quản lý người dùng.
        private readonly RoleManager<IdentityRole> _roleManager = roleManager; // Quản lý vai trò.

        // Hiển thị danh sách tất cả người dùng.
        public async Task<IActionResult> Index()
        {
            var users = await Task.Run(() => _userManager.Users.ToList()); // Lấy danh sách tất cả người dùng.
            return View(users); // Trả về giao diện danh sách người dùng.
        }

        // Hiển thị form tạo người dùng mới.
        public IActionResult Create()
        {
            ViewBag.Roles = _roleManager.Roles.ToList(); // Lấy danh sách vai trò hiện có.
            return View(); // Trả về giao diện tạo người dùng.
        }

        // Xử lý tạo người dùng mới.
        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid) // Kiểm tra dữ liệu đầu vào hợp lệ.
            {
                var user = new IdentityUser { UserName = model.UserName, Email = model.Email, EmailConfirmed = model.EmailConfirmed };
                var result = await _userManager.CreateAsync(user, model.Password); // Tạo người dùng mới với mật khẩu.

                if (result.Succeeded) // Nếu tạo thành công.
                {
                    if (!string.IsNullOrEmpty(model.Role)) // Nếu có chọn vai trò.
                    {
                        await _userManager.AddToRoleAsync(user, model.Role); // Gán vai trò cho người dùng.
                    }
                    return RedirectToAction(nameof(Index)); // Chuyển hướng về danh sách người dùng.
                }

                // Nếu có lỗi, thêm vào ModelState để hiển thị trên giao diện.
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewBag.Roles = _roleManager.Roles.ToList(); // Lấy lại danh sách vai trò.
            return View(model); // Trả về giao diện với dữ liệu đã nhập.
        }

        // Hiển thị form chỉnh sửa thông tin người dùng.
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id); // Tìm người dùng theo ID.
            if (user == null)
            {
                return NotFound(); // Nếu không tìm thấy, trả về lỗi 404.
            }

            var userRoles = await _userManager.GetRolesAsync(user); // Lấy vai trò hiện tại của người dùng.
            var model = new EditUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                EmailConfirmed = user.EmailConfirmed,
                Role = userRoles.FirstOrDefault() ?? string.Empty // Lấy vai trò đầu tiên nếu có.
            };

            ViewBag.Roles = _roleManager.Roles.ToList(); // Lấy danh sách vai trò để chọn.
            return View(model); // Trả về giao diện chỉnh sửa người dùng.
        }

        // Xử lý cập nhật thông tin người dùng.
        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid) // Kiểm tra dữ liệu đầu vào hợp lệ.
            {
                var user = await _userManager.FindByIdAsync(model.Id); // Tìm người dùng theo ID.
                if (user == null)
                {
                    return NotFound(); // Nếu không tìm thấy, trả về lỗi 404.
                }

                // Cập nhật thông tin người dùng.
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.EmailConfirmed = model.EmailConfirmed;

                var result = await _userManager.UpdateAsync(user); // Lưu thay đổi.

                if (result.Succeeded) // Nếu cập nhật thành công.
                {
                    // Nếu có nhập mật khẩu mới, thực hiện đổi mật khẩu.
                    if (!string.IsNullOrEmpty(model.NewPassword))
                    {
                        var passwordChangeResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                        if (!passwordChangeResult.Succeeded) // Nếu có lỗi khi đổi mật khẩu.
                        {
                            foreach (var error in passwordChangeResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                            ViewBag.Roles = _roleManager.Roles.ToList();
                            return View(model);
                        }
                    }

                    // Cập nhật vai trò nếu có thay đổi.
                    var userRoles = await _userManager.GetRolesAsync(user);
                    if (!string.IsNullOrEmpty(model.Role) && !userRoles.Contains(model.Role))
                    {
                        await _userManager.RemoveFromRolesAsync(user, userRoles); // Xóa tất cả vai trò hiện tại.
                        await _userManager.AddToRoleAsync(user, model.Role); // Thêm vai trò mới.
                    }
                    return RedirectToAction(nameof(Index)); // Quay lại danh sách người dùng.
                }

                // Nếu có lỗi, thêm vào ModelState để hiển thị.
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewBag.Roles = _roleManager.Roles.ToList(); // Lấy danh sách vai trò.
            return View(model); // Trả về giao diện chỉnh sửa.
        }

        // Hiển thị xác nhận xóa người dùng.
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id); // Tìm người dùng theo ID.
            if (user == null)
            {
                return NotFound(); // Nếu không tìm thấy, trả về lỗi 404.
            }

            return View(user); // Trả về giao diện xác nhận xóa.
        }

        // Xử lý xóa người dùng.
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id); // Tìm người dùng theo ID.
            if (user == null)
            {
                return NotFound(); // Nếu không tìm thấy, trả về lỗi.
            }

            var result = await _userManager.DeleteAsync(user); // Xóa người dùng.

            if (result.Succeeded) // Nếu xóa thành công.
            {
                return RedirectToAction(nameof(Index)); // Quay lại danh sách người dùng.
            }

            // Nếu có lỗi, thêm vào ModelState để hiển thị.
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(user); // Trả về giao diện xóa.
        }
    }
}