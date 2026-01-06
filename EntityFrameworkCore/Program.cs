using EntityFrameworkCore.Classes;
using EntityFrameworkCore.Models;
using EntityFrameworkCore.Models.Repository;
using EntityFrameworkCore.Models.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
using Microsoft.AspNetCore.Identity;
using EntityFrameworkCore.Areas.Identity.Data;
using EntityFrameworkCore.Areas.Admin.Controllers;
using EntityFrameworkCore.Areas.Identity.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<BookShopContext>(options =>
    options.UseLazyLoadingProxies()
           .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
});

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
builder.Services.AddScoped<IApplicationUserManager, ApplicationUserManager>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<ISmsSender, SmsSender>();
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

    //Configure SignIn
    options.SignIn.RequireConfirmedEmail = true;

    //Contigure LockoutAccount
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(20);
    options.Lockout.MaxFailedAccessAttempts = 3;
});

builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = "978532525496-duvt6h99ktdv937a9kltfefdkimshvho.apps.googleusercontent.com";
        options.ClientSecret = "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";
    }).AddYahoo(options =>
    {
        options.ClientId = "dj0yJmk9Vm9NSGd6UHRBdjdIJmQ9WVdrOWVWcHlOak5wWW0wbWNHbzlNQT09JnM9Y29uc3VtZXJzZWNyZXQmc3Y9MCZ4PTEx";
        options.ClientSecret = "25bd931d56c805b78a912d77efda8e8e860a042c";
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
app.UseSession();
app.UseRouting();
app.UseAuthorization();
app.UseAuthentication();



app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.Run();
