using Microsoft.AspNetCore.Mvc.Rendering;

namespace Shopping_Cart_2.ViewModels
{
    public class CreateOrderVM
    {
        public int Id { get; set; } // Mã đơn hàng

        public int Quantity { get; set; } // Số lượng sản phẩm trong đơn hàng

        // Hai thuộc tính dưới đây dùng cho danh sách lựa chọn (SelectList)
        // để có thể chọn nhiều sản phẩm trong một đơn hàng
        public List<int> SelectedItems { get; set; } = new List<int>(); // Danh sách ID các sản phẩm được chọn

        public IEnumerable<SelectListItem> Items { get; set; } = Enumerable.Empty<SelectListItem>();
        // Danh sách các sản phẩm có thể chọn (hiển thị dưới dạng dropdown)
    }
}
