using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;

namespace UserManagementAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            // In-memory user store
            var users = new List<User>();

            // Middleware pipeline
            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseMiddleware<AuthenticationMiddleware>();
            app.UseMiddleware<LoggingMiddleware>();

            // Endpoints
            app.MapGet("/users", () => Results.Ok(users));

            app.MapGet("/users/{id}", (int id) =>
            {
                var user = users.FirstOrDefault(u => u.Id == id);
                return user is not null ? Results.Ok(user) : Results.NotFound();
            });

            app.MapPost("/users", (User user) =>
            {
                if (string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.Email))
                    return Results.BadRequest("Name and Email are required.");

                if (!User.IsValidEmail(user.Email))
                    return Results.BadRequest("Invalid email format.");

                if (users.Any(u => u.Email.Equals(user.Email, System.StringComparison.OrdinalIgnoreCase)))
                    return Results.BadRequest("A user with this email already exists.");

                user.Id = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1;
                users.Add(user);
                return Results.Created($"/users/{user.Id}", user);
            });

            app.MapPut("/users/{id}", (int id, User updatedUser) =>
            {
                if (string.IsNullOrWhiteSpace(updatedUser.Name) || string.IsNullOrWhiteSpace(updatedUser.Email))
                    return Results.BadRequest("Name and Email are required.");

                if (!User.IsValidEmail(updatedUser.Email))
                    return Results.BadRequest("Invalid email format.");

                var user = users.FirstOrDefault(u => u.Id == id);
                if (user is null) return Results.NotFound();

                if (users.Any(u => u.Email.Equals(updatedUser.Email, System.StringComparison.OrdinalIgnoreCase) && u.Id != id))
                    return Results.BadRequest("A user with this email already exists.");

                user.Name = updatedUser.Name;
                user.Email = updatedUser.Email;
                return Results.Ok(user);
            });

            app.MapDelete("/users/{id}", (int id) =>
            {
                var user = users.FirstOrDefault(u => u.Id == id);
                if (user is null) return Results.NotFound();

                users.Remove(user);
                return Results.NoContent();
            });

            app.Run();
        }
    }
}
