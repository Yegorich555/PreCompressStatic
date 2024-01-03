# PreCompressStatic

ASP.NET Core. Compress **once** by demand if required and send preCompressed static files to the browser

[![NuGet version](https://badge.fury.io/nu/PreCompressStatic.svg)](https://www.nuget.org/packages/PreCompressStatic)
[![Nuget](https://img.shields.io/nuget/dt/PreCompressStatic)](https://www.nuget.org/packages/PreCompressStatic)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## How to use

Add `app.UsePreCompressStaticFiles()` instead of `app.UseStaticFiles()` in Startup.Configure section

```c#
namespace Example
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor(); // it's important to add
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            app.UseRouting();

            app.UsePreCompressStaticFiles(); // add this instead of app.UseStaticFiles();

            app.UseEndpoints(e => { e.MapFallbackToFile("index.html"); });
        }
    }
}
```

## Rules

- UI part can be compressed during the building (for example with [webpack](https://webpack.js.org/)). Compressed files must end with `.br` & `.gzip` (for example `main.js.br`)
- If browser supports Brotli or Gzip compression ([Accept-Encoding header](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Accept-Encoding)) then the helper looks for `{file}.br` or `{file}.gzip`
- If files missed the helper compresses requested file, saves it in the same folder and returns compressed result
- Compression happens only once per file and only if `fileSize < options.MinSizeKb` (8kb by default)
- Middleware is highly optimized for performance and compresses only once if it's really makes sense

## Example

An example can be found in the [Example](https://github.com/Yegorich555/PreCompressStatic/tree/master/Example) directory.

## Acknowledgements

This solution is based on @neyromant from the following issue <https://github.com/aspnet/Home/issues/1584#issuecomment-227455026>.
