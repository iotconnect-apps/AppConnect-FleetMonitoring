using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using Entity = iot.solution.entity;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using component.helper;
using System.Security.Claims;
using iot.solution.service.Interface;

namespace host.iot.solution.Middleware
{
    public static class HeaderkeyAuthorizationMiddlewareExtension
    {
        public static IApplicationBuilder UseHeaderkeyAuthorization(this IApplicationBuilder app)
        {
            return app.UseMiddleware<HeaderkeyAuthorizationMiddleware>();
        }
    }

    public class HeaderkeyAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IUserService _userService;
        public HeaderkeyAuthorizationMiddleware(RequestDelegate next, IUserService userService)
        {
            _next = next;
            _userService = userService;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {

                if (context.Request.Path.Value.Equals("/")
                     || context.Request.Path.Value.Contains("/api/account/FilePath")
                     || context.Request.Path.Value.Contains("/api/account/refreshtoken")
                     || context.Request.Path.Value.Contains("/api/subscriber")
                     || context.Request.Path.Value.Contains("/api/account/login")
                       || context.Request.Path.Value.Contains("/api/account/identity")
                     || context.Request.Path.Value.StartsWith("/wwwroot/")
                     || context.Request.Path.Value.Contains("/api/alert/addiotalert")                     
                     || context.Request.Path.Value.Contains("/api/account/adminlogin")
                     || context.Request.Path.Value.Contains("/api/chart/executecron")) // Nikunj

                {
                    await _next.Invoke(context);
                    return;
                }
                if (!context.Request.Headers.ContainsKey("company-id") || string.IsNullOrWhiteSpace(context.Request.Headers["company-id"]))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Header key is missing!");
                }
                SolutionConfiguration.CompanyId = Guid.Parse(context.Request.Headers["company-id"]);
                if (SolutionConfiguration.CompanyId == null || SolutionConfiguration.CompanyId == Guid.Empty)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Header key is missing!");
                }

                if (context.User != null && context.User.Claims != null)
                {
                    SolutionConfiguration.BearerToken = context.Request.Headers["Authorization"].ToString().Replace("Bearer", "").Trim();
                    Claim admin = context.User.Claims.Where(c => c.Type == "IOT_CONNECT").FirstOrDefault();
                    if (admin != null && admin.Value.Equals("AdminUser"))
                    {
                        SolutionConfiguration.CurrentUserId = Guid.Parse(context.User.Claims.Where(c => c.Type == "CURRENT_USERID").FirstOrDefault().Value);
                    }
                    else if (admin != null)
                    {
                        JwtSecurityTokenHandler hand = new JwtSecurityTokenHandler();
                        var BearerToken = admin.Value;
                        var tokenS = hand.ReadJwtToken(BearerToken);
                        var jsonValue = tokenS.Claims?.SingleOrDefault(p => p.Type == "user")?.Value;

                        Entity.UserDetail userDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<Entity.UserDetail>(jsonValue);
                        SolutionConfiguration.BearerToken = BearerToken;
                        SolutionConfiguration.CurrentUserId = Guid.Parse(userDetail.Id);
                        SolutionConfiguration.SolutionId = Guid.Parse(userDetail.SolutionGuid);
                        SolutionConfiguration.EntityGuid = Guid.Parse(userDetail.EntityGuid);
                        SolutionConfiguration.CompanyId = Guid.Parse(userDetail.CompanyId);
                    }
                    else
                    {
                        Entity.ActionStatus loginResponse = _userService.Identity(SolutionConfiguration.BearerToken);
                        if (loginResponse != null && loginResponse.Success)
                        {
                            IoTConnect.Model.IdentityResult responsedata = loginResponse.Data;
                            SolutionConfiguration.CurrentUserId = Guid.Parse(responsedata.data.userGuid);
                            SolutionConfiguration.SolutionId = Guid.Parse(responsedata.data.solutionGuid);
                            SolutionConfiguration.EntityGuid = Guid.Parse(responsedata.data.entityGuid);
                            SolutionConfiguration.CompanyId = Guid.Parse(responsedata.data.companyGuid);
                        }
                        else {
                            context.Response.StatusCode = 401;
                            await context.Response.WriteAsync(loginResponse.Message);                            
                        }
                    }
                    //SolutionConfiguration.BearerToken = context.User.Claims.Where(c => c.Type == "IOT_CONNECT").FirstOrDefault().Value;
                    //if (!string.IsNullOrWhiteSpace(SolutionConfiguration.BearerToken) && SolutionConfiguration.BearerToken.Equals("AdminUser"))
                    //{
                    //    SolutionConfiguration.CurrentUserId = Guid.Parse(context.User.Claims.Where(c => c.Type == "CURRENT_USERID").FirstOrDefault().Value);
                    //}
                    //else
                    //{
                    //    JwtSecurityTokenHandler hand = new JwtSecurityTokenHandler();
                    //    var tokenS = hand.ReadJwtToken(SolutionConfiguration.BearerToken);
                    //    var jsonValue = tokenS.Claims?.SingleOrDefault(p => p.Type == "user")?.Value;
                    //    Entity.UserDetail userDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<Entity.UserDetail>(jsonValue);
                    //    SolutionConfiguration.CurrentUserId = Guid.Parse(userDetail.Id);
                    //    SolutionConfiguration.SolutionId = Guid.Parse(userDetail.SolutionGuid);
                    //    SolutionConfiguration.EntityGuid = Guid.Parse(userDetail.EntityGuid);
                    //}
                }

                await _next.Invoke(context);
                return;

            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }
    }
}
