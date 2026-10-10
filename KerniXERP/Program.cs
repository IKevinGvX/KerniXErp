using System.Security.Claims;
using System.Text;

using KernixERP.Database.Connections;
using KernixERP.Interfaces;
using KernixERP.Interfaces.Compras;
using KernixERP.Interfaces.IA;
using KernixERP.Interfaces.Inventario;
using KernixERP.Interfaces.Organizacion;
using KernixERP.Interfaces.Seguridad;
using KernixERP.Interfaces.Ventas;
using KernixERP.Middlewares;
using KernixERP.Repositories;
using KernixERP.Repositories.Compras;
using KernixERP.Repositories.IA;
using KernixERP.Repositories.Inventario;
using KernixERP.Repositories.Organizacion;
using KernixERP.Repositories.Seguridad;
using KernixERP.Repositories.Ventas;
using KernixERP.Security.CurrentUser;
using KernixERP.Security.JWT;
using KernixERP.Services;
using KernixERP.Services.Compras;
using KernixERP.Services.IA;
using KernixERP.Services.Inventario;
using KernixERP.Services.Organizacion;
using KernixERP.Services.Seguridad;
using KernixERP.Services.Ventas;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ==========================================================
// SERILOG
// ==========================================================

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services);
});
builder.Services.AddScoped<
    IResultadoAnalisisRepository,
    ResultadoAnalisisRepository>();

builder.Services.AddScoped<
    IResultadoAnalisisService,
    ResultadoAnalisisService>();
// ==========================================================
// INFRAESTRUCTURA
// ==========================================================

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<
    IRotacionProductoRepository,
    RotacionProductoRepository>();
builder.Services.AddScoped<
    IClasificacionRiesgoRepository,
    ClasificacionRiesgoRepository>();

builder.Services.AddScoped<
    IClasificacionRiesgoService,
    ClasificacionRiesgoService>();
builder.Services.AddScoped<
    IRotacionProductoService,
    RotacionProductoService>();
builder.Services.AddSingleton<
    ISqlConnectionFactory,
    SqlConnectionFactory>();

builder.Services.AddScoped<
    ICurrentUserService,
    CurrentUserService>();
builder.Services.AddScoped<
    IMovimientoAnomaloRepository,
    MovimientoAnomaloRepository>();

builder.Services.AddScoped<
    IMovimientoAnomaloService,
    MovimientoAnomaloService>();
// ==========================================================
// CONTROLADORES
// ==========================================================

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

// ==========================================================
// CONEXION SQL - DESARROLLO: USER SECRETS / AZURE: VARIABLES
// ==========================================================

if (string.IsNullOrWhiteSpace(
    builder.Configuration.GetConnectionString("KernixERP")))
{
    throw new InvalidOperationException(
        "Falta ConnectionStrings:KernixERP. Configura User Secrets o " +
        "ConnectionStrings__KernixERP en Azure App Service.");
}

// ==========================================================
// SEGURIDAD - AUTENTICACIÓN
// ==========================================================

builder.Services.AddScoped<
    IAuthRepository,
    AuthRepository>();

builder.Services.AddScoped<
    IAuthService,
    AuthService>();

builder.Services.AddScoped<
    IJwtService,
    JwtService>();

builder.Services.AddScoped<
    ITokenService,
    TokenService>();

// ==========================================================
// SEGURIDAD - USUARIOS
// ==========================================================

builder.Services.AddScoped<
    IUsuarioRepository,
    UsuarioRepository>();

builder.Services.AddScoped<
    IUsuarioService,
    UsuarioService>();

// ==========================================================
// SEGURIDAD - ROLES
// ==========================================================

builder.Services.AddScoped<
    IRolRepository,
    RolRepository>();

builder.Services.AddScoped<
    IRolService,
    RolService>();

// ==========================================================
// ORGANIZACIÓN - EMPRESAS
// ==========================================================

builder.Services.AddScoped<
    IEmpresaRepository,
    EmpresaRepository>();

builder.Services.AddScoped<
    IEmpresaService,
    EmpresaService>();

// ==========================================================
// ORGANIZACIÓN - SUCURSALES
// ==========================================================

builder.Services.AddScoped<
    ISucursalRepository,
    SucursalRepository>();

builder.Services.AddScoped<
    ISucursalService,
    SucursalService>();

// ==========================================================
// ORGANIZACIÓN - EMPLEADOS
// ==========================================================

