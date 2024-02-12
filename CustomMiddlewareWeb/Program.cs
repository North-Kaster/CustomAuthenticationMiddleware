using CustomMiddleware;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Authorized.");

app.UseMiddleware<Authentication>();

app.Run();
