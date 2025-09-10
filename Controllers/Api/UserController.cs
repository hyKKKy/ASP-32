using ASP_32.Data;
using ASP_32.Services.Auth;
using ASP_32.Services.Kdf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Text.Json;

/* Д.З. Реалізувати повний цикл перевірок даних, що передаються
             * для автентифікації
             * - заголовок починається з 'Basic '
             * - credentials успішно декодуються з Base64
             * - userPass ділиться на дві частини (може не містити ":")
             */

namespace ASP_32.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(
        DataContext dataContext, 
        IKdfService kdfService,
        IAuthService authService
        ) : ControllerBase
    {
        private readonly DataContext _dataContext = dataContext;

        private readonly IKdfService _kdfService = kdfService;

        private readonly IAuthService _authService = authService;

        [HttpGet]
        public object Authenticate()
        {
            String? header = HttpContext.Request.Headers.Authorization;
            if (header == null)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return new { Status = "Authorization Header Required" };
            }
            
            //ДЗ -1
            String[] splitParts = header.Split(" ", 2);
            if (splitParts[0] != "Basic")
            {
                HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return new { Status = "Credentials rejected!" };
            }

            String credentials =    // 'Basic ' - length = 6
                header[6..];        // QWxhZGRpbjpvcGVuIHNlc2FtZQ==

            //ДЗ -2
            String userPass; 
            try 
            {
                String tmp = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(credentials));
                userPass = tmp;
            }
            catch(Exception ex)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return new { Status = "Invalid Base64 decoding", ex };
                }

            //ДЗ -3
            if (!userPass.Contains(':')) 
            {
                HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return new { Status = "Credentials rejected*" };
            }
            String[] parts = userPass.Split(':', 2);
            String login = parts[0];
            String password = parts[1];

            var userAccess = _dataContext
                .UserAccesses
                .AsNoTracking()
                .Include(ua => ua.User)
                .Include(ua => ua.Role)
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

            //HttpContext.Session.SetString(
            //    "UserController:Authenticate",
            //    JsonSerializer.Serialize(userAccess)
            //);

            _authService.SetAuth(userAccess);
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
