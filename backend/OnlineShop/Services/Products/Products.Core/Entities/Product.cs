using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Core.Entities
{
    public class Product : Base
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal OldPrice { get; set; }
        public decimal NewPrice { get; set; }
        public string? ImagePath { get; set; }  // Relative path e.g. "uploads/products/image.jpg"
       
    }
}
