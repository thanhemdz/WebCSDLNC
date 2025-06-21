using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BuiTanThanh_2280602928_W3.Models;
using BuiTanThanh_2280602928_W3.Data;

namespace BuiTanThanh_2280602928_W3.Controllers
{
    [Authorize(Roles = "admin")]
    public class StaffController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public StaffController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            // Lấy user có role staff (ưu tiên lấy từ UserManager + Role)
            var staff = _userManager.GetUsersInRoleAsync("staff").Result;
            return View(staff);
        }

        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string name, string email, string phone, string password)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin bắt buộc.");
                return View();
            }
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                Name = name,
                PhoneNumber = phone,
                Role = "staff"
            };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                // Gán role staff
                await _userManager.AddToRoleAsync(user, "staff");
                return RedirectToAction("Index");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View();
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null || !(await _userManager.IsInRoleAsync(user, "staff"))) return NotFound();
            return View(user);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string name, string phone)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null || !(await _userManager.IsInRoleAsync(user, "staff"))) return NotFound();
            user.Name = name;
            user.PhoneNumber = phone;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return RedirectToAction("Index");
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
            return View(user);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null || !(await _userManager.IsInRoleAsync(user, "staff"))) return NotFound();
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null || !(await _userManager.IsInRoleAsync(user, "staff"))) return NotFound();
            // Ràng buộc: Không cho xóa nếu có ca làm việc trong tuần hoặc tương lai
            var today = DateTime.Today;
            var weekStart = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday); // Thứ 2 tuần này
            var hasFutureShift = _context.Shifts.Any(s => s.StaffId == user.Id && s.Date >= weekStart);
            if (hasFutureShift)
            {
                TempData["Error"] = "Không thể xóa nhân viên đang có ca làm việc trong tuần hoặc tương lai.";
                return RedirectToAction("Index");
            }
            await _userManager.DeleteAsync(user);
            return RedirectToAction("Index");
        }
    }
}
