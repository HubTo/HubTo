namespace HubTo.Core.Application.Common.Settings;

internal class Argon2Settings
{
    public int SaltSize { get; set; }
    public int HashSize { get; set; }
    public int Iterations { get; set; }
    public int MemorySize { get; set; }
    public int Parallelism { get; set; }
    public string CurrentVersion { get; set; }
    public Dictionary<string, string> Peppers { get; set; }
}
