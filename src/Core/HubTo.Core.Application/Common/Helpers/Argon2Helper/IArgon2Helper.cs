namespace HubTo.Core.Application.Common.Helpers.Argon2Helper;

public interface IArgon2Helper
{
    string Hash(string key);
    bool Verify(string key, string encodedHash);
}
