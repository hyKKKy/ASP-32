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


        public ProductController(
            IStorageService storageService,
            DataAccessor dataAccessor)
        {
            _storageService = storageService;
            _dataAccessor = dataAccessor;
            
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
            _dataAccessor.AddFeedback(
                userId == null ? Guid.Empty.ToString() : userId.ToString(),
                product.Id.ToString(),
                comment ?? "",
                rate == null ? 0 : (int)rate
                );

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

            _dataAccessor.AddProduct(
                model.GroupId,
                model.Name,
                model.Description ?? "",
                model.Slug ?? "",
                model.Image != null ? model.Image.FileName : "no-image.jpg",
                model.Stock,
                model.Price
                );

            try
            {
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
