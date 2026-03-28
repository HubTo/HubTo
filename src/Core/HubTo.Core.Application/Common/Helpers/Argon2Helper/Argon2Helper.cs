using HubTo.Core.Application.Common.Settings;
using Konscious.Security.Cryptography;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace HubTo.Core.Application.Common.Helpers.Argon2Helper;

internal sealed class Argon2Helper : IArgon2Helper
{
    private readonly Argon2Settings _settings;

    public Argon2Helper(IOptions<Argon2Settings> settings)
    {
        _settings = settings.Value;
    }

    public string Hash(string key)
    {
        var passwordBytes = Encoding.UTF8.GetBytes(key + _settings.Peppers[_settings.CurrentVersion]);
        var salt = RandomNumberGenerator.GetBytes(_settings.SaltSize);

        var argon2 = new Argon2id(passwordBytes)
        {
            Salt = salt,
            Iterations = _settings.Iterations,
            MemorySize = _settings.MemorySize,
            DegreeOfParallelism = _settings.Parallelism
        };

        var hash = argon2.GetBytes(_settings.HashSize);

        return $"$hubto${_settings.CurrentVersion}$argon2id$v=19$m={_settings.MemorySize},t={_settings.Iterations},p={_settings.Parallelism}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public bool Verify(string key, string encodedHash)
    {
        var parts = encodedHash.Split('$', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 7 || parts[0] != "hubto")
            return false;

        var version = parts[1];
        if (!_settings.Peppers.TryGetValue(version, out var pepper))
            return false;

        var passwordBytes = Encoding.UTF8.GetBytes(key + pepper);

        var parameters = parts[4];
        var salt = Convert.FromBase64String(parts[5]);
        var hash = Convert.FromBase64String(parts[6]);

        var paramParts = parameters.Split(',');

        int memory = int.Parse(paramParts[0].Split('=')[1]);
        int iterations = int.Parse(paramParts[1].Split('=')[1]);
        int parallelism = int.Parse(paramParts[2].Split('=')[1]);

        var argon2 = new Argon2id(passwordBytes)
        {
            Salt = salt,
            Iterations = iterations,
            MemorySize = memory,
            DegreeOfParallelism = parallelism
        };

        var computed = argon2.GetBytes(hash.Length);
        return CryptographicOperations.FixedTimeEquals(hash, computed);
    }
}

