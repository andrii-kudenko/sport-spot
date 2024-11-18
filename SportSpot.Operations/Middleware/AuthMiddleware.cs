namespace SportSpot.Operations.Middleware
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthMiddleware> _logger;

        public AuthMiddleware(RequestDelegate next, ILogger<AuthMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var userId = context.Session.GetInt32("UserId");
            var path = context.Request.Path.Value?.ToLower();

            // Skip authentication for login and register pages
            if (path.Contains("/auth/login") ||
                path.Contains("/auth/register") ||
                path.Contains("/css/") ||
                path.Contains("/js/") ||
                path.Contains("/lib/"))
            {
                await _next(context);
                return;
            }

            if (!userId.HasValue)
            {
                context.Response.Redirect("/Auth/Login");
                return;
            }

            await _next(context);
        }
    }

    public static class AuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthMiddleware>();
        }
    }
}