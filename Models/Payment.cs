using System;
using System.ComponentModel.DataAnnotations;

namespace BuiTanThanh_2280602928_W3.Models
{
    public class Payment
    {
        public int Id { get; set; }
        [Required]
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        [Required]
        public string Method { get; set; } = string.Empty; // cash, card, momo, banking
        [Required]
        public decimal Amount { get; set; }
        public DateTime PaidAt { get; set; } = DateTime.Now;
    }
}
