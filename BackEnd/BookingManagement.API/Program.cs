using Asp.Versioning;
using BookingManagement.API.Middleware;
using BookingManagement.API.ModelBinding;
using BookingManagement.BL.Implementation;
using BookingManagement.BL.Interfaces;
using BookingManagement.DAL.Implementation;
using BookingManagement.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.ModelBinderProviders.Insert(0, new UtcDateTimeModelBinderProvider());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "BookingManagement.API", Version = "v1" });
});

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddMvc()
.AddApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookingManagement.API v1"));

app.UseHttpsRedirection();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapControllers();

app.Run();
