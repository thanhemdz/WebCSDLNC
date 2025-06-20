using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BuiTanThanh_2280602928_W3.Models
{
    public class Order
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
        public string? StaffId { get; set; }
        public ApplicationUser? Staff { get; set; }
        public int? TableId { get; set; }
        public Table? Table { get; set; }
        [StringLength(255)]
        public string? Address { get; set; }
        [Required]
        public decimal TotalAmount { get; set; }
        [Required]
        public string Status { get; set; } = "pending"; // pending, preparing, done, cancelled
        [Required]
        public string PaymentStatus { get; set; } = "unpaid"; // unpaid, paid
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<OrderItem>? OrderItems { get; set; }
        public ICollection<Payment>? Payments { get; set; }
    }
}
