using Microsoft.AspNetCore.Mvc.Rendering;
using Shopping_Cart_2.Attributes;
using Shopping_Cart_2.Sittings;
using System.ComponentModel.DataAnnotations;

namespace Shopping_Cart_2.ViewModels
{
    public class EditItemVM : BaseItemVM
    {
        public int Id { get; set; } // Mã sản phẩm

        public string? CurrentCover { get; set; } // Ảnh bìa hiện tại của sản phẩm

        [AllowedExtensions(FileSettings.AllowedExtensions)] // Kiểm tra định dạng tệp được phép
        [MaxFileSize(FileSettings.MaxFileSizeInBytes)] // Giới hạn kích thước tệp tối đa
        public IFormFile? Cover { get; set; } = default!; // Ảnh bìa mới của sản phẩm
    }
}
