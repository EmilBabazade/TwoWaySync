using Data;
using Data.MappingProfiles;
using Data.Repos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
// TODO: interface
builder.Services.AddScoped<UsersRepo>();
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<UserMappingProfile>();
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
