using EntityFrameworkDapperApp.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.ConfigurePersistenceServices();
builder.Services.ConfigureDbContext(builder.Configuration);

builder.Services.AddControllers();
builder.Services.ConfigureApiVersioning();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.UseSwagger();
    // app.UseSwaggerUI();
}

app.UseApiVersioning();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// For testing purposes
public partial class Program {}