﻿using System.ComponentModel.DataAnnotations.Schema;

namespace ASP_32.Data.Entities
{
    public record Cart
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public double Price { get; set; }
        public Guid? DiscountId { get; set; }
        public User User { get; set; } = null!;
        public ICollection<CartItem> CartItems { get; set; } = [];

    }
}
