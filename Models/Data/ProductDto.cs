using System.ComponentModel.DataAnnotations.Schema;

namespace ASP_32.Models.Data
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public String Name { get; set; } = null!;
        public String? Description { get; set; } = null!;
        public String? Slug { get; set; } = null!;
        public String? ImageUrl { get; set; } = null!;
        public int Stock { get; set; }
        public double Price { get; set; }
        
    }
}
