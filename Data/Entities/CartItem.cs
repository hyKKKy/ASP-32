using System.ComponentModel.DataAnnotations.Schema;

namespace ASP_32.Data.Entities
{
    public class CartItem
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }

        [Column(TypeName = "decimal(14,2)")]
        public double Price { get; set; }
        public int Quantity { get; set; } = 1;
        public Guid? DiscountId { get; set; }


        public Cart Cart { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
