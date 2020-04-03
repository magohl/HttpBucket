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

namespace HttpBucket
{
    public class Startup
    {
        private static int _messageCounter=0;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
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
                endpoints.Map("/{returncode?}/bucket/{*url}", async context =>
                {
                    var retCode = ParseReturnCode(context);
                    await OutputRequestInfo(context, retCode);
                    context.Response.StatusCode = retCode;
                });

                endpoints.Map("/bucket/{*url}", async context =>
                {
                    var retCode=200;
                    await OutputRequestInfo(context, retCode);
                    context.Response.StatusCode = retCode;
                });

                endpoints.MapRazorPages();
                endpoints.MapHub<BucketHub>("/bucketHub");
            });
        }

        private static int ParseReturnCode(HttpContext context)
        {
            var routeValues = context.GetRouteData().Values;
            var retCodeValue = routeValues["returncode"] as string;

            if (!int.TryParse(retCodeValue, out var retCode)) return 200;
            return retCode;

        }
        private static async Task OutputRequestInfo(HttpContext context, int retCode)
        {
            _messageCounter++;

            var hubContext = context.RequestServices
                .GetRequiredService<IHubContext<BucketHub>>();

            string body;
            using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8))
            {  
                body = await reader.ReadToEndAsync();
            }

            var message = $"[Id {_messageCounter}] {context.Request.Method} {context.Request.Path} | Returning status {retCode} | Body: {(body==String.Empty ? "[NO BODY RECEVIED]" : body)}";
            await hubContext.Clients.All.SendAsync("ReceiveBucketMessage", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), message);
        }

    }
}
