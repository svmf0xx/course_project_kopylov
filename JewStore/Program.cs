using JewStore.Data;
using JewStore.Interfaces;
using JewStore.Realizations;
using Microsoft.EntityFrameworkCore;




var builder = WebApplication.CreateBuilder(args);
;
builder.Configuration.AddJsonFile("appsettings.json");
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IResourceService, ResourceService>();
builder.Services.AddDbContextFactory<AppDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);








var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=LoginPage}/{id?}");

app.Run();
