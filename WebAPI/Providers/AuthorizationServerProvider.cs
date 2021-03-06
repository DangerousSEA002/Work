using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Work.Model.Models;

namespace WebAPI.Providers
{
    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public AuthorizationServerProvider()
        {
        }

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            await Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            UserManager<ApplicationUser> userManager = context.OwinContext.GetUserManager<UserManager<ApplicationUser>>();
            ApplicationUser user;
            try
            {
                user = await userManager.FindAsync(context.UserName, context.Password);
            }
            catch
            {
                // Could not retrieve the user due to error.
                context.SetError("server_error", "Lỗi trong quá trình xử lý.");
                context.Rejected();
                return;
            }
            if (user != null)
            {
                //var permissions = ServiceFactory.Get<IPermissionService>().GetByUserId(user.Id);
                //var permissionViewModels = AutoMapper.Mapper.Map<ICollection<Permission>, ICollection<PermissionViewModel>>(permissions);
                var roles = userManager.GetRoles(user.Id);
                ClaimsIdentity identity = await userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ExternalBearer);
                string avatar = string.IsNullOrEmpty(user.avatar) ? "" : user.avatar;
                string email = string.IsNullOrEmpty(user.Email) ? "" : user.Email;
                identity.AddClaim(new Claim("id", user.Id));
                identity.AddClaim(new Claim("name", user.name));
                identity.AddClaim(new Claim("avatar", avatar));
                identity.AddClaim(new Claim("email", email));
                identity.AddClaim(new Claim("username", user.UserName));
                identity.AddClaim(new Claim("roles", JsonConvert.SerializeObject(roles)));
                identity.AddClaim(new Claim("phone_number", user.PhoneNumber));
                //identity.AddClaim(new Claim("permissions", JsonConvert.SerializeObject(permissionViewModels)));
                var props = new AuthenticationProperties(new Dictionary<string, string>
                    {
                        {"id", user.Id },
                        {"name", user.name},
                        {"avatar", avatar },
                        {"email", email},
                        {"username", user.UserName},
                 //       {"permissions",JsonConvert.SerializeObject(permissionViewModels) },
                        {"roles",JsonConvert.SerializeObject(roles) },
                        {"phone_number",user.PhoneNumber }
                    });
                context.Validated(new AuthenticationTicket(identity, props));
            }
            else
            {
                context.SetError("invalid_grant", "Tài khoản hoặc mật khẩu không đúng.");
                context.Rejected();
            }
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }
            return Task.FromResult<object>(null);
        }
    }
}