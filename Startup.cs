using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HttpBucket.Hubs;
using Microsoft.AspNetCore.HttpOverrides;
using HttpBucket.Stores;
using HttpBucket.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Primitives;

namespace HttpBucket
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IBucketStore, BucketStore>();

            services.AddRazorPages(options =>
            {
                options.Conventions.AddPageRoute("/UI/New", "");
            });
            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler("/Error");
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.Map("/{returncode:int?}/{bucket:guid}/{*url}", async context =>
                {
                    var retCode = ParseReturnCode(context);
                    var bucketId = ParseBucketId(context);
                    await OutputRequestInfo(context, retCode, bucketId);
                    context.Response.StatusCode = retCode;
                });

                endpoints.Map("{bucket:guid}/{*url}", async context =>
                {
                    var retCode=200;
                    var bucketId = ParseBucketId(context);
                    await OutputRequestInfo(context, retCode, bucketId);
                    context.Response.StatusCode = retCode;
                });

                endpoints.MapRazorPages();
                endpoints.MapHub<BucketHub>("/bucketHub");
            });
        }

        private Guid ParseBucketId(HttpContext context)
        {
            var routeValues = context.GetRouteData().Values;
            var bucketIdValue = routeValues["bucket"] as string;

            if (!Guid.TryParse(bucketIdValue, out var bucketId))
            {
                throw new ArgumentException();
            }
            return bucketId;
        }

        private static int ParseReturnCode(HttpContext context)
        {
            var routeValues = context.GetRouteData().Values;
            var retCodeValue = routeValues["returncode"] as string;

            if (!int.TryParse(retCodeValue, out var retCode)) return 200;
            return retCode;
        }

        private static string ParseHeaders(IEnumerable<KeyValuePair<string, StringValues>> keyValuePairs)
        {
            var sb = new StringBuilder();
            foreach (var value in keyValuePairs)
            {
                sb.AppendLine(value.Key + " = " + string.Join(", ", value.Value));
            }
            return sb.ToString();
        }

        private static async Task OutputRequestInfo(HttpContext context, int retCode, Guid bucketId)
        {
            var hubContext = context.RequestServices
                .GetRequiredService<IHubContext<BucketHub>>();

            var store = context.RequestServices
                .GetRequiredService<IBucketStore>();

            string body;
            using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8))
            {  
                body = await reader.ReadToEndAsync();
            }

            var entry = new BucketEntry
            {
                Id = store.Counter(bucketId),
                Received = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                StatusCodeToReturn = retCode,
                Method = context.Request.Method,
                Path = context.Request.Path,
                Headers = ParseHeaders(context.Request.Headers),
                Body = String.IsNullOrEmpty(body) ? "[EMPTY BODY]" : body
            };

            store.AddEntry(bucketId, entry);
            await hubContext.Clients.Group(bucketId.ToString()).SendAsync("ReceiveBucketMessage", entry);
        }
    }
}
