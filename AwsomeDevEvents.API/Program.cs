using AwsomeDevEvents.API.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddSingleton<DevEventsDbContext>();
var connectionString = builder.Configuration.GetConnectionString("DevEventsCs");

//builder.Services.AddDbContext<DevEventsDbContext>(o => o.UseInMemoryDatabase("DevEventsDb"));
builder.Services.AddDbContext<DevEventsDbContext>(o => o.UseSqlServer(connectionString));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AwsomeDevEvents.API",
        Version = "v1",
        Contact = new OpenApiContact
        {
            Name = "Let Galvao",
            Email = "le@gmail.com",
            Url = new Uri("https://legalvao.com.br")
        }
    });
    // nome arquivo xml
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    // construir o arquivo no caminho diretorio base + nome arquivo
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
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
