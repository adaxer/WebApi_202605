using Microsoft.IdentityModel.Tokens;
using SmartLibrary.Auth.User;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartLibrary.Auth.Helpers;

public class JwtHelper
{
    public static string GenerateJwtToken(ApplicationUser user, IList<string> roles, ConfigureUserDbOptions options)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.EncryptionSecret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user!.UserName!),
            new Claim(ClaimTypes.Email, user!.Email!)
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
            options.TokenIssuer,
            options.TokenAudience,
            claims,
            expires: DateTime.UtcNow + options.TokenLifetime,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
