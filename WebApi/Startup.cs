using System.Text.Json.Serialization;
using Domain.Interfaces;
using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistence;
using Persistence.Repositories;

namespace WebApi;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Add database context and connection string
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

        // Add controllers and related services
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            });
        
        // Dependency Injection Setup
        services.AddScoped<IGenericRepository<Buyer>, GenericRepository<Buyer>>();
        services.AddScoped<IGenericRepository<Seller>, GenericRepository<Seller>>();
        services.AddScoped<IGenericRepository<Category>, GenericRepository<Category>>();
        services.AddScoped<IGenericRepository<Machinery>, GenericRepository<Machinery>>();
        services.AddScoped<IGenericRepository<Image>, GenericRepository<Image>>();

        // Add any other services your application needs here
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c => { c.EnableAnnotations(); });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();

        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
        
        
        //app.Run();
    }
}