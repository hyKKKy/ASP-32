using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ASP_32.Data.Entities
{
    public record CartItem
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }

        [Column(TypeName = "decimal(14,2)")]
        public double Price { get; set; }
        public int Quantity { get; set; } = 1;
        public Guid? DiscountId { get; set; }

        [JsonIgnore]
        public Cart Cart { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
