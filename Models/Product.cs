using System.ComponentModel.DataAnnotations;

namespace BuiTanThanh_2280602928_W3.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required, StringLength(255)]
        public string Name { get; set; } = string.Empty;
        [Required]
        public int CategoryId { get; set; }
        public ProductCategory? Category { get; set; }
        [Required]
        public decimal Price { get; set; }
        [StringLength(500)]
        public string? ImageUrl { get; set; }
        [Required]
        public string Status { get; set; } = "active"; // active, inactive
        public string? Description { get; set; }
    }
}