builder.Services.AddScoped<
    IEmpleadoRepository,
    EmpleadoRepository>();

builder.Services.AddScoped<
    IEmpleadoService,
    EmpleadoService>();

// ==========================================================
// INVENTARIO - ALMACENES
// ==========================================================

builder.Services.AddScoped<
    IAlmacenRepository,
    AlmacenRepository>();

builder.Services.AddScoped<
    IAlmacenService,
    AlmacenService>();

// ==========================================================
// INVENTARIO - CATEGORÍAS
// ==========================================================

builder.Services.AddScoped<
    ICategoriaProductoRepository,
    CategoriaProductoRepository>();

builder.Services.AddScoped<
    ICategoriaProductoService,
    CategoriaProductoService>();

// ==========================================================
// INVENTARIO - MARCAS
// ==========================================================

builder.Services.AddScoped<
    IMarcaRepository,
    MarcaRepository>();

builder.Services.AddScoped<
    IMarcaService,
    MarcaService>();

// ==========================================================
// INVENTARIO - UNIDADES DE MEDIDA
// ==========================================================

builder.Services.AddScoped<
    IUnidadMedidaRepository,
    UnidadMedidaRepository>();

builder.Services.AddScoped<
    IUnidadMedidaService,
    UnidadMedidaService>();

// ==========================================================
// INVENTARIO - PRODUCTOS
// ==========================================================

builder.Services.AddScoped<
    IProductoRepository,
    ProductoRepository>();

builder.Services.AddScoped<
    IProductoService,
    ProductoService>();

// ==========================================================
// INVENTARIO - STOCK
// ==========================================================

builder.Services.AddScoped<
    IStockRepository,
    StockRepository>();

builder.Services.AddScoped<
    IStockService,
    StockService>();

// ==========================================================
// INVENTARIO - MOVIMIENTOS / KARDEX
// ==========================================================

builder.Services.AddScoped<
    IMovimientoInventarioRepository,
    MovimientoInventarioRepository>();

builder.Services.AddScoped<
    IMovimientoInventarioService,
    MovimientoInventarioService>();

// ==========================================================
// VENTAS - CLIENTES
// ==========================================================

builder.Services.AddScoped<
    IClienteRepository,
    ClienteRepository>();

builder.Services.AddScoped<
    IClienteService,
    ClienteService>();

// ==========================================================
// COMPRAS - PROVEEDORES
// ==========================================================

builder.Services.AddScoped<
    IProveedorRepository,
    ProveedorRepository>();

builder.Services.AddScoped<
    IProveedorService,
    ProveedorService>();

// ==========================================================
// COMPRAS - SOLICITUDES
// ==========================================================

builder.Services.AddScoped<
    ISolicitudCompraRepository,
    SolicitudCompraRepository>();

builder.Services.AddScoped<
    ISolicitudCompraService,
    SolicitudCompraService>();

// ==========================================================
// COMPRAS - ÓRDENES
// ==========================================================

builder.Services.AddScoped<
    IOrdenCompraRepository,
    OrdenCompraRepository>();

builder.Services.AddScoped<
    IOrdenCompraService,
    OrdenCompraService>();

// ==========================================================
// COMPRAS - RECEPCIONES
// ==========================================================

builder.Services.AddScoped<
    IRecepcionCompraRepository,
    RecepcionCompraRepository>();

builder.Services.AddScoped<
    IRecepcionCompraService,
    RecepcionCompraService>();

// ==========================================================
// COMPRAS - DOCUMENTOS DE PROVEEDOR
// ==========================================================

builder.Services.AddScoped<
    IDocumentoProveedorRepository,
    DocumentoProveedorRepository>();

builder.Services.AddScoped<
    IDocumentoProveedorService,
    DocumentoProveedorService>();

// ==========================================================
// COMPRAS - DEVOLUCIONES
// ==========================================================

builder.Services.AddScoped<
    IDevolucionCompraRepository,
    DevolucionCompraRepository>();

builder.Services.AddScoped<
    IDevolucionCompraService,
    DevolucionCompraService>();

// ==========================================================
// COMPRAS - NOTAS DE CRÉDITO
// ==========================================================

builder.Services.AddScoped<
    INotaCreditoProveedorRepository,
    NotaCreditoProveedorRepository>();

builder.Services.AddScoped<
    INotaCreditoProveedorService,
    NotaCreditoProveedorService>();

// ==========================================================
// COMPRAS - CUENTAS POR PAGAR
// ==========================================================

