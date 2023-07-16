using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

public class UtilitiesService : IUtilitiesService {

    public ClaimsPrincipal ParseClaimsFromJwt(string jwt) {
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.ReadJwtToken(jwt);

        if(token is null)
            return null;
        
        var claims = new ClaimsIdentity(token.Claims,
            CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(claims);

        return principal;
    }
}