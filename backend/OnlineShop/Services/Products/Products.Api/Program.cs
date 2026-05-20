using Microsoft.EntityFrameworkCore;
using Products.Application.Commands;
using Products.Application.Mappers;
using Products.Core.Repositories;
using Products.Infrastructure.Data.Contexts;
using Products.Infrastructure.Repositories;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi


builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Products API",
        Version = "v1",
        Description = "The Products API",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Kerolos",
            Email = "kerolos@gmail.com"
        }
    });
});

builder.Services.AddOpenApi();

builder.Services.AddAutoMapper(typeof(ProductProfile).Assembly);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies
                                                                    (
                                                                    Assembly.GetExecutingAssembly(),
                                                                    Assembly.GetAssembly(typeof(CreateProductCommand))
                                                                    )
                           );



builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
   builder.Configuration.GetConnectionString("OrderConnectionString"),
    sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null
        )
    ));


builder.Services.AddSingleton<IFileStorageService>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    var webRoot = string.IsNullOrEmpty(env.WebRootPath) ? Path.Combine(env.ContentRootPath, "wwwroot") : env.WebRootPath;
    return new FileStorageService(webRoot);
});

builder.Services.AddScoped<IProductRepository, ProductRepository>();


builder.Services.AddHttpContextAccessor();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors("AllowAngular");
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
