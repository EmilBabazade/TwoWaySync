using Data;
using Data.MappingProfiles;
using Data.Repos;
using Domain.User;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Services.DataSync;
using Services.MappingProfiles;
using Services.UsersApi;
using TwoWaySync.Validators;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHttpClient<UserApiHttpClient>();
        builder.Services.AddDbContext<DataContext>(options =>
            options.UseSqlite("Data Source = Database.db"));
        builder.Services.AddScoped<IUsersRepo, UsersRepo>();
        builder.Services.AddScoped<IDataSyncService, DataSyncService>();
        builder.Services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<UserEntityMappingProfile>();
            cfg.AddProfile<RequestUserMappingProfile>();
        });
        builder.Services.AddScoped<IValidator<User>, UserValidator>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}