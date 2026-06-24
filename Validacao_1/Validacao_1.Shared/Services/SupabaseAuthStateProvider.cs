using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Supabase;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Validacao_1.Web.Services
{
    public class SupabaseAuthStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly Supabase.Client _supabaseClient;
        private const string TokenKey = "supabase_session";

        private AuthenticationState _currentAuthenticationState;

        public SupabaseAuthStateProvider(IJSRuntime jsRuntime, Client supabaseClient)
        {
            _jsRuntime = jsRuntime;
            _supabaseClient = supabaseClient;
            _currentAuthenticationState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                if (_currentAuthenticationState.User.Identity?.IsAuthenticated == true)
                {
                    return _currentAuthenticationState;
                }

                var savedSession = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TokenKey);

                if (string.IsNullOrWhiteSpace(savedSession))
                {
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                using var doc = JsonDocument.Parse(savedSession);
                var root = doc.RootElement;

                string email = "";
                string id = "";
                string perfil = ""; // 👈 Mantendo o padrão: criamos a string vazia

                if (root.TryGetProperty("user", out var userProp))
                {
                    email = userProp.TryGetProperty("email", out var emailProp) ? emailProp.GetString() ?? "" : "";

                    if (userProp.TryGetProperty("id", out var idProp))
                    {
                        id = idProp.ValueKind == JsonValueKind.Number
                            ? idProp.GetInt64().ToString()
                            : idProp.GetString() ?? "";
                    }

                    // 🟢 BUSCA O PERFIL DO USUÁRIO NO LOCALSTORAGE
                    if (userProp.TryGetProperty("perfil", out var perfilProp))
                    {
                        perfil = perfilProp.GetString() ?? "";
                    }
                }

                if (string.IsNullOrEmpty(email))
                {
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, email),
                    new Claim(ClaimTypes.NameIdentifier, id),
                    new Claim(ClaimTypes.Role, perfil) // 🟢 ADICIONA O PERFIL NAS CLAIMS
                }, "SupabaseAuth");

                _currentAuthenticationState = new AuthenticationState(new ClaimsPrincipal(identity));
                return _currentAuthenticationState;
            }
            catch
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public async Task MarcarComoLogado(string sessionJson)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, sessionJson);

            using var doc = JsonDocument.Parse(sessionJson);
            var root = doc.RootElement;

            string email = root.GetProperty("user").GetProperty("email").GetString() ?? "";

            var idProp = root.GetProperty("user").GetProperty("id");
            string id = idProp.ValueKind == JsonValueKind.Number
                ? idProp.GetInt64().ToString()
                : idProp.GetString() ?? "";

            // 🟢 BUSCA O PERFIL AQUI NO LOGIN TAMBÉM
            string perfil = "";
            if (root.GetProperty("user").TryGetProperty("perfil", out var perfilProp))
            {
                perfil = perfilProp.GetString() ?? "";
            }

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.NameIdentifier, id),
                new Claim(ClaimTypes.Role, perfil) // 🟢 ADICIONA O PERFIL NAS CLAIMS
            }, "SupabaseAuth");

            _currentAuthenticationState = new AuthenticationState(new ClaimsPrincipal(identity));

            NotifyAuthenticationStateChanged(Task.FromResult(_currentAuthenticationState));
        }

        public async Task MarcarComoDeslogado()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);

            _currentAuthenticationState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            NotifyAuthenticationStateChanged(Task.FromResult(_currentAuthenticationState));
        }

        public async Task FazerLogout()
        {
            // 1. Desloga do Supabase
            await _supabaseClient.Auth.SignOut();

            // 2. Limpa o estado local do Blazor (o método que você já tem)
            await MarcarComoDeslogado();
        }
    }
}