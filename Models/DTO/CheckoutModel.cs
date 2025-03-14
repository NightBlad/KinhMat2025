namespace Shopping_Cart_2.Models.DTO
{
    public class CheckoutModel
    {
        [Required]
        [MaxLength(30)]
        public string? Name { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(30)]
        public string? Email { get; set; }

        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "The mobile number must be 10 digits.")]
        public string? MobileNumber { get; set; }

        [Required]
        [MaxLength(200)]
        public string? Address { get; set; }

        [Required]
        public string? PaymentMethod { get; set; }
    }
}
