using Append.Blazor.Printing;

using Blazored.Toast;

using MESystem.Data;
using MESystem.Data.IFS;
using MESystem.Data.LineControl;
using MESystem.Data.Location;
using MESystem.Data.SetupInstruction;
using MESystem.Data.TRACE;
using MESystem.LabelComponents;
using MESystem.Pages.Warehouse;
using MESystem.Service;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using MouseEventArgs = global::Microsoft.AspNetCore.Components.Web.MouseEventArgs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
//   .AddNegotiate();

//builder.Services.AddAuthorization(options =>
//{
//    // By default, all incoming requests will be authorized according to the default policy.
//    options.FallbackPolicy = options.DefaultPolicy;
//});
builder.Services.AddBlazoredToast();
builder.Services.AddRazorPages();
builder.Services.AddLocalization();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEntityFrameworkOracle();
builder.Services.AddDevExpressBlazor(configure =>
    configure.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5);
builder.Services.Configure<DevExpress.Blazor.Configuration.GlobalOptions>(options =>
{
    options.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5;
});
builder.Services.AddScoped<SessionValues>();
builder.Services.AddScoped<LineEventsService>();

builder.Services.AddDbContext<LisDbContext>(options =>
{
    options.UseOracle(builder.Configuration.GetConnectionString("LineControlConnection"));
});
builder.Services.AddScoped<TraceService>();
builder.Services.AddDbContextPool<TraceDbContext>(options =>
{
    options.UseOracle(builder.Configuration.GetConnectionString("TraceConnectionVN"));
});
builder.Services.AddScoped<IfsService>();
builder.Services.AddDbContextPool<IfsDbContext>(options =>
{
    options.UseOracle(builder.Configuration.GetConnectionString("IFSConnection"));
});
builder.Services.AddScoped<SmtService>();
builder.Services.AddDbContext<SiDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SiplaceProConnectionVN"));
});
builder.Services.AddHttpClient("IP", (options) =>
{
    options.BaseAddress = new Uri("https://jsonip.com");
});
builder.Services.AddScoped<IApiClientService, ApiClientService>();
builder.Services.AddHttpClient("Location", options =>
{
    options.BaseAddress = new Uri("http://api.ipstack.com");
});

builder.Services.AddScoped<IPrintingService, PrintingService>();
builder.Services.AddScoped<PalleteLabel>();
builder.Services.AddScoped<ShipOutPallet>();
builder.Services.AddScoped<SwitchToggle>();

//builder.Services.AddBlazmBluetooth();

//builder.Services.AddWebSockets(configure: options =>
//{
//    builder.Host.UseContentRoot(Directory.GetCurrentDirectory());
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

//app.UseRequestLocalization(new RequestLocalizationOptions()
//    .AddSupportedCultures(new[] { "de-DE", "vi-VN" })
//    .AddSupportedUICultures(new[] { "de-DE", "vi-VN" }));

//app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

//app.UseAuthentication();
//app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
