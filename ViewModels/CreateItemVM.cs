using Microsoft.AspNetCore.Mvc.Rendering;
using Shopping_Cart_2.Attributes;
using Shopping_Cart_2.Sittings;
using System.ComponentModel.DataAnnotations;

namespace Shopping_Cart_2.ViewModels
{
    public class CreateItemVM : BaseItemVM
    {
        // Ảnh bìa của mặt hàng, kiểm tra định dạng và kích thước file
        [AllowedExtensions(FileSettings.AllowedExtensions)] // Chỉ cho phép các định dạng trong FileSettings
        [MaxFileSize(FileSettings.MaxFileSizeInBytes)] // Giới hạn kích thước file theo FileSettings
        public IFormFile Cover { get; set; } = default!; // File ảnh bìa khi tạo mặt hàng

        // Thông tin kho hàng liên quan đến mặt hàng
        Stock Stock { get; set; } = default!; // Đối tượng Stock để quản lý số lượng
    }
}