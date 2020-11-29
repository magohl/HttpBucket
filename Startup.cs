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
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

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
                //Reqest for a specific returncode, body payload and headers
                endpoints.Map("/{returncode:int?}/{headers}/{payload}/{bucket:guid}/{*url}", async context =>
                {
                    var bodyBytes = ParseReturnBody(context);
                    var headers = ParseResponseHeaders(context);
                    var retCode = ParseReturnCode(context);
                    await ProcessRequest(context, retCode, headers, bodyBytes);
                });

                //Reqest for a specific returncode and body payload
                endpoints.Map("/{returncode:int?}/{payload}/{bucket:guid}/{*url}", async context =>
                {
                    var bodyBytes = ParseReturnBody(context);
                    var retCode = ParseReturnCode(context);
                    await ProcessRequest(context, retCode, null, bodyBytes);
                });

                //Reqest for a specific returncode
                endpoints.Map("/{returncode:int?}/{bucket:guid}/{*url}", async context =>
                {
                    var retCode = ParseReturnCode(context);
                    await ProcessRequest(context, retCode, null, null);
                });

                //Reqest
                endpoints.Map("{bucket:guid}/{*url}", async context =>
                {
                    var retCode= StatusCodes.Status200OK;
                    await ProcessRequest(context, retCode, null, null);
                });

                endpoints.MapRazorPages();
                endpoints.MapHub<BucketHub>("/bucketHub");
            });
        }

        /* --- PRIVATE METHODS --- */
        private async Task ProcessRequest(HttpContext context, int retCode, IEnumerable<KeyValuePair<string, StringValues>> headers, byte[] payload)
        {
            var bucketId = ParseBucketId(context);
            if (!BucketExist(context, bucketId))
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }
            await OutputRequestInfo(context, retCode, bucketId);
            context.Response.StatusCode = retCode;
            if (headers != null)
            {
                foreach (var h in headers)
                {
                    context.Response.Headers.Append(h.Key, h.Value);
                }
            }
            if (payload != null)
            {
                await context.Response.BodyWriter.WriteAsync(payload);
            }
        }
        
        private async Task OutputRequestInfo(HttpContext context, int retCode, Guid bucketId)
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
                Path = context.Request.Path + context.Request.QueryString,
                Headers = ParseRequestHeaders(context.Request.Headers),
                Body = String.IsNullOrEmpty(body) ? "[EMPTY BODY]" : body
            };

            store.AddEntry(bucketId, entry);
            await hubContext.Clients.Group(bucketId.ToString()).SendAsync("ReceiveBucketMessage", entry);
        }

        private bool BucketExist(HttpContext context, Guid bucketId)
        {
            var store = context.RequestServices
                .GetRequiredService<IBucketStore>();

            return store.Buckets.Any(b=>b.Id==bucketId);
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

        private int ParseReturnCode(HttpContext context)
        {
            var routeValues = context.GetRouteData().Values;
            var retCodeValue = routeValues["returncode"] as string;

            if (!int.TryParse(retCodeValue, out var retCode)) return 200;
            return retCode;
        }

        private byte[] ParseReturnBody(HttpContext context)
        {
            var routeValues = context.GetRouteData().Values;
            var bodyBase64 = routeValues["payload"] as string;
            var bodyBytes = Convert.FromBase64String(bodyBase64);
            return bodyBytes;
        }
        private IEnumerable<KeyValuePair<string, StringValues>> ParseResponseHeaders(HttpContext context)
        {
            var routeValues = context.GetRouteData().Values;
            var headersBase64 = routeValues["headers"] as string;
            var headersBytes = Convert.FromBase64String(headersBase64);
            var headersString = UTF8Encoding.UTF8.GetString(headersBytes);
            var headerPairs = headersString
                .Split('|')
                .Select(x => x.Split('='))
                .ToDictionary(x => x[0], x => new StringValues(x[1]));
                return headerPairs;
        }

        private string ParseRequestHeaders(IEnumerable<KeyValuePair<string, StringValues>> keyValuePairs)
        {
            var headersStringBuilder = new StringBuilder();
            foreach (var value in keyValuePairs)
            {
                headersStringBuilder.AppendLine(value.Key + " = " + string.Join(", ", value.Value));
            }
            return headersStringBuilder.ToString();
        }
    }
}
