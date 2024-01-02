using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System;

#if NETSTANDARD2_0
using IHost = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
#else
using IHost = Microsoft.AspNetCore.Hosting.IWebHostEnvironment;
#endif


namespace PreCompressStatic
{
    public struct Compressor
    {
        public string Extension { get; set; }
        public Func<FileStream, Stream> CompressStream { get; set; }
    }

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

        public static readonly Compressor CompressorBr = new() { Extension = ".br", CompressStream = (s) => new BrotliStream(s, CompressionMode.Compress) };
        public static readonly Compressor CompressorZip = new() { Extension = ".gz", CompressStream = (s) => new GZipStream(s, CompressionMode.Compress) };
        public static int MinSize = 8192; // compress only when size bigger

        public IFileInfo FindAndCompress(Compressor c, string subpath)
        {
            var file = _fileProvider.GetFileInfo(subpath.EndsWith(c.Extension) ? subpath : subpath + c.Extension);
            if (file.Exists) return file;
            else
            {
                var originalFile = _fileProvider.GetFileInfo(subpath);
                if (originalFile.Exists && originalFile.Length > MinSize)
                {
                    using (var stream = originalFile.CreateReadStream())
                    {
                        using (var fStream = File.Create(originalFile.PhysicalPath + c.Extension))
                        {
                            using (var cStream = c.CompressStream(fStream))
                            {
                                stream.CopyTo(cStream);
                                cStream.Close(); // ensure that i/o operation with file is done => without it file info length can be 0
                                var nFile = _fileProvider.GetFileInfo(subpath + c.Extension);
                                if (nFile.Exists) return nFile;
                            }
                        }
                    }
                }
            }
            return _fileProvider.GetFileInfo(subpath);
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Accept-Encoding", out var encodings))
            {
                if (encodings.Any(e => e.Contains("br")))
                    return FindAndCompress(CompressorBr, subpath);
                else if (encodings.Any(e => e.Contains("gzip")))
                    return FindAndCompress(CompressorZip, subpath);
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
            // TODO: allow user extend staticFileOptions & MinSize
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
