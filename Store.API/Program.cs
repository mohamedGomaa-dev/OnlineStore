using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Store.DataAccess.Data;
using Store.DataAccess.Units.implementations;
using Store.DataAccess.Units.interfaces;
using Store.Services.Helpers;
using Store.Services.Services.implementations;
using Store.Services.Services.interfaces;
using System.Text;

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
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// add automapper
builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// 🌟 1. إعدادات الـ Authentication و JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        // قراءة المفتاح من الـ User Secrets أو البيئة
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT_SECRET_KEY"] ?? throw new InvalidOperationException("JWT Key is missing"))),
        ValidateIssuer = false, // (مغلق حالياً للتبسيط)
        ValidateAudience = false, // (مغلق حالياً للتبسيط)
        ValidateLifetime = true, // التحقق من أن التوكن لم تنتهِ صلاحيته
        ClockSkew = TimeSpan.Zero
    };
});

// 🌟 2. تعليم Swagger كيف يتعامل مع التوكن
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Store API", Version = "v1" });

    // إضافة زر القفل 🔒
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter token like this: Bearer your_token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // إجبار Swagger على إرسال التوكن مع كل طلب
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// add cors configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("StoreApiCorsPolicy", policy =>
    {
        policy
            .WithOrigins(
                "https://localhost:7023",
                "http://localhost:5146"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("StoreApiCorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
