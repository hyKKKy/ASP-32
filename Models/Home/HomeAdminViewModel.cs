using ASP_32.Data.Entities;

namespace ASP_32.Models.Home
{
    public class HomeAdminViewModel
    {
        public IEnumerable<ProductGroup> ProductGroups { get; set; } = [];
    }
}
