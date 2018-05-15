using CashMe.Data.DAL;
using CashMe.Shared.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace CashMe.Admin
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit https://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(CashMeContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                },
                CookieName = "CashMeCookie"

            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});
        }
         /// <summary>
        /// Cookie auth provider that adds extra role claims on the identity
        /// Role claims are kept in cache and added on the identity on every request
        /// </summary>
        /// <returns></returns>
        //private static CookieAuthenticationProvider GetMyCookieAuthenticationProvider()
        //{
        //    var cookieAuthenticationProvider = new CookieAuthenticationProvider();
        //    cookieAuthenticationProvider.OnValidateIdentity = async context =>
        //    {
        //        var cookieValidatorFunc = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
        //            TimeSpan.FromMinutes(10),
        //            (manager, user) =>
        //            {
        //                var identity = user.GenerateUserIdentityAsync(manager);
        //                return identity;
        //            });
        //        await cookieValidatorFunc.Invoke(context);

        //        if (context.Identity == null || !context.Identity.IsAuthenticated)
        //        {
        //            return;
        //        }

        //        // get list of roles on the user
        //        var userRoles = context.Identity
        //                               .Claims
        //                               .Where(c => c.Type == ClaimTypes.Role)
        //                               .Select(c => c.Value)
        //                               .ToList();

        //        foreach (var roleName in userRoles)
        //        {
        //            var cacheKey = ApplicationUser.ApplicationRole.GetCacheKey(roleName);
        //            var cachedClaims = System.Web.HttpContext.Current.Cache[cacheKey] as IEnumerable<Claim>;
        //            if (cachedClaims == null)
        //            {                        
        //                var roleManager = DependencyResolver.Current.GetService<ApplicationRoleManager>();
        //                cachedClaims = await ApplicationUserManager.GetClaimsAsync(roleName);
        //                System.Web.HttpContext.Current.Cache[cacheKey] = cachedClaims;
        //            }
        //            context.Identity.AddClaims(cachedClaims);
        //        }
        //    };
        //    return cookieAuthenticationProvider;
        //}
    }
}