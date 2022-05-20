using System.Reflection;
using int3306;
using dotenv.net;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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
builder.Services.AddControllers().AddNewtonsoftJson(o =>
{
    o.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
    o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
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
    appBuilder.UseRouting();
    appBuilder.UseAuthorization();
    appBuilder.UseCors();
    appBuilder.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
            name: "default",    
            pattern: "{controller}/{action=Index}/{id?}");
    });
});

app.Run();
