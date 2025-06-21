using Microsoft.AspNetCore.Mvc;
using BuiTanThanh_2280602928_W3.Models;
using BuiTanThanh_2280602928_W3.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BuiTanThanh_2280602928_W3.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string CartSessionKey = "CartSession";

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị giỏ hàng
        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        // Thêm sản phẩm vào giỏ
        [HttpGet]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound();
            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                item.Quantity += quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ImageUrl = product.ImageUrl,
                    UnitPrice = product.Price,
                    Quantity = quantity
                });
            }
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // Xóa sản phẩm khỏi giỏ
        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                cart.Items.Remove(item);
                SaveCart(cart);
            }
            return RedirectToAction("Index");
        }

        // Cập nhật nhiều sản phẩm trong giỏ hàng
        [HttpPost]
        public IActionResult UpdateCart(int[] productIds, int[] quantities)
        {
            var cart = GetCart();
            for (int i = 0; i < productIds.Length; i++)
            {
                var item = cart.Items.FirstOrDefault(x => x.ProductId == productIds[i]);
                if (item != null)
                {
                    item.Quantity = quantities[i];
                    if (item.Quantity <= 0)
                    {
                        cart.Items.Remove(item);
                    }
                }
            }
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // Hiển thị form đặt hàng
        [Authorize]
        public IActionResult Checkout()
        {
            var cart = GetCart();
            if (cart.Items.Count == 0) return RedirectToAction("Index");
            // Lấy danh sách bàn còn trống
            var tables = _context.Tables.Where(t => t.Status == "available").ToList();
            ViewBag.Tables = tables;
            return View(cart);
        }

        // Đặt hàng
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(string orderType, int? tableId, string? address)
        {
            var cart = GetCart();
            if (cart.Items.Count == 0) return RedirectToAction("Index");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Challenge();
            // Validate: Nếu orderType là table thì phải chọn tableId, nếu delivery thì phải nhập address
            if (orderType == "table" && tableId == null)
            {
                ModelState.AddModelError("tableId", "Vui lòng chọn bàn.");
            }
            if (orderType == "delivery" && string.IsNullOrWhiteSpace(address))
            {
                ModelState.AddModelError("address", "Vui lòng nhập địa chỉ giao hàng.");
            }
            if (!ModelState.IsValid)
            {
                // Lấy lại danh sách bàn để render lại view
                var tables = _context.Tables.Where(t => t.Status == "available").ToList();
                ViewBag.Tables = tables;
                return View("Checkout", cart);
            }
            // Tạo Order
            var order = new Order
            {
                CreatedAt = DateTime.Now,
                TotalAmount = cart.GrandTotal,
                UserId = userId,
                TableId = orderType == "table" ? tableId : null,
                Address = orderType == "delivery" ? address : null,
                OrderItems = cart.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            // Xóa giỏ hàng
            SaveCart(new CartViewModel());
            return RedirectToAction("OrderSuccess");
        }

        public IActionResult OrderSuccess()
        {
            return View();
        }

        // Lịch sử đơn hàng
        [Authorize]
        public async Task<IActionResult> OrderHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Challenge();
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
            return View(orders);
        }

        // Helper: Lấy giỏ hàng từ Session
        private CartViewModel GetCart()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            if (!string.IsNullOrEmpty(cartJson))
            {
                return JsonSerializer.Deserialize<CartViewModel>(cartJson) ?? new CartViewModel();
            }
            return new CartViewModel();
        }

        // Helper: Lưu giỏ hàng vào Session
        private void SaveCart(CartViewModel cart)
        {
            var cartJson = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(CartSessionKey, cartJson);
        }
    }
}
