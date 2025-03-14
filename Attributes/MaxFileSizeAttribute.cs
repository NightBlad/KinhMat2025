using System.ComponentModel.DataAnnotations;

namespace Shopping_Cart_2.Attributes
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;

        public MaxFileSizeAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        protected override ValidationResult? IsValid
            (object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;

            if (file is not null)
            {
                if (file.Length > _maxFileSize)
                {
                    return new ValidationResult($"Dung lượng tệp tối đa cho phép là {_maxFileSize} byte");
                }
            }

            return ValidationResult.Success;
        }
    }
}
