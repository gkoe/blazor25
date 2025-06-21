using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace WebAssembly.Services
{
    //public class PersistentAuthenticationStateProvider : AuthenticationStateProvider
    //{
    //    private readonly HttpClient _httpClient;
    //    private readonly ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    //    public PersistentAuthenticationStateProvider(HttpClient httpClient)
    //    {
    //        _httpClient = httpClient;
    //    }

    //    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    //    {
    //        try
    //        {
    //            var claims = await _httpClient.GetFromJsonAsync<List<ClaimDto>>("api/auth/status");

    //            if (claims == null || !claims.Any())
    //                return new AuthenticationState(_anonymous);

    //            var identity = new ClaimsIdentity(claims.Select(c => new Claim(c.Type, c.Value)), "serverAuth");
    //            var user = new ClaimsPrincipal(identity);

    //            return new AuthenticationState(user);
    //        }
    //        catch
    //        {
    //            // Fallback, wenn der Server nicht erreichbar ist
    //            return new AuthenticationState(_anonymous);
    //        }
    //    }

    //    public async Task RefreshAuthenticationState()
    //    {
    //        var authState = await GetAuthenticationStateAsync();
    //        NotifyAuthenticationStateChanged(Task.FromResult(authState));
    //    }
    //}

    //public class ClaimDto
    //{
    //    public string Type { get; set; } = String.Empty;
    //    public string Value { get; set; } = String.Empty;
    //}

}
