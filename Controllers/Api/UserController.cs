using ASP_32.Data;
using ASP_32.Data.Entities;
using ASP_32.Models.Rest;
using ASP_32.Services.Auth;
using ASP_32.Services.Kdf;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ASP_32.Controllers.Api
{
    [Route("api/user")]
    [ApiController]
    public class UserController(
            DataContext dataContext, 
            IKdfService kdfService,
            IAuthService authService,
            IConfiguration configuration,
            DataAccessor dataAccessor ) : ControllerBase
    {
        private readonly DataContext _dataContext = dataContext;
        private readonly DataAccessor _dataAccessor = dataAccessor;
        private readonly IKdfService _kdfService = kdfService;
        private readonly IAuthService _authService = authService;
        private readonly IConfiguration _configuration = configuration;

        [HttpGet("jwt")]

        public RestResponce AuthenticateJwt()
        {
            RestResponce responce = new();
            UserAccess userAccess;
            try
            {
                var (login, password) = GetBasicCredentials();
                userAccess = _dataAccessor.Authenticate(login, password)
                    ?? throw new Exception("Credintials rejected1");
            }
            catch(Exception ex)
            {
                responce.Status = RestStatus.Status401;
                responce.Data = ex.Message;
                return responce;
            }

            var headerObject = new 
            {
                alg = "HS256",
                typ = "JWT"
            };
            String headerJson = JsonSerializer.Serialize(headerObject);
            String header64 = Base64UrlTextEncoder.Encode(System.Text.Encoding.UTF8.GetBytes(headerJson));

            var payloadObject = new

            {

                iss = "ASP-32",   // Issuer	Identifies principal that issued the JWT.

                sub = userAccess.UserId,   // Subject	Identifies the subject of the JWT.

                aud = userAccess.RoleId,   // Audience	Identifies the recipients that the JWT is intended for. Each principal intended to process the JWT must identify itself with a value in the audience claim. If the principal processing the claim does not identify itself with a value in the aud claim when this claim is present, then the JWT must be rejected.

                exp = DateTime.Now.AddMinutes(10),   // Expiration Time	Identifies the expiration time on and after which the JWT must not be accepted for processing. The value must be a NumericDate:[9] either an integer or decimal, representing seconds past 1970-01-01 00:00:00Z.

                nbf = DateTime.Now,   // Not Before	Identifies the time on which the JWT will start to be accepted for processing. The value must be a NumericDate.

                iat = DateTime.Now,   // Issued at	Identifies the time at which the JWT was issued. The value must be a NumericDate.

                jti = Guid.NewGuid(),   // JWT ID	Case-sensitive unique identifier of the token even among different issuers.iss	Issuer	Identifies principal that issued the JWT.

                name = userAccess.User.Name,
                email = userAccess.User.Email, 
            
            };


            String payloadJson = JsonSerializer.Serialize(payloadObject);
            String payload64 = Base64UrlTextEncoder.Encode(System.Text.Encoding.UTF8.GetBytes(payloadJson));

            String secret = _configuration.GetSection("Jwt").GetSection("Secret").Value
                ?? throw new KeyNotFoundException("Not found configuration 'Jwt.Secret'");

            String tokenBody = header64 + '.' + payload64;

            String signature = Base64UrlTextEncoder.Encode(
                System.Security.Cryptography.HMACSHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes(secret),
                System.Text.Encoding.UTF8.GetBytes(tokenBody)
                ));

            responce.Data = tokenBody + '.' + signature;

            return responce;
        }

        private (String, String) GetBasicCredentials()
        {
            String? header = HttpContext.Request.Headers.Authorization;
            if (header == null)      // Basic QWxhZGRpbjpvcGVuIHNlc2FtZQ==
            {
                throw new Exception("Authorization Header Required");
            }
            String credentials =    // 'Basic ' - length = 6
                header[6..];        // QWxhZGRpbjpvcGVuIHNlc2FtZQ==
            String userPass =       // Aladdin:open sesame
                System.Text.Encoding.UTF8.GetString(
                    Convert.FromBase64String(credentials));

            String[] parts = userPass.Split(':', 2);
            String login = parts[0];
            String password = parts[1];
            return(login, password);
        }

        public RestResponce Authenticate()
        {
            RestResponce restResponce = new()
            {
                Meta = new RestMeta
                {
                    Service = "ASP 32. AuthService",
                    Url = HttpContext.Request.Path,
                    Cache = 0,
                    Manipulations = new[] { "AUTH" },
                    DataType = "UserAccess"
                }
            };

            String? header = HttpContext.Request.Headers.Authorization;
            if (header == null)      // Basic QWxhZGRpbjpvcGVuIHNlc2FtZQ==
            {
                restResponce.Status = RestStatus.Status401;
                return restResponce;
            }
            try
            {
                String credentials =    // 'Basic ' - length = 6
                header[6..];        // QWxhZGRpbjpvcGVuIHNlc2FtZQ==
                String userPass =       // Aladdin:open sesame
                    System.Text.Encoding.UTF8.GetString(
                        Convert.FromBase64String(credentials));

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
                    restResponce.Status = RestStatus.Status401;
                    restResponce.Data = new { Error = "Credentials rejected." };
                    return restResponce;
                }
                String dk = _kdfService.Dk(password, userAccess.Salt);
                if (dk != userAccess.Dk)
                {
                    restResponce.Status = RestStatus.Status401;
                    restResponce.Data = new { Error = "Credentials rejected" };
                    return restResponce;
                }

                _authService.SetAuth(userAccess);
                restResponce.Status = RestStatus.Status200;
                restResponce.Data = new
                {
                    Id = userAccess.UserId,
                    Login = userAccess.Login,
                    User = userAccess.User?.Name,
                    Role = userAccess.Role.Id
                };
                return restResponce;
            }
            catch (Exception ex)
            {
                restResponce.Status = RestStatus.Status400;
                restResponce.Data = new { Error = ex.Message };
                return restResponce;
            }

        }


                [HttpPost]
        public RestResponce SignUp()
        {
            RestResponce restResponce = new() {
                
                Meta = new RestMeta
                {
                    Service = "ASP 32. AuthService",
                    Url = HttpContext.Request.Path,
                    Cache = 0,
                    Manipulations = new[] { "SIGNUP" },
                    DataType = "SingUp works"
                }

            };
            return restResponce;
        }

        [HttpPost("admin")]  // POST /api/user/admin
        public RestResponce SignUpAdmin()
        {
            RestResponce restResponce = new()
            {

                Meta = new RestMeta
                {
                    Service = "ASP 32. AuthService",
                    Url = HttpContext.Request.Path,
                    Cache = 0,
                    Manipulations = new[] { "SIGNUP" },
                    DataType = "SingUpAdmin works"
                }

            };
            return restResponce;
        }
    }
}
/* Відмінності АРІ та MVC контролерів
 * MVC:
 *  адресація за назвою дії (Action) - різні дії -- різні адреси
 *  GET  /Home/Index     --> HomeController.Index()
 *  POST /Home/Index     --> HomeController.Index()
 *  GET  /Home/Privacy   --> HomeController.Privacy()
 *  повернення - IActionResult частіше за все View
 *  
 * API:
 *  адресація за анотацією [Route("api/user")], різниця
 *  у методах запиту
 *  GET  api/user  ---> [HttpGet] Authenticate()
 *  POST api/user  ---> [HttpPost] SignUp()
 *  PUT  api/user  ---> 
 *  
 *  C   POST
 *  R   GET
 *  U   PUT(replace) PATCH(partially update)
 *  D   DELETE
 */
/* Авторизація. Схеми.
 * 0) Кукі (Cookie) - заголовки НТТР-пакету, які зберігаються у клієнта
 *      та автоматично включаються ним до всіх наступних запитів до сервера
 *      "+" простота використання
 *      "-" автоматизовано тільки у браузерах, в інших програмах це справа
 *           програміста. 
 *      "-" відкритість, легкість перехоплення даних
 *      
 * 1) Сесії (серверні): базуються на Кукі, проте всі дані зберігаються
 *     на сервері, у куках передається тільки ідентифікатор сесії
 *     "+" покращена безпека
 *     "-" велике навантаження на сховище сервера
 *     
 * 2) Токени (клієнтські): клієнт зберігає токен, який лише перевіряється
 *     сервером.
 *     "+" відмова від кукі та сесій
 *     "-" більше навантаження на роботу сервера
 *  2а) Токени-ідентифікатори
 *  2б) Токени з даними (JWT)
 */