namespace ASP_32.Data.Entities
{
    public class ProductGroup
    {
        public Guid      Id          { get; set; }
        public Guid?     ParentId    { get; set; }
        public string    Name        { get; set; } = null!;
        public string    Description { get; set; } = null!;
        public String    Slug        { get; set; } = null!;
        public String    ImageUrl    { get; set; } = null!;
        public DateTime? DeletedAt   { get; set; }
        public ICollection<Product> Products { get; set; } = [];
    }
}
