using ASP_32.Data.Entities;
using ASP_32.Services.Kdf;
using Microsoft.EntityFrameworkCore;

namespace ASP_32.Data
{
    public class DataAccessor(DataContext dataContext, IKdfService kdfService)
    {
        public readonly DataContext _dataContext = dataContext;
        private readonly IKdfService _kdfService = kdfService;

        public UserAccess? Authenticate(String login, String password)
        {
            var userAccess = _dataContext
                .UserAccesses
                .AsNoTracking()
                .Include(ua => ua.User)
                .Include(ua => ua.Role)
                .FirstOrDefault(ua => ua.Login == login);
            if (userAccess == null)
            {
                return null;
            }
            String dk = _kdfService.Dk(password, userAccess.Salt);
            if (dk != userAccess.Dk)
            {
                return null;
            }

            return userAccess;
        }
        public IEnumerable<ProductGroup> GetProductGroups() 
        {
            return _dataContext
                .ProductGroups
                .Where(g => g.DeletedAt == null)
                .AsEnumerable();
        }
    }
}
