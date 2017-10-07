using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Blades.Interfaces;
using Blades.Auth.Interfaces;
using System.Security.Claims;
using System.Net.Http;
using System.Net;

namespace Blades.Auth.Controllers
{
    [Route("api/[controller]/[action]")]
    public class CookieAuthController : Controller
    {
        public const string AuthScheme = "CookieAuth";

        private IAuthManager authManager;
        public CookieAuthController(IAuthManager authManager)
        {
            this.authManager = authManager;
        }


        [HttpPost]
        public async Task<HttpResponseMessage> Login(AuthModel model)
        {
            ClaimsPrincipal principal = null;
            bool authSuccess = authManager.TryAuthenticateUser(model.Login, model.Password, out principal);
            if (authSuccess && principal != null)
            {
                await HttpContext.Authentication.SignInAsync(AuthScheme, principal);
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                return response;
            }
            else
            {
                var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                return response;
            }
        }



        [HttpPost]
        public async Task<HttpResponseMessage> Logout(AuthModel model)
        {
            await HttpContext.Authentication.SignOutAsync(AuthScheme);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }



        public class AuthModel
        {
            public string Login { get; set; }

            public string Password { get; set; }
        }
    }
}
