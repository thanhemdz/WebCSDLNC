using System;
using System.ComponentModel.DataAnnotations;

namespace BuiTanThanh_2280602928_W3.Models
{
    public enum ShiftType
    {
        Morning, // 7h-12h
        Afternoon, // 12h-17h
        Evening // 17h-22h
    }

    public class Shift
    {
        public int Id { get; set; }
        [Required]
        public string StaffId { get; set; } = string.Empty;
        public ApplicationUser? Staff { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } // Ngày làm việc
        [Required]
        public ShiftType ShiftType { get; set; } // Ca sáng/trưa/tối
    }
}
