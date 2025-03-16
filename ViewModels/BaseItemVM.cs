using Microsoft.AspNetCore.Mvc.Rendering;
using Shopping_Cart_2.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Shopping_Cart_2.ViewModels
{
    public class BaseItemVM
    {
        // Tên của mặt hàng, mặc định là chuỗi rỗng
        public string Name { get; set; } = string.Empty;

        // Mô tả của mặt hàng, bắt buộc nhập
        [Required]
        public string? Description { get; set; } = string.Empty;

        // Giá của mặt hàng, bắt buộc nhập và phải lớn hơn 0
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Giá trị phải lớn hơn 0.")]
        public double Price { get; set; }

        // Số lượng có sẵn của mặt hàng, bắt buộc nhập và phải lớn hơn 0
        [Required]
        [Display(Name = "Số lượng có sẵn")]
        [Range(1, int.MaxValue, ErrorMessage = "Giá trị phải lớn hơn 0.")]
        public int Quantity { get; set; }

        // ID danh mục của mặt hàng, dùng để liên kết với danh mục
        [Display(Name = "Danh mục")]
        // Dùng để điền danh sách dropdown trong view (danh mục)
        public int CategoryId { get; set; } = 0; // Thuộc tính asp-for trong view

        // Danh sách các danh mục dưới dạng SelectListItem để hiển thị trong dropdown
        public IEnumerable<SelectListItem> Categories { get; set; } = Enumerable.Empty<SelectListItem>(); // Thuộc tính asp-items trong view
    }
}