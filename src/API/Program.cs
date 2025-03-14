using API.SignalR;

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

builder.Services.AddSignalR();

var app = builder.Build();


app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowCredentials()
.WithOrigins("https://localhost:4200"));

app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();
app.MapGroup("api").MapIdentityApi<AppUser>();
app.MapHub<NotificationHub>("/hub/notifications");
app.MapFallbackToController("Index", "Fallback");

await ApplyMigrationsAndSeedDatabaseAsync(app);

await app.RunAsync();

static async Task ApplyMigrationsAndSeedDatabaseAsync(IHost app)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        await context.Database.MigrateAsync();
        await StoreContextSeeder.SeedAsync(context, logger);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}