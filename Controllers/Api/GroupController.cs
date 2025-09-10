using Microsoft.AspNetCore.Mvc;
using ASP_32.Models.Home;
using ASP_32.Services.Storage;
using ASP_32.Data;

namespace ASP_32.Controllers.Api
{
    [Route("api/group")]
    [ApiController]
    public class GroupController(IStorageService storageService, DataContext dataContext) : ControllerBase
    {
        private readonly IStorageService _storageService = storageService;

        private readonly DataContext _dataContext = dataContext;

        [HttpPost]

        public object AddGroup(AdminGroupFormModel model)
        {
            _dataContext.ProductGroups.Add(new Data.Entities.ProductGroup.)
            {

            }
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
