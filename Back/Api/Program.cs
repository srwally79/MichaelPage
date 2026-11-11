using Api.Middleware;
using Application.Interfaces;
using Application.Services;
using Domain.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Api.Swagger;
using Api.Security;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required.");
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length != 25)
    throw new InvalidOperationException("Jwt:Key must contain exactly 25 characters.");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IStatusRepository, StatusRepository>();
builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IStatusService, StatusService>();
builder.Services.AddScoped<ILogService, LogService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "MichaelPage.Api",
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "MichaelPage.Client",
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(TokenKeyDerivation.Derive(jwtKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies[AuthCookie.Name];
                if (!string.IsNullOrEmpty(token))
                    context.Token = token;
                return Task.CompletedTask;
            },
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "Unauthorized.",
                    details = new[] { "A valid bearer token is required." }
                }, context.HttpContext.RequestAborted);
            },
            OnForbidden = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "Forbidden.",
                    details = new[] { "You do not have permission to access this resource." }
                }, context.HttpContext.RequestAborted);
            }
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "MichaelPage API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT bearer token returned by POST /api/auth/login."
    });
    options.OperationFilter<AuthorizeOperationFilter>();
});
builder.Services.AddCors(options => options.AddPolicy("Frontend", policy =>
    policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod().AllowCredentials()));
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.SuppressMapClientErrors = true;
    options.InvalidModelStateResponseFactory = context =>
    {
        var isInvalidAdditionalInfo = context.ModelState.Values
            .SelectMany(value => value.Errors)
            .Any(error => error.ErrorMessage.Contains("additionalInfo", StringComparison.OrdinalIgnoreCase));
        var detail = isInvalidAdditionalInfo
            ? "AdditionalInfo must be a JSON object."
            : "The request body or one of its fields is invalid.";
        return new BadRequestObjectResult(new
        {
            message = "Request validation failed.",
            details = new[] { detail }
        });
    };
});

var app = builder.Build();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseStatusCodePages(async statusContext =>
{
    var response = statusContext.HttpContext.Response;
    var (message, detail) = response.StatusCode switch
    {
        StatusCodes.Status400BadRequest => ("Bad request.", "Request data is invalid."),
        StatusCodes.Status401Unauthorized => ("Unauthorized.", "A valid bearer token is required."),
        StatusCodes.Status403Forbidden => ("Forbidden.", "You do not have permission to access this resource."),
        StatusCodes.Status404NotFound => ("Not found.", "The requested resource was not found."),
        StatusCodes.Status405MethodNotAllowed => ("Method not allowed.", "The HTTP method is not supported for this route."),
        StatusCodes.Status415UnsupportedMediaType => ("Unsupported media type.", "Send a supported request content type."),
        >= StatusCodes.Status500InternalServerError => ("An unexpected error occurred.", "The request could not be completed."),
        _ => ("Request failed.", "The request could not be completed.")
    };
    response.ContentType = "application/json";
    await response.WriteAsJsonAsync(new { message, details = new[] { detail } }, statusContext.HttpContext.RequestAborted);
});
app.UseCors("Frontend");
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapFallback(async context =>
{
    context.Response.StatusCode = StatusCodes.Status404NotFound;
    await context.Response.WriteAsJsonAsync(new
    {
        message = "Endpoint not found.",
        details = Array.Empty<string>()
    }, context.RequestAborted);
});

app.Run();


