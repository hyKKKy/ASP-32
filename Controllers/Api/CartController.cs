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
        private RestResponce restResponce = new()
        {
            Meta = new()
            {
                Service = "Shop API 'User cart'.",
                ServerTime = DateTime.Now.Ticks,
            }
        };


        private String imgPath => $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/Storage/Item/";

        [HttpGet]
        public RestResponce GetActiveCart()
        {
            RestResponce restResponce = new()
            {
                Meta = new()
                {
                    Service = "Shop API 'User cart'. Get Active Cart",
                    ServerTime = DateTime.Now.Ticks,

                }
            };
            if (HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                try
                {
                    String userId = HttpContext.User.Claims
                    .First(c => c.Type == ClaimTypes.PrimarySid).Value;

                    var activeCart = _dataAccessor.GetActiveCart(userId);

                    restResponce.Data = activeCart == null ? null :
                        activeCart with
                        {
                            CartItems = activeCart
                            .CartItems
                            .Select(ci => ci with {
                                Product = ci.Product with
                                {
                                    ImageUrl = imgPath + (ci.Product.ImageUrl ?? "no-image.jpg")
                                }
                            }).ToList()
                        };
                }
                catch (Exception ex) when (ex is ArgumentNullException)
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
                restResponce.Status = RestStatus.Status401;
            }
            return restResponce;
        }

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
            if (HttpContext.User.Identity?.IsAuthenticated ?? false) {
                try
                {
                    String userId = HttpContext.User.Claims
                    .First(c => c.Type == ClaimTypes.PrimarySid).Value;
                    _dataAccessor.AddToCart(userId, id);
                }
                catch (Exception ex) when (ex is ArgumentNullException)
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
                restResponce.Status = RestStatus.Status401;
            }
            return restResponce;
        }


        [HttpPatch("{id}")]
        public RestResponce UpdateCartItem(String id, [FromQuery] int cnt) //cart item id
        {
            this.restResponce.Meta.Service += "Update cart Item for " + cnt;
            ExecuteAuthorized((userId) => this.restResponce.Data = _dataAccessor.UpdateCartItem(userId, id, cnt)
            );
            return this.restResponce;
        }

        [HttpDelete("{id}")]
        public RestResponce DeleteCartItem(String id) //cart item id
        {
            this.restResponce.Meta.Service += "Delete cart Item.";
            ExecuteAuthorized((userId) => this.restResponce.Data = _dataAccessor.DeleteCartItem(userId, id)
            );

            return this.restResponce;
        }

        [HttpDelete]
        public RestResponce DeleteCart()
        {
            return new();
        }

        private void ExecuteAuthorized(Action<String> action)
        {
            if (HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                try
                {
                    String userId = HttpContext.User.Claims
                    .First(c => c.Type == ClaimTypes.PrimarySid).Value;

                    action(userId);
                }
                catch (Exception ex) when (ex is ArgumentNullException)
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
                restResponce.Status = RestStatus.Status401;
            }
        }

    }
}
