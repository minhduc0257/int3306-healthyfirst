using System.Net;
using System.Reflection;
using int3306;
using dotenv.net;
using int3306.Controllers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using CookieAuthenticationEvents = int3306.CookieAuthenticationEvents;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);
const string baseApiPath = "api";

builder.Services.AddDbContext<DataDbContext>(options =>
{
    var connectionString = Environment.GetEnvironmentVariable("MARIADB_CONNECTION_STRING")!;
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});
builder.Services.AddCors(cors =>
{
    cors.AddDefaultPolicy(
        policyBuilder => policyBuilder
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed(_ => true)
    );
});
builder.Services.AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
        options =>
        {
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "int3306",
                ValidAudience = "int3306",
                IssuerSigningKey = Keys.SigningKey
            };
            
            builder.Configuration.Bind("JwtSettings", options);
        })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
        options =>
        {
            options.EventsType = typeof(CookieAuthenticationEvents);
            options.SlidingExpiration = true;
            options.ExpireTimeSpan = TimeSpan.FromDays(180);
            builder.Configuration.Bind("CookieSettings", options);
        });

builder.Services.AddAuthorization(o =>
{
    var defaultAuthorizationPolicyBuilder =
        new AuthorizationPolicyBuilder(
            CookieAuthenticationDefaults.AuthenticationScheme,
            JwtBearerDefaults.AuthenticationScheme
        )
            .RequireAuthenticatedUser();
    o.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
});

builder.Services.AddScoped<CookieAuthenticationEvents>();

builder.Services.AddControllers().AddNewtonsoftJson(o =>
{
    o.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
    o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter \"Bearer\" followed by the token",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Name = "Bearer",
                In = ParameterLocation.Header,
                Reference = new OpenApiReference { Id = "Bearer", Type = ReferenceType.SecurityScheme }
            },
            new List<string>()
        }
    });

    options.EnableAnnotations();
    options.CustomSchemaIds(type => type.ToString());
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
    options.DocumentFilter<SwaggerPrefixDocumentFilter>(baseApiPath);
}).AddSwaggerGenNewtonsoftSupport();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(o =>
{
    o.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    o.RoutePrefix = string.Empty;
});

app.Map($"/{baseApiPath}", appBuilder =>
{
    appBuilder.UseAuthentication();
    appBuilder.UseRouting();
    appBuilder.UseAuthorization();
    appBuilder.UseCors();
    appBuilder.Use(async (ctx, next) =>
    {
        var controllerActionDescriptor = ctx
            .GetEndpoint()?
            .Metadata
            .GetMetadata<ControllerActionDescriptor>();

        if (controllerActionDescriptor != null)
        {
            var requirePrivileged = controllerActionDescriptor.ControllerTypeInfo
                .GetCustomAttribute<RequirePrivilegedAttribute>();

            requirePrivileged ??=
                controllerActionDescriptor.MethodInfo.GetCustomAttribute<RequirePrivilegedAttribute>();

            if (requirePrivileged is not null)
            {
                var uid = ctx.User.GetUserId();
                var dbcontext = ctx.RequestServices.GetRequiredService<DataDbContext>();
                var user = await dbcontext.Users.FirstOrDefaultAsync(user => user.Id == uid);
                if (user?.Type != UserType.Admin)
                {
                    ctx.Response.StatusCode = (int) HttpStatusCode.Found;
                    ctx.Response.Redirect(UnauthorizedController.Route);
                    return;
                }
            }
        }
        
        await next.Invoke();
    });
    appBuilder.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
            name: "default",    
            pattern: "{controller}/{action=Index}/{id?}");
    });
});

app.Run();
