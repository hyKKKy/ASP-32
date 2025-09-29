using ASP_32.Data;
using ASP_32.Models.Rest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace ASP_32.Controllers.Api
{
    [Route("api/cart")]
    [ApiController]
    public class CartController(
            DataAccessor dataAccessor
        ) : ControllerBase
    {
        private readonly DataAccessor _dataAccessor = dataAccessor; 

        [HttpPost("{id}")]
        public RestResponce AddProduct(String id)
        {
            RestResponce restResponce = new()
            {
                Meta = new()
                { 
                    Service = "Shop API 'User cart'. Add Product to cart",
                    ServerTime = DateTime.Now.Ticks,

                }
            };
            if(HttpContext.User.Identity?.IsAuthenticated ?? false){
                try
                {
                    String userId = HttpContext.User.Claims
                    .First(c => c.Type == ClaimTypes.PrimarySid).Value;
                    _dataAccessor.AddToCart(userId, id);
                }
                catch(Exception ex) when (ex is ArgumentNullException) 
                {
                    restResponce.Status = RestStatus.Status400;
                    restResponce.Data = ex.Message;
                }
                catch (Exception ex) when (ex is InvalidOperationException)
                {
                    restResponce.Status = RestStatus.Status401;
                    restResponce.Data = "Error user identification";
                }
                catch (Exception ex) when (ex is FormatException)
                {
                    restResponce.Status = RestStatus.Status400;
                    restResponce.Data = ex.Message;
                }

            }
            else
            {
                restResponce.Status = RestStatus.Status200;
            }
            return restResponce;
        }
    }
}
