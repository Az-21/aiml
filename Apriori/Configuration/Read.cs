using System.Text.Json;

namespace Apriori.Configuration;

public readonly record struct Config(string DataSource, int MinimumFrequency);
internal static class Read
{
  public static Config Config()
  {
    string raw = ReadConfigJson(ConstructConfigJsonPath());
    Config config = JsonSerializer.Deserialize<Config>(raw);
    WriteConfig(in config);
    return config;
  }

  private static string ConstructConfigJsonPath()
  {
    string relative = Directory.GetCurrentDirectory();
    return Path.Combine(relative, "Configuration", "Config.json");
  }

  private static string ReadConfigJson(in string path) => File.ReadAllText(path);

  private static void WriteConfig(in Config config)
  {
    Console.WriteLine(config.DataSource);
    Console.WriteLine($"Minimum frequency = {config.MinimumFrequency}");
  }
}
