using Microsoft.EntityFrameworkCore;
using ECommerce.Data;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// JSON döngüsel referans sorunu için ayar
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// DbContext'i ekle
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// SESSION AYARLARINI EKLE
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// SWAGGER AYARLARINI EKLE
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "E-Commerce API",
        Description = "E-Commerce platformu için Web API dökümantasyonu",
        Contact = new OpenApiContact
        {
            Name = "Destek Ekibi",
            Email = "destek@ecommerce.com"
        }
    });

    // XML yorumlarını etkinleştir (opsiyonel ama önerilir)
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Bearer token authentication için (gelecekte kullanmak isterseniz)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header kullanarak Bearer token. Örnek: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    // Geliştirme ortamında Swagger'ı etkinleştir
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "E-Commerce API V1");
        options.RoutePrefix = "swagger"; // https://localhost:port/swagger adresinden erişim
        options.DocumentTitle = "E-Commerce API Dökümantasyonu";

        // UI Özelleştirmeleri
        options.DefaultModelsExpandDepth(-1); // Modelleri varsayılan olarak gizle
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None); // Endpoint'leri varsayılan olarak kapat
        options.DisplayRequestDuration(); // İstek süresini göster
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// SESSION MIDDLEWARE'İNİ EKLE
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();