using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PreCompressStatic;

namespace Example
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            app.UseRouting();
            app.UsePreCompressStaticFiles();
            //app.UseStaticFiles();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapFallbackToFile("index.html", new StaticFileOptions
                {
                    OnPrepareResponse = x =>
                    {
                        var httpContext = x.Context; // endPoint for SPA
                        var path = httpContext.Request.RouteValues["path"];
                    }
                });
            });
        }
    }
}
