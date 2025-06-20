using Microsoft.AspNetCore.Mvc;
using BuiTanThanh_2280602928_W3.Data;
using BuiTanThanh_2280602928_W3.Models;

namespace BuiTanThanh_2280602928_W3.Controllers
{
    public class TableController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TableController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var tables = _context.Tables.ToList();
            return View(tables);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Table table)
        {
            if (ModelState.IsValid)
            {
                _context.Tables.Add(table);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(table);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            var table = _context.Tables.Find(id);
            if (table == null) return NotFound();
            return View(table);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Table table)
        {
            if (id != table.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(table);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(table);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            var table = _context.Tables.Find(id);
            if (table == null) return NotFound();
            return View(table);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var table = _context.Tables.Find(id);
            if (table != null)
            {
                _context.Tables.Remove(table);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult ChangeStatus(int id, string status)
        {
            var table = _context.Tables.Find(id);
            if (table != null)
            {
                table.Status = status;
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
