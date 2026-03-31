using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using onvatenter.Models.Data;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite("Data Source=app.db"));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

    // 1. Créer le rôle Admin
    string[] roles = { "Admin", "Inspector" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // ADMIN
    var adminEmail = "admin@test.com";
    var admin = await userManager.FindByEmailAsync(adminEmail);

    if (admin == null)
    {
        var user = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail
        };

        await userManager.CreateAsync(user, "Admin123!");
        await userManager.AddToRoleAsync(user, "Admin");
    }

    // INSPECTOR
    var inspectorEmail = "inspector@test.com";
    var inspector = await userManager.FindByEmailAsync(inspectorEmail);

    if (inspector == null)
    {
        var user = new IdentityUser
        {
            UserName = inspectorEmail,
            Email = inspectorEmail
        };

        await userManager.CreateAsync(user, "Inspector123!");
        await userManager.AddToRoleAsync(user, "Inspector");
    }
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapRazorPages();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.Run();
