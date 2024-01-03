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
            app.UsePreCompressStaticFiles(); // app.UseStaticFiles();
            app.UseEndpoints(e => { e.MapFallbackToFile("index.html"); });
        }
    }
}
