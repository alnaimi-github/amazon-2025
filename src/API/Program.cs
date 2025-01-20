
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddInfrastructure(builder.Configuration);


var app = builder.Build();

app.MapControllers();

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

app.Run();
