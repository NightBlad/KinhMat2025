namespace Shopping_Cart_2.Services
{
    public interface IManageItemService
    {
        // Lấy tất cả các mặt hàng từ cơ sở dữ liệu
        Task<IEnumerable<Item>> GetAllItems();

        // Chuyển đổi trạng thái phê duyệt của một mặt hàng dựa trên ID
        Task ToggleApprovementStatus(int ItemId);
    }
}