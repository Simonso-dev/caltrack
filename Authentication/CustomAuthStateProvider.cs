using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace caltrack.Authentication;

public class CustomAuthStateProvider : AuthenticationStateProvider {
    private readonly ProtectedSessionStorage _sessionStorage;
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _http;
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    public CustomAuthStateProvider(ProtectedSessionStorage sessionStorage, ILocalStorageService localStorage, HttpClient http) {
        _sessionStorage = sessionStorage;
        _localStorage = localStorage;
        _http = http;
    } 

    public override async Task<AuthenticationState> GetAuthenticationStateAsync() {
        try {
            var userSessionStorageResult = await _sessionStorage.GetAsync<UserSession>("UserSession");
            var userSession = userSessionStorageResult.Success ? userSessionStorageResult.Value : null;
            string token = await _localStorage.GetItemAsStringAsync("token");

            if(userSession is null) {
                _http.DefaultRequestHeaders.Authorization = null;
                return await Task.FromResult(new AuthenticationState(_anonymous));
            }

            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("\"", ""));

            var ClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> {
                new Claim(ClaimTypes.Name, userSession.Username),
                new Claim(ClaimTypes.Role, userSession.Role),
            }, "CustomAuth"));

            return await Task.FromResult(new AuthenticationState(ClaimsPrincipal));
        }
        catch(Exception ex) {
            ex.GetBaseException();
            return await Task.FromResult(new AuthenticationState(_anonymous));
        }
    }

    public async Task UpdateAuthenticationState(UserSession userSession) {
        ClaimsPrincipal claimsPrincipal;
        string token = await _localStorage.GetItemAsStringAsync("token");
        
        if(userSession != null) {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer" + token.Replace("\"", ""));
            await _sessionStorage.SetAsync("UserSession", userSession);
            claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> {
                new Claim(ClaimTypes.Name, userSession.Username),
                new Claim(ClaimTypes.Role, userSession.Role),
            }));
        }
        else {
            await _sessionStorage.DeleteAsync("UserSession");
            claimsPrincipal = _anonymous;
            _http.DefaultRequestHeaders.Authorization = null;
        }

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
    }
}