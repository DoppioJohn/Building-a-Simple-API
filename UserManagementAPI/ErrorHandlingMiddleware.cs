using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace UserManagementAPI
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { error = "Internal server error." });
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
    }
}
