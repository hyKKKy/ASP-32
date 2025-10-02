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

        [HttpGet("{id}")]
        public RestResponce GroupBySlug(String id)
        {
            var pg = _dataAccessor.GetProductGroupBySlug(id);

            return new()
            {
                Status = pg == null ? RestStatus.Status404 : RestStatus.Status200,
                Meta = new()
                {
                    Manipulations = ["GET"],
                    Cache = 60 * 60,
                    Service = "Shop API: products of group by slug",
                    DataType = pg == null ? "null" : "json/object"
                },
                Data = pg
            };
        }

        [HttpPost]
        public RestResponce AddGroup(AdminGroupFormModel model)
        {

            RestResponce restResponce = new()
            {
                Meta = new()
                {
                    Manipulations = ["POST"],
                    Cache = 0,
                    Service = "Shop API: add product group",
                    DataType = "json/object"
                },
                Data = null
            };
            // Валідація
            if (!ModelState.IsValid)
            {
                restResponce.Status = RestStatus.Status400;
                return restResponce;
            }

            bool exist =  _dataContext.ProductGroups.Any(g => g.Name == model.Name);

            if (exist)
            {
                ModelState.AddModelError("group-name", "Група з такою назвою вже створена");
                restResponce.Status = RestStatus.Status500;
                return restResponce;
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
                restResponce.Status = RestStatus.Status200;
                restResponce.Data = new { Name = model.Name };
                return restResponce;
            }
            catch (Exception ex)
            {
                restResponce.Status = RestStatus.Status500;
                restResponce.Data = ex.Message;
                return restResponce;
            }
        }
    }
}
