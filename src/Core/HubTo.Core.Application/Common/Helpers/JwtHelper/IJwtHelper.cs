namespace HubTo.Core.Application.Common.Helpers.JwtHelper;

internal interface IJwtHelper
{
    (string, DateTime) GenerateAccessToken(UserJwtDto user);
}