using System.Collections.Generic;
using BuiTanThanh_2280602928_W3.Models;

namespace BuiTanThanh_2280602928_W3.ViewModels
{
    public class ProductCategoryGroupViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public List<Product> Products { get; set; } = new List<Product>();
    }

    public class ProductIndexViewModel
    {
        public List<ProductCategoryGroupViewModel> Groups { get; set; } = new List<ProductCategoryGroupViewModel>();
        public int? SelectedCategoryId { get; set; }
    }
}
