var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//*******Added by Miguel to convert this old app to .Net 6
//this is for Firebase authentication
builder.Services.AddMvc().AddSessionStateTempDataProvider();
//with line below the browser session can be ended
// services.AddSession(s => s.IdleTimeout = TimeSpan.FromMinutes(1));
builder.Services.AddSession();
//*******************

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
/*Miguel commented out line below
app.UseAuthorization();
*/

//Miguel added below. This is for Firebase authentication
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();