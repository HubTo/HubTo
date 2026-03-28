using HubTo.Core.Application.Common.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HubTo.Core.Application.Common.Helpers.JwtHelper;

internal sealed class JwtHelper : IJwtHelper
{
    private readonly JwtSettings _jwtSettings;

    public JwtHelper(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public (string, DateTime) GenerateAccessToken(UserJwtDto user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("username", user.Username),
        };

        if (user.Roles is not null && user.Roles.Any())
        {
            foreach (var (namespaceId, roleId) in user.Roles)
            {
                claims.Add(new Claim("user_roles", $"{namespaceId}:{roleId}"));
            }
        }

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        (string, DateTime) result = (new JwtSecurityTokenHandler().WriteToken(token), token.ValidTo);

        return result;
    }
}
