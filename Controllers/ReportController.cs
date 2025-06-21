using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BuiTanThanh_2280602928_W3.Data;
using System.Globalization;
using BuiTanThanh_2280602928_W3.Models;
using Microsoft.EntityFrameworkCore;

namespace BuiTanThanh_2280602928_W3.Controllers
{
    [Authorize(Roles = "admin,staff")]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Revenue(string type = "day")
        {
            var orders = _context.Orders.Where(o => o.Status != "cancelled");
            object data;
            switch (type)
            {
                case "month":
                    data = orders
                        .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                        .Select(g => new
                        {
                            Year = g.Key.Year,
                            Month = g.Key.Month,
                            Total = g.Sum(x => x.TotalAmount)
                        }).ToList()
                        .Select(x => new
                        {
                            Period = $"{x.Year}-{x.Month:D2}",
                            x.Total
                        }).OrderBy(x => x.Period).ToList();
                    break;
                case "year":
                    data = orders
                        .GroupBy(o => o.CreatedAt.Year)
                        .Select(g => new
                        {
                            Year = g.Key,
                            Total = g.Sum(x => x.TotalAmount)
                        }).ToList()
                        .Select(x => new
                        {
                            Period = x.Year.ToString(),
                            x.Total
                        }).OrderBy(x => x.Period).ToList();
                    break;
                default:
                    data = orders
                        .GroupBy(o => o.CreatedAt.Date)
                        .Select(g => new
                        {
                            Date = g.Key,
                            Total = g.Sum(x => x.TotalAmount)
                        }).ToList()
                        .Select(x => new
                        {
                            Period = x.Date.ToString("yyyy-MM-dd"),
                            x.Total
                        }).OrderBy(x => x.Period).ToList();
                    break;
            }
            ViewBag.Type = type;
            return View(data);
        }

        [HttpGet]
        public IActionResult OrderDetails(string period, string type)
        {
            IQueryable<Order> orders = _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .Include(o => o.Table)
                .Where(o => o.Status != "cancelled");
            DateTime start, end;
            if (type == "day")
            {
                if (!DateTime.TryParse(period, out start))
                    return Content("Dữ liệu không hợp lệ");
                end = start.AddDays(1);
                orders = orders.Where(o => o.CreatedAt.Date == start.Date);
            }
            else if (type == "month")
            {
                var parts = period.Split('-');
                if (parts.Length != 2 || !int.TryParse(parts[0], out int year) || !int.TryParse(parts[1], out int month))
                    return Content("Dữ liệu không hợp lệ");
                start = new DateTime(year, month, 1);
                end = start.AddMonths(1);
                orders = orders.Where(o => o.CreatedAt.Year == year && o.CreatedAt.Month == month);
            }
            else if (type == "year")
            {
                if (!int.TryParse(period, out int year))
                    return Content("Dữ liệu không hợp lệ");
                start = new DateTime(year, 1, 1);
                end = start.AddYears(1);
                orders = orders.Where(o => o.CreatedAt.Year == year);
            }
            else
            {
                return Content("Dữ liệu không hợp lệ");
            }
            var orderList = orders.OrderByDescending(o => o.CreatedAt).ToList();
            return PartialView("_OrderDetailPartial", orderList);
        }

        [HttpGet]
        public IActionResult OrderItems(int orderId)
        {
            var order = _context.Orders
                .Where(o => o.Id == orderId)
                .Select(o => new
                {
                    Items = o.OrderItems.Select(oi => new
                    {
                        oi.Product,
                        oi.Quantity,
                        oi.UnitPrice
                    })
                })
                .FirstOrDefault();
            if (order == null)
                return Content("Không tìm thấy hóa đơn");
            // Lấy lại danh sách OrderItem đầy đủ để truyền vào partial
            var items = _context.OrderItems.Where(oi => oi.OrderId == orderId).ToList();
            return PartialView("_OrderItemsPartial", items);
        }
    }
}
