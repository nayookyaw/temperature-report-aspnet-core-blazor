using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace BackendAspNetCore.Middlewares
{
    public class PodInfoHeaderMiddleware
    {
        private readonly RequestDelegate _next;

        private const string PodHeader = "X-Served-By";
        private const string NodeHeader = "X-Node-Name";

        public PodInfoHeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Prefer Downward API env vars if present
            var podName = Environment.GetEnvironmentVariable("POD_NAME") 
                          ?? Environment.GetEnvironmentVariable("HOSTNAME") 
                          ?? "unknown-pod";

            var nodeName = Environment.GetEnvironmentVariable("NODE_NAME")
                           ?? "unknown-node";

            // Add headers before response is sent
            context.Response.OnStarting(() =>
            {
                // Use Append to add header values without throwing if the key already exists
                context.Response.Headers.Append(PodHeader, podName);
                context.Response.Headers.Append(NodeHeader, nodeName);

                return Task.CompletedTask;
            });

            await _next(context);
        }
    }

    public static class PodInfoHeaderMiddlewareExtensions
    {
        public static IApplicationBuilder UsePodInfoHeaders(this IApplicationBuilder app)
        {
            return app.UseMiddleware<PodInfoHeaderMiddleware>();
        }
    }
}