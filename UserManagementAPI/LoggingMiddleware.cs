using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace UserManagementAPI
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine($"Incoming Request: {context.Request.Method} {context.Request.Path}");
            await _next(context);
            Console.WriteLine($"Response Status: {context.Response.StatusCode}");
        }
    }
}
