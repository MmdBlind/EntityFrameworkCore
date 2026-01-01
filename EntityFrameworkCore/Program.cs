using EntityFrameworkCore.Classes;
using EntityFrameworkCore.Models;
using EntityFrameworkCore.Models.Repository;
using EntityFrameworkCore.Models.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
using Microsoft.AspNetCore.Identity;
using EntityFrameworkCore.Areas.Identity.Data;
using EntityFrameworkCore.Areas.Admin.Controllers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<BookShopContext>(options =>
    options.UseLazyLoadingProxies()
           .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddDefaultIdentity<BookShopUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<BookShopContext>();
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<BookShopContext>()
    .AddDefaultUI()
    .AddErrorDescriber<ApplicationIdentityErrorDescriber>()
    .AddDefaultTokenProviders();
builder.Services.AddTransient<BooksRepository>();
builder.Services.AddTransient<ConvertDate>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IConvertDate, ConvertDate>();
builder.Services.AddTransient<IBooksRepository, BooksRepository>();
builder.Services.AddScoped<IApplicationRoleManager, ApplicationRoleManager>();
builder.Services.AddScoped<ApplicationIdentityErrorDescriber>();
builder.Services.AddScoped<IApplicationUserManager,ApplicationUserManager>();
builder.Services.Configure<IdentityOptions>(options =>
{
    //Configure Password
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;

    //Configure User
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    options.User.RequireUniqueEmail = true;
});

builder.Services.AddPaging(options =>
{
    options.ViewName = "Bootstrap4";
    options.HtmlIndicatorDown = " <span>&darr;</span>";
    options.HtmlIndicatorUp = " <span>&uarr;</span>";
});
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
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.Run();
