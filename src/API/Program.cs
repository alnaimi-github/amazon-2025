
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var configuration = builder.Configuration;

builder.Services.AddInfrastructure(configuration);
builder.Services.AddCors();

builder.Services.AddSingleton<IConnectionMultiplexer>(con =>
{
    var connectionString = configuration.GetConnectionString("Redis")
                         ?? throw new InvalidOperationException("Cannot get Redis connection string!");
    var config = ConfigurationOptions.Parse(connectionString, true);
    return ConnectionMultiplexer.Connect(config);
});

builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<AppUser>()
  .AddEntityFrameworkStores<StoreContext>();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod()
.WithOrigins("https://localhost:4200"));

app.MapControllers();
app.MapIdentityApi<AppUser>();

try
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
    await context.Database.MigrateAsync();
    await StoreContextSeeder.SeedAsync(context);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
await app.RunAsync();