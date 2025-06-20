using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace BuiTanThanh_2280602928_W3.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [StringLength(20)]
        public string? Phone { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Required]
        public string Role { get; set; } = "customer"; // admin, staff, customer
    }
}
