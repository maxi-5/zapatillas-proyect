using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Reflection;
using System.Text;
using ZapatillasProject.Middleware;
using ZapatillasProject.Models;
using ZapatillasProject.Repositories;
using ZapatillasProject.Services;

var builder = WebApplication.CreateBuilder(args);

// ================== SECRETOS (variables de entorno) ==================
// La cadena de conexion y la llave JWT NO estan en appsettings.json.
// Se leen de las variables de entorno: ConnectionStrings__MiConexion y Jwt__Key
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
{
    throw new InvalidOperationException(
        "Falta la variable de entorno Jwt__Key (minimo 32 caracteres). Ejecuta configurar-variables.bat");
}

// ================== BASE DE DATOS (Entity Framework Core) ==================
// Scoped (por defecto): un DbContext por cada peticion HTTP
if (!builder.Environment.IsEnvironment("Testing"))
{
    var conexion = builder.Configuration.GetConnectionString("MiConexion");
    if (string.IsNullOrWhiteSpace(conexion))
    {
        throw new InvalidOperationException(
            "Falta la variable de entorno ConnectionStrings__MiConexion. Ejecuta configurar-variables.bat");
    }

    builder.Services.AddDbContext<ZapatillasDbContext>(options =>
        options.UseSqlServer(conexion));
}

// ================== INYECCION DE DEPENDENCIAS ==================
// Repositorios -> Scoped: usan el DbContext, que vive lo mismo que la peticion
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IZapatillaRepository, ZapatillaRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IVentaRepository, VentaRepository>();
builder.Services.AddScoped<IResenaRepository, ResenaRepository>();
builder.Services.AddScoped<IStockTallaRepository, StockTallaRepository>();
// Servicio de tokens -> Singleton: no guarda estado y solo lee la configuracion
builder.Services.AddSingleton<ITokenService, TokenService>();

// ================== AUTENTICACION JWT ==================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

// ================== VERSIONAMIENTO (por URL: /api/v1/...) ==================
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true; // agrega el header api-supported-versions
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddMvc()
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";      // grupo "v1" para swagger
    options.SubstituteApiVersionInUrl = true; // muestra /api/v1/... en vez de /api/v{version}/...
});

builder.Services.AddControllers();

// configuramos los endpoints
builder.Services.AddEndpointsApiExplorer();
// generamos el documento con swagger, con el boton Authorize para probar las apis bloqueadas
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Tienda de Zapatillas",
        Version = "v1",
        Description = "API RESTful para gestionar el catalogo, stock por talla, ventas y reseñas de una tienda de zapatillas. " +
                      "Roles: Admin (gestiona catalogo, stock y usuarios) y Cliente (compra y deja reseñas)."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pega aqui SOLO el token obtenido en POST /api/v1/User/login"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });

    // usa los comentarios /// <summary> de los controladores en la documentacion
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// crea el usuario Admin inicial si no existe
await DbSeeder.CrearAdminAsync(app.Services, app.Configuration, app.Logger);

// ================== PIPELINE (orden de los middleware) ==================
app.UseMiddleware<ErrorHandlerMiddleware>(); // 1. captura errores de todo lo que viene despues
app.UseMiddleware<LoggingMiddleware>();      // 2. registra cada peticion

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API Zapatillas v1");
        options.DocumentTitle = "API Tienda de Zapatillas";
    });
}

app.UseHttpsRedirection();

// se habilitan los permisos
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// necesario para que el proyecto de pruebas de integracion pueda levantar la API
public partial class Program { }
