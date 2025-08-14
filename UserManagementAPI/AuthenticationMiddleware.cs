using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace UserManagementAPI
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private const string VALID_TOKEN = "12345"; // Example token

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue("Authorization", out var token) || token != $"Bearer {VALID_TOKEN}")
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new { error = "Unauthorized" });
                return;
            }

            await _next(context);
        }
    }
}
