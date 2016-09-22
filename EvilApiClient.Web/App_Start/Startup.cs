using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.SignalR;

[assembly: OwinStartup(typeof(EvilApiClient.Web.App_Start.Startup))]

namespace EvilApiClient.Web.App_Start
{
    public class Startup
    {
        
        public void Configuration(IAppBuilder app)
        {
            //var idProvider = new CustomUserIdProvider();

            //GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => idProvider);

            
            //// For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            //System.Web.Helpers.AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Email;

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Home"),
                CookieSecure = CookieSecureOption.SameAsRequest
            });
            app.MapSignalR();
        }

     
    }

    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(IRequest request)
        {
            // your logic to fetch a user identifier goes here.

            // for example:

            return request.User.Identity.Name;
            
        }
    }

}
