using System.ComponentModel.DataAnnotations;

namespace Shopping_Cart_2.ViewModels
{
    public class CreateUserViewModel
    {
        [Required]
        public string UserName { get; set; } // Tên người dùng

        [Required]
        [EmailAddress]
        public string Email { get; set; } // Địa chỉ email

        public bool EmailConfirmed { get; set; } // Xác nhận email

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } // Mật khẩu

        public string Role { get; set; } // Vai trò người dùng
    }

    public class EditUserViewModel
    {
        public string Id { get; set; } // Mã người dùng

        [Required]
        public string UserName { get; set; } // Tên người dùng

        [Required]
        [EmailAddress]
        public string Email { get; set; } // Địa chỉ email

        public bool EmailConfirmed { get; set; } // Xác nhận email

        public string Role { get; set; } // Vai trò người dùng

        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } // Mật khẩu hiện tại

        [DataType(DataType.Password)]
        public string NewPassword { get; set; } // Mật khẩu mới

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu mới và xác nhận mật khẩu không khớp.")]
        public string ConfirmNewPassword { get; set; } // Xác nhận mật khẩu mới
    }
}
