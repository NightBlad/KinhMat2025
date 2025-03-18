namespace Shopping_Cart_2.Models.DTO
{
    public class CheckoutModel
    {
        [Required(ErrorMessage = "Họ và Tên is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại is required")]
        [Phone(ErrorMessage = "Invalid Phone Number")]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "Địa chỉ is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Phương thức thanh toán is required")]
        public PaymentMethods PaymentMethod { get; set; }
    }
}
