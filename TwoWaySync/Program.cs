using Data;
using Data.MappingProfiles;
using Data.Repos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Services.DataSync;
using Services.MappingProfiles;
using Services.UsersApi;

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
// TODO: interface
builder.Services.AddScoped<IDataSyncService, DataSyncService>();
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<UserEntityMappingProfile>();
    cfg.AddProfile<RequestUserMappingProfile>();
});

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
