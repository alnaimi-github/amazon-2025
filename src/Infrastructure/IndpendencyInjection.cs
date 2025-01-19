namespace infrastructure;

public static class IndpendencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
     IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<StoreContext>(options =>
            options.UseSqlServer(connectionString));


        return services;
    }

}
