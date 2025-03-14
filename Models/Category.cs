using System.ComponentModel.DataAnnotations;

namespace Shopping_Cart_2.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;

        // Thiết lập quan hệ 1-N (One-to-Many) giữa Category và Item
        public ICollection<Item?> Items { get; set; }=new List<Item>();

    }
}
