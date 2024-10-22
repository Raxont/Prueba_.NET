using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

/**
 * Configuración de logging.
 * Limpiamos los proveedores de logging por defecto y añadimos los de consola y debug.
 */
builder.Logging.ClearProviders(); // Opcional, para limpiar los proveedores de logging por defecto
builder.Logging.AddConsole(); // Agregar el proveedor de logging de consola
builder.Logging.AddDebug(); // Agregar el proveedor de logging de debug

/**
 * Agregar servicios al contenedor.
 * Configuramos el controlador para manejar opciones de serialización JSON.
 */
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        options.JsonSerializerOptions.WriteIndented = true;
    });

/**
 * Configuración de Swagger para la documentación de la API.
 * Permite explorar y probar los endpoints de la API.
 */
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/**
 * Configuración del cliente HTTP para el servicio de vuelos.
 * Permite realizar solicitudes HTTP a servicios externos.
 */
builder.Services.AddHttpClient<FlightService>();

/**
 * Configuración del uso de la memoria cache
 * Permite guardar informacion en el cache del cliente
 */
builder.Services.AddMemoryCache();

/**
 * Agregar DbContext para la conexión a la base de datos.
 * Utilizamos SQL Server con la cadena de conexión definida en la configuración.
 */
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

/**
 * Configurar los pipelines de solicitudes HTTP.
 * Si estamos en un entorno de desarrollo, habilitamos Swagger.
 */
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Habilitar Swagger
    app.UseSwaggerUI(); // Habilitar UI de Swagger
}

/**
 * Configurar la autorización.
 * Esto asegura que solo los usuarios autenticados puedan acceder a ciertos recursos.
 */
app.UseAuthorization();
app.MapControllers();

/**
 * Iniciar la aplicación.
 * Comienza a escuchar las solicitudes HTTP entrantes.
 */
app.Run();
