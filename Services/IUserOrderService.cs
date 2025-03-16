using Shopping_Cart_2.Models;
using System.Collections.Generic;

namespace Shopping_Cart_2.Services
{
    public interface IUserOrderService
    {
        // Lấy danh sách các đơn hàng của người dùng hiện tại
        Task<IEnumerable<Order>> UserOrders();

        // Lấy thông tin một đơn hàng cụ thể theo ID
        Task<Order?> GetOrderById(int id);

        // Lấy chi tiết của một đơn hàng dựa trên ID đơn hàng
        Task<Order> GetOrderDetail(int orderId);

        // Lấy danh sách các trạng thái đơn hàng (đã bị comment trong mã gốc)
        // Task<IEnumerable<OrderStatus>> GetOrderStatuses();

        // Thay đổi trạng thái của một đơn hàng dựa trên dữ liệu từ UpdateOrderStatusModel
        Task ChangeOrderStatus(UpdateOrderStatusModel data);

        // Chuyển đổi trạng thái thanh toán của một đơn hàng dựa trên ID
        Task TogglePaymentStatus(int orderId);

        // Lấy danh sách các mục dưới dạng SelectListItem để sử dụng trong dropdown
        IEnumerable<SelectListItem> GetSelectLists();

        // Lấy tất cả các đơn hàng trong hệ thống
        Task<IEnumerable<Order>> AllOrders();
    }
}