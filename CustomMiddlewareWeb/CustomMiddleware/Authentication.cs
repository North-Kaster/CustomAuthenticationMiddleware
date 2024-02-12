namespace CustomMiddleware;

public class Authentication
{
    private readonly RequestDelegate _next;

    public Authentication(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var username = context.Request.Query["username"];
        var password = context.Request.Query["password"];

        if (username != "user1" || password != "password1")
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Not authorized.");
            return;
        }
        else
        {
            await _next(context);
        }
    }
}
