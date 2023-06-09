using Apriori.Algorithm;
using Apriori.Configuration;
using Spectre.Console;
using System.Globalization;

namespace Apriori;

internal static class Program
{
  static void Main()
  {
    // Load parameters
    Config rawConfig = Configuration.Read.Config();

    // Generate CSV hash
    HashSet<HashSet<string>> hashedCsv = Frequency.HashCsvLines(in rawConfig);
    Config config = rawConfig with
    {
      MinimumFrequency = (int)(rawConfig.MinimumSupport * hashedCsv.Count)
    };

    // Get frequency of individual items => Prune based on minimum frequency
    Dictionary<string, int> items = Frequency.OfIndividualItems(in hashedCsv, in config);

    // Generate all possible subsets from the pruned items dictionary
    Dictionary<HashSet<string>, int> subsets = Subset.Generate(items);

    // Get frequency of all subsets from the original CSV
    Frequency.UpdateFrequencyInplace(ref subsets, in hashedCsv);

    // Total number of transactions | Empty/null set contains number of lines
    int transactions = subsets[new HashSet<string>()];

    // Calculate support, confidence, and list
    Console.WriteLine();
    foreach (KeyValuePair<HashSet<string>, int> subset in subsets)
    {
      // Ignore sets with one item (basket analysis does not apply)
      if (subset.Key.Count <= 1) { continue; } // <=1 to account for null subset

      // Ignore subsets which lower than threshold frequency
      if (subset.Value < config.MinimumFrequency) { continue; }

      // Print outer rule AB
      string outerRule = "{ " + string.Join(", ", subset.Key) + " }";
      AnsiConsole.MarkupLine($"\nSubset [purple]{outerRule}[/] with f = {subset.Value}");

      // Create subsets of subset
      foreach (HashSet<string> innerSubset in Subset.GenerateX(subset.Key))
      {
        // Calculate A => B rule given AB
        HashSet<string> ruleA = innerSubset; // X Y Z
        HashSet<string> ruleB = new(subset.Key); // U X Y Z V
        ruleB.ExceptWith(ruleA); // U V

        // Skip empty rules
        if (ruleA.Count == 0 || ruleB.Count == 0) { continue; }

        // Calculate analysis indicators
        int frequencyA = subsets[ruleA];
        int frequencyB = subsets[ruleB];
        int frequencyAB = subsets[subset.Key];

        double supportA = Calculate.Support(frequencyA, in transactions);
        double supportB = Calculate.Support(frequencyB, in transactions);
        double supportAB = Calculate.Support(frequencyAB, in transactions);
        double confidence = Calculate.Confidence(in supportAB, in supportA);
        double lift = Calculate.Lift(in confidence, in supportB);

        // Skip low scoring rules
        bool isLowScoring =
          supportAB < config.MinimumSupport ||
          confidence < config.MinimumConfidence ||
          lift < config.MinimumLift;
        if (isLowScoring) { continue; }

        // Print subset rule in A => B format
        string a = "{ " + string.Join(", ", ruleA) + " }";
        string b = "{ " + string.Join(", ", ruleB) + " }";
        a = ToTitleCase(a);
        b = ToTitleCase(b);
        AnsiConsole.MarkupLine($"\n[green]{a}[/] [red]=>[/] [blue]{b}[/]");

        // Print analysis results
        Console.WriteLine($"Support = {supportAB}");
        Console.WriteLine($"Confidence = {confidence}");
        Console.WriteLine($"Lift = {lift}\n");
      }
      // Separator
      AnsiConsole.Write(new Rule());
    }
  }

  private static string ToTitleCase(string input)
  {
    TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
    return textInfo.ToTitleCase(input);
  }
}
