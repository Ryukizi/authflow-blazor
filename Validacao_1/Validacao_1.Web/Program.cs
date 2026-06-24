using Blazor.Extensions.Storage;
using Microsoft.AspNetCore.Components.Authorization;
using Supabase;

using Validacao_1.Shared.Services;
using Validacao_1.Web.Components;
using Validacao_1.Web.Services;           

var builder = WebApplication.CreateBuilder(args);

// 1. Registra o serviço de armazenamento (LocalStorage)
builder.Services.AddStorage();

// 2. Ativa o motor de autenticação nativo do Blazor
builder.Services.AddAuthenticationCore();

// 3. Registra uma ÚNICA instância do seu Provider e resolve os dois tipos
builder.Services.AddScoped<SupabaseAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<SupabaseAuthStateProvider>());

// 4. Configuração SEGURA do Cliente Supabase vinda do appsettings
string urlSupabase = builder.Configuration["Supabase:Url"] ?? string.Empty;
string chaveSupabase = builder.Configuration["Supabase:Chave"] ?? string.Empty;

var opcoes = new SupabaseOptions { AutoConnectRealtime = true };
builder.Services.AddSingleton(provider => new Client(urlSupabase, chaveSupabase, opcoes));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 5.  Alterado para AddScoped e o .NET gerencia a injeção do IConfiguration sozinho!
builder.Services.AddScoped<IValidador, Validador>();

builder.Services.AddScoped<Validacao_1.Shared.Services.CriptografiaServico>();

builder.Services.AddScoped<Validacao_1.Shared.Services.EmailServico>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(Validacao_1.Shared.Pages.CadastroForm).Assembly);

app.Run();