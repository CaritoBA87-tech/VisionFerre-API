using Amazon.Extensions.NETCore.Setup;
using Amazon.Rekognition;
using Amazon.S3;
using Amazon.Translate;
using AulaDiser.Proyecto.API.Configuration;
using AulaDiser.Proyecto.Datos;
using AulaDiser.Proyecto.Datos.Auth;
using AulaDiser.Proyecto.Datos.Compra;
using AulaDiser.Proyecto.API;
using AulaDiser.Proyecto.Logica.Auth;
using AulaDiser.Proyecto.Logica.Compra;
using AulaDiser.Proyecto.Logica.Seg;
using AulaDiser.Proyecto.Logica.Servicios;
using Microsoft.AspNetCore.Authentication.Cookies;
using Scalar.AspNetCore;
using Amazon.BedrockRuntime;
using AulaDiser.Proyecto.Logica;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Configurar los mapeos de Mapster
MapsterConfig.RegisterMappings();

//--Inyección de dependencias
/*string connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
                         ?? builder.Configuration.GetConnectionString("AD.Conexion");*/

/*string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                         ?? builder.Configuration.GetConnectionString("AD.Conexion");*/

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                         ?? builder.Configuration["ConnectionStrings:DefaultConnection"]
                         ?? builder.Configuration["DefaultConnection"]
                         ?? builder.Configuration.GetConnectionString("AD.Conexion");

//Esto le dice al API que cuando alguien pida ICompraRepository proporcione CompraRepository con la cadena de conexión en el constructor
builder.Services.AddScoped<ICompraRepository>(sp => new CompraRepository(connectionString));

//Esto le dice al API que cuando alguien pida ICompraService proporcione CompraService
builder.Services.AddScoped<ICompraService, CompraService>();

//Esto le dice al API que cuando alguien pida IUsuarioRepository proporcione UsuarioRepository con la cadena de conexión en el constructor
builder.Services.AddScoped<IUsuarioRepository>(sp => new UsuarioRepository(connectionString));
//Esto le dice al API que cuando alguien pida IAuthService proporcione AuthService
builder.Services.AddScoped<IAuthService, AuthService>();
//Esto le dice al API que cuando alguien pida ITokenService proporcione TokenService
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddScoped<DatosProducto>(sp => new DatosProducto(connectionString));
builder.Services.AddSwaggerGen();

//Servicio de Amazon Rekognition
//builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
/*var awsOptions = builder.Configuration.GetAWSOptions("AWS");
builder.Services.AddDefaultAWSOptions(awsOptions);*/

//Intentamos obtener las llaves desde las variables de entorno (Prioridad para Railway)
var accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID")
                ?? builder.Configuration["AWS:AccessKeyId"];

var secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY")
                ?? builder.Configuration["AWS:SecretAccessKey"];

var awsOptions = builder.Configuration.GetAWSOptions("AWS");

if (!string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey))
{
    awsOptions.Credentials = new Amazon.Runtime.BasicAWSCredentials(accessKey, secretKey);
    awsOptions.Region = Amazon.RegionEndpoint.USEast1;
}

builder.Services.AddDefaultAWSOptions(awsOptions);
builder.Services.AddAWSService<IAmazonRekognition>();
builder.Services.AddScoped<VisionService>();

//Servicios de Amazon S3
builder.Services.AddAWSService<IAmazonS3>();

// Registrar el cliente de AWS
builder.Services.AddAWSService<IAmazonBedrockRuntime>();

// Registrar tu servicio de lógica
builder.Services.AddScoped<IAsistenteIA, BedrockService>();

//Servicio de Amazon Translate
//builder.Services.AddAWSService<IAmazonTranslate>();

//Servicio de Amazon Comprehend
//builder.Services.AddScoped<ComprehendService>();

//Instalamos Microsoft.AspNetCore.Authentication.JwtBearer para poder usar Jwt
/*builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuerSigningKey = true, //Que valide la firma del emisor del token
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])), //Que la llave del emisor la tome de appsettings.json
            ValidateIssuer = false, //Que no valide el emisor del token
            ValidateAudience = false  //Que no valide a quien recibe el token
        };
    });*/

builder.Services.AddAuthorization();

/*builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowS3",
        builder => builder.WithOrigins("http://vision-ferre-web.s3-website-us-east-1.amazonaws.com") // Tu URL de la imagen 911e39
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});*/

builder.Services.AddControllers();

// Configuramos la autenticación para que use Cookies por defecto
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Cookie.Name = "AuthToken"; // Nombre de la cookie
    options.Cookie.HttpOnly = true;    // JavaScript de Angular no podrá leerla (Seguridad XSS)
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Solo viaja por HTTPS
    options.Cookie.SameSite = SameSiteMode.None; // Necesario si Angular y API tienen puertos distintos
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    options.SlidingExpiration = true;

    // IMPORTANTE: Evita que la API intente redireccionar a una página de Login HTML
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//---Permitir llamadas desde mi aplicación Angular
/*builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy => policy.WithOrigins("http://localhost:4200") // El origen de tu Angular
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
});*/

builder.Services.AddCors(options =>
{
    options.AddPolicy("VisionFerrePolicy", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",
                "http://vision-ferre-web.s3-website-us-east-1.amazonaws.com"
              )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Importante para tus cookies de autenticación
    });
});
//---


var app = builder.Build();


//---

//Permitir llamadas desde mi sitio web estático en S3
//app.UseCors("AllowS3");

app.UseCors("VisionFerrePolicy");

app.UseAuthentication(); //¿Quién eres? Valida el token
app.UseAuthorization(); //¿Qué puedes hacer? Valida los roles o claims

app.UseSwagger(options =>
{
    options.RouteTemplate = "openapi/{documentName}.json";
});

app.MapScalarApiReference(options =>
{
    options.WithTitle("VisionFerre API 2026")
           .WithTheme(ScalarTheme.Default);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    
}

//---Permitir llamadas desde mi aplicación Angular
//app.UseCors("AllowAngular");

app.MapControllers();

app.Run();
