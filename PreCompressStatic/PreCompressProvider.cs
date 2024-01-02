using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

#if NETSTANDARD2_0
using IHost = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
#else
using IHost = Microsoft.AspNetCore.Hosting.IWebHostEnvironment;
#endif


namespace PreCompressStatic
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>")]
    public class PreCompressProvider : IFileProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFileProvider _fileProvider;

        public PreCompressProvider(IHost hostingEnv, IHttpContextAccessor httpContextAccessor)
        {
            _fileProvider = hostingEnv.WebRootFileProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
            => _fileProvider.GetDirectoryContents(subpath);

        public IFileInfo GetFileInfo(string subpath)
        {
            // todo compress only if file size bigger expected
            if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Accept-Encoding", out var encodings))
            {
                if (encodings.Any(encoding => encoding.Contains("br")))
                {
                    var file = _fileProvider.GetFileInfo(subpath.EndsWith(".br") ? subpath : subpath + ".br");
                    if (file.Exists) return file;
                    else
                    {
                        var originalFile = _fileProvider.GetFileInfo(subpath);
                        if (originalFile.Exists)
                        {
                            using (var stream = originalFile.CreateReadStream())
                            {
                                using (var compressedStream = File.Create(originalFile.PhysicalPath + ".br"))
                                {
                                    using (var brStream = new BrotliStream(compressedStream, CompressionMode.Compress))
                                    {
                                        stream.CopyTo(brStream);
                                        brStream.Close(); // ensure that i/o operation with file is done => without it file info length can be 0
                                        var brFile = _fileProvider.GetFileInfo(subpath + ".br");
                                        if (brFile.Exists) return brFile;
                                    }
                                }
                            }
                        }
                    }
                }
                if (encodings.Any(encoding => encoding.Contains("gzip")))
                {
                    var file = _fileProvider.GetFileInfo(subpath.EndsWith(".gz") ? subpath : subpath + ".gz"); ;
                    if (file.Exists) return file;
                    else
                    {
                        var originalFile = _fileProvider.GetFileInfo(subpath);
                        if (originalFile.Exists)
                        {
                            using (var stream = originalFile.CreateReadStream())
                            {
                                using (var compressedStream = File.Create(originalFile.PhysicalPath + ".gz"))
                                {
                                    using (var gzStream = new GZipStream(compressedStream, CompressionMode.Compress))
                                    {
                                        stream.CopyTo(gzStream);
                                        gzStream.Close(); // ensure that i/o operation with file is done => without it file info length can be 0
                                        var gzFile = _fileProvider.GetFileInfo(subpath + ".gz");
                                        if (gzFile.Exists) return gzFile;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return _fileProvider.GetFileInfo(subpath);
        }

        public IChangeToken Watch(string filter)
            => _fileProvider.Watch(filter);
    }

    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Enables static files serving with pre compression (if pre compressed files is missing it creates once)
        /// </summary>
        public static IApplicationBuilder UsePreCompressStaticFiles(this IApplicationBuilder app)
        {
            var s = app.ApplicationServices;
            return app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true,
                FileProvider = new PreCompressProvider(s.GetRequiredService<IHost>(), s.GetRequiredService<IHttpContextAccessor>()),
                OnPrepareResponse = ctx =>
                {
                    var headers = ctx.Context.Response.Headers;

                    if (ctx.File.Name.EndsWith(".br"))
                        headers.Add("Content-Encoding", "br");
                    else if (ctx.File.Name.EndsWith(".gz"))
                        headers.Add("Content-Encoding", "gzip");
                }
            });
        }
    }
}
