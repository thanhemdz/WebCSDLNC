using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BuiTanThanh_2280602928_W3.Data;
using BuiTanThanh_2280602928_W3.Models;
using System.Globalization;

namespace BuiTanThanh_2280602928_W3.Controllers
{
    public class ShiftController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ShiftController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị lịch ca dạng bảng tuần
        public async Task<IActionResult> Index(DateTime? weekStart)
        {
            // Nếu không truyền vào thì lấy thứ 2 tuần hiện tại
            DateTime startOfWeek = weekStart ?? DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);
            DateTime endOfWeek = startOfWeek.AddDays(6);
            var shifts = await _context.Shifts.Include(s => s.Staff)
                .Where(s => s.Date >= startOfWeek && s.Date <= endOfWeek)
                .ToListAsync();
            ViewBag.StartOfWeek = startOfWeek;
            ViewBag.EndOfWeek = endOfWeek;
            // Lấy user có role Staff
            var staffRole = SD.Role_Staff;
            var staffUsers = _context.Users
                .Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == _context.Roles.FirstOrDefault(r => r.Name == staffRole).Id))
                .ToList();
            ViewBag.AllStaff = staffUsers;
            return View(shifts);
        }

        // Thêm ca làm mới
        public IActionResult Create()
        {
            ViewBag.AllStaff = _context.Users.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Shift shift)
        {
            if (ModelState.IsValid)
            {
                _context.Shifts.Add(shift);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.AllStaff = _context.Users.ToList();
            return View(shift);
        }

        // Xóa ca làm
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var shift = await _context.Shifts.Include(s => s.Staff).FirstOrDefaultAsync(s => s.Id == id);
            if (shift == null) return NotFound();
            return View(shift);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shift = await _context.Shifts.FindAsync(id);
            if (shift != null)
            {
                _context.Shifts.Remove(shift);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveSchedule()
        {
            Console.WriteLine("Saving schedule called");
            var form = Request.Form;
            Console.WriteLine("Form keys: " + form);
            var startOfWeek = DateTime.Parse(form["startOfWeek"]);
            var days = Enumerable.Range(0, 7).Select(i => startOfWeek.AddDays(i)).ToList();
            var shiftTypes = Enum.GetValues(typeof(ShiftType)).Cast<ShiftType>().ToList();
            Console.WriteLine($"Start of week: {startOfWeek.ToString("yyyy-MM-dd")}, Days count: {days.Count}, Shift types count: {shiftTypes.Count}");
            foreach (var day in days)
            {
                foreach (var shiftType in shiftTypes)
                {
                    var prefix = $"shift_{day:yyyyMMdd}_{shiftType}";
                    var staffId = form[$"{prefix}_staffId"];
                    Console.WriteLine($"Processing shift for {day:yyyy-MM-dd} - {shiftType}: Staff ID = {staffId}");
                    var date = day;
                    var type = shiftType;
                    var exist = await _context.Shifts.FirstOrDefaultAsync(s => s.Date.Date == date.Date && s.ShiftType == type);
                    if (!string.IsNullOrWhiteSpace(staffId))
                    {
                        if (exist != null)
                        {
                            exist.StaffId = staffId;
                        }
                        else
                        {
                            _context.Shifts.Add(new Shift { Date = date, ShiftType = type, StaffId = staffId });
                        }
                    }
                    else
                    {
                        if (exist != null)
                        {
                            _context.Shifts.Remove(exist);
                        }
                    }
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
