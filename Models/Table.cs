using System.ComponentModel.DataAnnotations;

namespace BuiTanThanh_2280602928_W3.Models
{
    public class Table
    {
        public int Id { get; set; }
        [Required, StringLength(50)]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Status { get; set; } = "available"; // available, occupied, reserved
    }
}
