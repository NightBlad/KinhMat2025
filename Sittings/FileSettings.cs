namespace Shopping_Cart_2.Sittings
{
    public class FileSettings
    {
        // Đường dẫn lưu trữ ảnh của các mặt hàng
        public const string ImagesPath = "/assets/images/items";

        // Danh sách các định dạng file được phép tải lên
        public const string AllowedExtensions = ".jpg,.jpeg,.png";

        // Kích thước tối đa của file tính bằng MB
        public const int MaxFileSizeInMB = 1;

        // Kích thước tối đa của file tính bằng byte (chuyển đổi từ MB)
        public const int MaxFileSizeInBytes = MaxFileSizeInMB * 1024 * 1024;
    }
}