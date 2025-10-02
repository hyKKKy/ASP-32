using ASP_32.Data;
using ASP_32.Data.Entities;
using ASP_32.Models;
using ASP_32.Models.Api;
using ASP_32.Models.Rest;
using ASP_32.Services.Storage;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;

namespace ASP_32.Controllers.Api
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IStorageService _storageService;
        private readonly DataAccessor _dataAccessor;
        private readonly DataContext _dataContext;

        public ProductController(
            IStorageService storageService,
            DataAccessor dataAccessor,
            DataContext dataContext)
        {
            _storageService = storageService;
            _dataAccessor = dataAccessor;
            _dataContext = dataContext;
        }

        [HttpPost("feedback/{id}")]
        public RestResponce AddFeedback(String id, int? rate, String? comment)
        {
            RestResponce restResponce = new(){
                Meta = new()
                {
                    Manipulations = ["POST"],
                    Cache = 60 * 60,
                    Service = "Shop API: product feedback",
                    DataType = "null",
                    Opt = {
                        { "id", id},
                        { "rate", rate ?? -1},
                        { "comment", comment ?? ""},
                    },
                },
                Data = null
            };
            Guid? userId = null;

            if (HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                userId = Guid.Parse(HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Sid)
                    .Value);
            }

            var product = _dataAccessor.GetProductBySlug(id);
            if(product == null)
            {
                restResponce.Status = RestStatus.Status404;
                return restResponce;
            }
            _dataContext.Feedbacks.Add(new()
            { 
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            UserId = userId,
            Comment = comment,
            Rate = rate,
            CreatedAt = DateTime.Now
            });
            _dataContext.SaveChanges();

            return restResponce;
            
        }

        [HttpPost]
        public RestResponce AddProduct(ApiProductFormModel model)
        {
            RestResponce restResponce = new()
            {
                Meta = new()
                {
                    Manipulations = ["POST"],
                    Cache = 60 * 60,
                    Service = "Shop API: add product",
                    DataType = "null",
                    Opt = new Dictionary<String, Object>
                    {
                        { "Name", model.Name },
                        { "Description", model.Description },
                        { "Slug", model.Slug },
                        { "Price", model.Price },
                        { "Stock", model.Stock },
                        { "GroupId", model.GroupId },
                        { "Image", model.Image != null ? model.Image.FileName : "null" }
                    },
                },
                Data = null
            };

            #region Валідація моделі
            Guid groupGuid;
            try { groupGuid = Guid.Parse(model.GroupId); }
            catch { restResponce.Status = RestStatus.Status400;
                return restResponce;
            }
            #endregion

            if (!ModelState.IsValid)
            {
                restResponce.Status = RestStatus.Status400;
                return restResponce;
            }

            _dataContext.Products.Add(new()
            {
                Id = Guid.NewGuid(),
                GroupId = groupGuid,
                Name = model.Name,
                Description = model.Description,
                Slug = model.Slug,
                Price = model.Price,
                Stock = model.Stock,
                ImageUrl = model.Image == null ? null :
                    _storageService.Save(model.Image),
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
