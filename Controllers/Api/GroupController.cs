using ASP_32.Data;
using ASP_32.Data.Entities;
using ASP_32.Models.Home;
using ASP_32.Models.Rest;
using ASP_32.Services.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ASP_32.Controllers.Api
{
    [Route("api/group")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IStorageService _storageService;
        private readonly DataAccessor _dataAccessor;
        private readonly DataContext _dataContext;

        public GroupController(IStorageService storageService, DataAccessor dataAccessor, DataContext dataContext)
        {
            _storageService = storageService;
            _dataAccessor = dataAccessor;
            _dataContext = dataContext;
        }

        [HttpGet]
        public RestResponce AllGroups()
        {
            return new()
            {
                //Status = RestStatus.Status400,
                Meta = new()
                {
                    Manipulations = ["GET", "POST"],
                    Cache = 24 * 60 * 60,
                    Service = "Shop API: product groups",
                    DataType = "json/array"
                },
                Data = _dataAccessor.GetProductGroups()
            };
        }

        [HttpPost]
        public object AddGroup(AdminGroupFormModel model)
        {
            // Валідація
            if (!ModelState.IsValid)
            {
                return new { ModelState };
            }

            bool exist =  _dataContext.ProductGroups.Any(g => g.Name == model.Name);

            if (exist)
            {
                ModelState.AddModelError("group-name", "Група з такою назвою вже створена");
                return ValidationProblem(ModelState);
            }

            _dataContext.ProductGroups.Add(new Data.Entities.ProductGroup
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Description = model.Description,
                Slug = model.Slug,
                ImageUrl = _storageService.Save(model.Image)
            });
            try
            {
                _dataContext.SaveChanges();
                return new { status = "OK", code = 200 };
            }
            catch (Exception ex)
            {
                return new { status = ex.Message, code = 500 };
            }
        }
    }
}
