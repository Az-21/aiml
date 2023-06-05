using Apriori.Algorithm;
using Apriori.Configuration;

namespace Apriori;

internal static class Program
{
  static void Main()
  {
    // Load parameters
    Config config = Configuration.Read.Config();

    // Get frequency of individual items => Prune based on minimum frequency
    Dictionary<string, int> items = Frequency.OfIndividualItemsWithMinimumThreshold(in config);

    // Generate all possible subsets from the pruned items dictionary
    Dictionary<HashSet<string>, int> subsets = Subset.Generate(items);

    // Get frequency of all subsets from the original CSV
    Frequency.UpdateFrequencyInplace(ref subsets, in config);

    // Get total number of transactions
    int transactions = Frequency.CountNonEmptyLines(in config);

    // Calculate support, confidence, and list
    Console.WriteLine();
    foreach (KeyValuePair<HashSet<string>, int> subset in subsets)
    {
      // Ignore sets with one item (basket analysis does not apply)
      if (subset.Key.Count <= 1) { continue; } // <=1 to account for null subset

      // Split subset into {A} || {B} = {x1, x2, ..., xN-1} || {xN}
      HashSet<string> subsetAB = subset.Key;
      HashSet<string> subsetA = new(subsetAB.Take(subsetAB.Count - 1));

      // Calculate analysis indicators
      double support = Calculate.Support(subsetAB.Count, in transactions);
      double confidence = Calculate.Confidence(subsetAB.Count, subsetA.Count);
      double lift = Calculate.Lift(in confidence, in support);

      // Skip low scoring rules
      bool isLowScoring =
        support < config.MinimumSupport ||
        confidence < config.MinimumConfidence ||
        lift < config.MinimumLift;
      if (isLowScoring) { continue; }

      // Print subset rule in A => B format
      string a = "{ " + string.Join(", ", subsetA) + " }";
      string b = subsetAB.Last();
      Console.WriteLine($"{a} => {b}");

      // Print analysis results
      Console.WriteLine($"Support = {support}");
      Console.WriteLine($"Confidence = {confidence}");
      Console.WriteLine($"Lift = {lift}\n");
    }
  }
}