builder.Services.AddScoped<
    ICuentaPorPagarRepository,
    CuentaPorPagarRepository>();

builder.Services.AddScoped<
    ICuentaPorPagarService,
    CuentaPorPagarService>();

// ==========================================================
// KERNIX AI - REPOSICIÓN
// ==========================================================

builder.Services.AddScoped<
    IReposicionIARepository,
    ReposicionIARepository>();

builder.Services.AddScoped<
    IReposicionIAService,
    ReposicionIAService>();

// ==========================================================
// KERNIX AI - ALERTAS DE INVENTARIO
// ==========================================================

builder.Services.AddScoped<
    IAlertaInventarioRepository,
    AlertaInventarioRepository>();

builder.Services.AddScoped<
    IAlertaInventarioService,
    AlertaInventarioService>();

// ==========================================================
// KERNIX AI - PRONÓSTICO DE VENTAS
// ==========================================================

builder.Services.AddScoped<
    IPronosticoVentasRepository,
    PronosticoVentasRepository>();

builder.Services.AddScoped<
    IPronosticoVentasService,
    PronosticoVentasService>();

// ==========================================================
// SWAGGER
// ==========================================================

builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type =>
        type.FullName?.Replace("+", "."));

    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Ingrese únicamente el access token JWT."
        });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [
                new OpenApiSecuritySchemeReference(
                    "Bearer",
                    document)
            ] = []
        });
});

// ==========================================================
// JWT
// ==========================================================

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection(
        JwtSettings.SectionName));

var jwtSettings = builder.Configuration
    .GetSection(JwtSettings.SectionName)
    .Get<JwtSettings>()
    ?? throw new InvalidOperationException(
        "No se encontró la configuración JWT.");

if (string.IsNullOrWhiteSpace(jwtSettings.Issuer))
{
    throw new InvalidOperationException(
        "El emisor JWT es obligatorio.");
}

if (string.IsNullOrWhiteSpace(jwtSettings.Audience))
{
    throw new InvalidOperationException(
        "La audiencia JWT es obligatoria.");
}

if (string.IsNullOrWhiteSpace(jwtSettings.Key) ||
    Encoding.UTF8.GetByteCount(jwtSettings.Key) < 32)
{
    throw new InvalidOperationException(
        "La clave JWT debe contener como mínimo 32 bytes.");
}

// ==========================================================
// AUTENTICACIÓN JWT
// ==========================================================

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,

                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            jwtSettings.Key)),

                ValidateLifetime = true,
                RequireExpirationTime = true,

                ClockSkew = TimeSpan.Zero,

                NameClaimType = ClaimTypes.Name,
                RoleClaimType = ClaimTypes.Role
            };
    });

builder.Services.AddAuthorization();

// ==========================================================
// CORS
// ==========================================================

var originsProduccion = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "KernixERPPolicy",
        policy =>
        {
            if (builder.Environment.IsDevelopment())
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            }
            else
            {
                if (originsProduccion.Length > 0)
                {
                    policy.WithOrigins(originsProduccion);
                }

                policy.AllowAnyHeader().AllowAnyMethod();
            }
        });
});

// ==========================================================
// BUILD
// ==========================================================

var app = builder.Build();

// ==========================================================
// PIPELINE HTTP
// ==========================================================

app.UseGlobalExceptionMiddleware();

app.UseSerilogRequestLogging();

// Swagger esta activo automaticamente en Development.
// En Azure Production solo se habilita si Swagger__Enabled=true.
// Deshabilitar Swagger__Enabled una vez terminadas las pruebas.
var swaggerHabilitado =
    app.Environment.IsDevelopment() ||
    app.Configuration.GetValue<bool>("Swagger:Enabled");

if (swaggerHabilitado)
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "KerniX ERP API v1");

        options.RoutePrefix = "swagger";

        options.DocumentTitle =
            "KerniX ERP - Documentación API";

        options.DisplayRequestDuration();
        options.DefaultModelsExpandDepth(-1);
    });
}

app.UseHttpsRedirection();

app.UseCors("KernixERPPolicy");

app.UseAuthentication();

app.UseAuthorization();

// Salud del proceso HTTP. No consulta SQL ni expone credenciales.
app.MapGet("/health", () => Results.Ok(new
{
    servicio = "KerniX ERP API",
    estado = "Disponible",
    entorno = app.Environment.EnvironmentName
})).AllowAnonymous();

app.MapControllers();

app.Run();
