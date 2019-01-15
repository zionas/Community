
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Linq;
using CommunityNetWork.Dal.Interfaces;
using System;
using System.Net.Http;

namespace Authentication.BL
{
    public class AuthorizeValidator : AuthorizeAttribute
    {
        
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var headers = actionContext.Request.Headers;
            var tokenUser = headers.GetValues("TokenUser");

            if (tokenUser.Count() < 2)
                return false;
            string email = tokenUser.ElementAt(0);
            string token = tokenUser.ElementAt(1);

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("http://localhost:60562/");
                var response = httpClient.GetAsync($"api/Authentication/CheckValidToken?email={email}token={token}").Result;
                return true;
            }


        }
    }
}
