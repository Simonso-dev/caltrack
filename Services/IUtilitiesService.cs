using System.Security.Claims;

public interface IUtilitiesService {
    ClaimsPrincipal ParseClaimsFromJwt(string jwt);
 }