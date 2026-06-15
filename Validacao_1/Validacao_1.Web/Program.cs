using Validacao_1.Shared.Services;
using Validacao_1.Web.Components;
using Validacao_1.Web.Services;
using Supabase;

var builder = WebApplication.CreateBuilder(args);

var opcoes = new SupabaseOptions { AutoConnectRealtime = true };
builder.Services.AddSingleton(provider => new Client(urlSupabase, chaveSupabase, opcoes));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add device-specific services used by the Validacao_1.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();
builder.Services.AddSingleton<IValidador, Validador>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(Validacao_1.Shared._Imports).Assembly);

app.Run();
