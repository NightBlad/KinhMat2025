using Shopping_Cart_2.Models;
using Shopping_Cart_2.ViewModels;

namespace Shopping_Cart_2.Services
{
    public interface IItemService
    {
        // Lấy tất cả các mặt hàng từ cơ sở dữ liệu
        IEnumerable<Item> GetAll();

        // Lấy danh sách các mặt hàng theo ID của người dùng
        IEnumerable<Item> GetItemsByUserId();

        // Lấy thông tin một mặt hàng cụ thể theo ID
        Item? GetById(int id);

        // Tạo một mặt hàng mới dựa trên dữ liệu từ CreateItemVM và thông tin kho (Stock)
        Task Create(CreateItemVM vmItem, Stock st);

        // Cập nhật thông tin một mặt hàng dựa trên dữ liệu từ EditItemVM
        Task<Item?> Update(EditItemVM vmItem);

        // Xóa một mặt hàng theo ID và trả về kết quả thành công/thất bại
        bool Delete(int id);
    }
}