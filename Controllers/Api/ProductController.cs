using ASP_32.Data;
using ASP_32.Data.Entities;
using ASP_32.Models;
using ASP_32.Models.Api;
using ASP_32.Services.Storage;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

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

        

        [HttpPost]
        public object AddProduct(ApiProductFormModel model)
        {
            #region Валідація моделі
            Guid groupGuid;
            try { groupGuid = Guid.Parse(model.GroupId); }
            catch { return new { status = "Invalid GroupId", code = 400 }; }
            #endregion

            if (!ModelState.IsValid)
            {
                return new { ModelState };
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
                return new { status = "OK", code = 200 };
            }
            catch (Exception ex)
            {
                return new { status = ex.Message, code = 500 };
            }
        }
    }
}
