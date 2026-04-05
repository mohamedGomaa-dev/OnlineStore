using Microsoft.EntityFrameworkCore;
using Store.DataAccess.Data;
using Store.DataAccess.Units.implementations;
using Store.DataAccess.Units.interfaces;
using Store.Services.Helpers;
using Store.Services.Services.implementations;
using Store.Services.Services.interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// add connection string
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddControllers();

// add unit of work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// add services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductImageService, ProductImageService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IShippingService, ShippingService>();

// add automapper
builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
