using ASP_32.Data;
using ASP_32.Services.Kdf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP_32.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(DataContext dataContext, IKdfService kdfService) : ControllerBase
    {
        private readonly DataContext _dataContext = dataContext;

        private readonly IKdfService _kdfService = kdfService;

        [HttpGet]
        public object Authenticate()
        {
            String? header = HttpContext.Request.Headers.Authorization;
            if (header == null)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return new { Status = "Authorization Header Required" };
            }
            String credentials = header[6..];
            String userPass = System.Text.Encoding.UTF8.GetString(
                Convert.FromBase64String(credentials));
            String[] parts = userPass.Split(':', 2);
            String login = parts[0];
            String password = parts[1];

            var userAccess = _dataContext
                .UserAccesses
                .FirstOrDefault(ua => ua.Login == login);

            if (userAccess == null) 
            { 
                HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return new { Status = "Credentials rejected" };
            }
            String dk = _kdfService.Dk(password, userAccess.Salt);
            if(dk != userAccess.Dk)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return new { Status = "Credentials rejected." };
            }

            return userAccess;
        }

        [HttpPost]
        public object SingUp()
        {
            return new { Status = "SingUp Works" };
        }

        [HttpPost("admin")]
        public object SingUpAdmin()
        {
            return new { Status = "SingUpAdmin Works" };
        }
    }
}
