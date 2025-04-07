using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Logging;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddControllersWithViews();

// coisas pra n deixar o usuario mexer sem login
builder.Services.AddSession();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// adicionar dois certificados. uma key e outro do cert. utilizei mkcert para validar os meus.
var certificadoPath = Path.Combine(Directory.GetCurrentDirectory(), "Certs", "192.168.0.71+1.pem");
var chavePath = Path.Combine(Directory.GetCurrentDirectory(), "Certs", "192.168.0.71+1-key.pem");

builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 44382, listenOptions =>
    {
        listenOptions.UseHttps(certificadoPath, chavePath);
    });
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // exceptions etc
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
// sessao de login 
app.UseSession();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=entrarConta}/{id?}"); //definir uma página inicial

app.Run();