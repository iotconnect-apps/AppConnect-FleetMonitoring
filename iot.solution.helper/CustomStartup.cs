using component.helper.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace component.helper
{
    public static class CustomStartup
    {
        public static void ConfigureService(IServiceCollection services)
        {
            //services.AddSingleton<ICacheAccessTokenHelper, CacheAccessTokenHelper>();
            //services.AddSingleton<IAccessTokenHelper, AccessTokenHelper>();
            services.AddSingleton<IHttpClientHelper, HttpClientHelper>();
            //services.AddSingleton<IEmailHelper, EmailHelper>();
            //services.AddSingleton<ICacheManager, CacheManager>();
            //services.AddSingleton<IDiscoveryHelper, DiscoveryHelper>();
        }
    }
}
