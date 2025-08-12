using E_Menu.API.Extensions;
using E_Menu.Caching;
using E_Menu.Chatbot;
using E_Menu.Engine;
using E_Menu.Internal;
using E_Menu.JobScheduler;
using E_Menu.Logging;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;


// Add services to the container.

builder.Services.AddApiServices(builder.Configuration);
builder.Services.AddInternalServices();
builder.Services.AddLoggingServices();
builder.Services.AddEngineServices();
builder.Services.AddCachingServices();
builder.Services.AddChatbotService(builder.Configuration);
builder.Services.AddJobScheduler(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddLogging();
builder.Services.AddRateLimiter(opt =>
{
    opt.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", token);
    };

    opt.AddPolicy("Default", httpContext =>
    {
        var useXForwarded = builder.Configuration.GetValue<bool>("RateLimiting:UseForwardedHeader");

        var clientIp = useXForwarded
            ? httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                ?? httpContext.Connection.RemoteIpAddress?.ToString()
            : httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(clientIp, partition =>
        {
            return new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 60,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            };
        });
    });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "E-Menu API", Version = "v1" });
});
builder.Services.AddEndpointsApiExplorer();
var app = builder.Build();

// Configure the HTTP request pipeline.

bool allowSwagger = builder.Configuration.GetValue<bool>("AllowSwagger");
if (allowSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "E-Menu API v1"));
}

app.UseCors("AllowAllOrigins");

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=86400"); // 1 gün
    }
});

app.UseRateLimiter();

app.UseSecurityHeaders();

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
public partial class Program { }