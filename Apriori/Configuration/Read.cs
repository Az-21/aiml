using Spectre.Console;
using System.Text.Json;

namespace Apriori.Configuration;

public readonly record struct Config(
  string DataSource,
  string RuleSet,
  bool UseRuleSet,
  int MinimumFrequency,
  double MinimumSupport,
  double MinimumConfidence,
  double MinimumLift
 );

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
    Table table = new Table()
      .AddColumn("[green]Attribute[/]")
      .AddColumn("[green]Value[/]")
      .AddRow("Data source", config.DataSource)
      .AddRow("Minimum frequency", config.MinimumFrequency.ToString())
      .AddRow("Minimum support", config.MinimumSupport.ToString())
      .AddRow("Minimum confidence", config.MinimumConfidence.ToString())
      .AddRow("Minimum lift", config.MinimumLift.ToString());

    if (config.UseRuleSet) { table.AddRow("Rule set", config.RuleSet); }

    table.Border(TableBorder.Rounded);
    AnsiConsole.Write(table);
  }
}
