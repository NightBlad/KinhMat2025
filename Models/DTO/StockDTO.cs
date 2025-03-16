namespace Shopping_Cart_2.Models.DTO
{
    public class StockDTO
    {
        public int ItemId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải là giá trị không âm.")]
        public int Quantity { get; set; } = 0;
    }
}
