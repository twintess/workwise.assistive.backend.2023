using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.DataProtection;
using Scrutor;
using System.Net;
using System.Reflection;
using workwise.assistive.backend;
using workwise.assistive.backend.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Logging.AddLog4Net();

builder.Services.Scan(scan => scan
    .FromCallingAssembly()
    .AddClasses()
    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
    .AsSelf()
    .WithScopedLifetime());

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
        .WithHeaders("content-type")
        .AllowCredentials();
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.RequireAuthenticatedSignIn = true;
}).AddCookie(options =>
{
    options.Cookie = new CookieBuilder
    {
        Name = "token",
        MaxAge = TimeSpan.FromMinutes(Config.TOKEN_TIMEOUT),
        HttpOnly = true,
        SecurePolicy = CookieSecurePolicy.Always,
        SameSite = SameSiteMode.None //TODO usunąć
    };

    options.TicketDataFormat = new JwtSecureDataFormat();
    options.LoginPath = "/api/auth/login";
    options.LogoutPath = "/api/auth/logout";
    options.Events.OnRedirectToLogin = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }
        else
        {
            context.Response.Redirect(context.RedirectUri);
        }

        return Task.CompletedTask;
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("popupOperatorPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("popupOperator");
    });

    options.AddPolicy("popupReaderPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("popupReader");
    });
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(Config.SESSION_TIMEOUT);
    options.Cookie.SameSite = SameSiteMode.None; //TODO usunąć
    options.Cookie.IsEssential = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.MaxAge = TimeSpan.FromMinutes(Config.SESSION_TIMEOUT);
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.Name = "session_id";
});

builder.Services.AddCookiePolicy(options =>
{
    options.HttpOnly = HttpOnlyPolicy.Always;
    options.Secure = CookieSecurePolicy.Always;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.UseCookiePolicy();

app.UseSession();

app.MapControllers();

app.Use((context, next) =>
{
    //context.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains;";
    context.Response.Headers["Referrer-Policy"] = "no-referrer";
    context.Response.Headers["X-Frame-Options"] = "deny";
    return next.Invoke();
});

app.Run();
