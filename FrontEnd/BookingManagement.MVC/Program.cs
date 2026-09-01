using BookingManagement.MVC.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddBookingManagementServices(builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Bookings}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
