﻿using Apriori.Algorithm;
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

    // Total number of transactions | Empty/null set contains number of lines
    int transactions = subsets[new HashSet<string>()];

    // Calculate support, confidence, and list
    Console.WriteLine();
    foreach (KeyValuePair<HashSet<string>, int> subset in subsets)
    {
      // Ignore sets with one item (basket analysis does not apply)
      if (subset.Key.Count <= 1) { continue; } // <=1 to account for null subset

      // Ignore sets which don't exist in CSV (will throw div by 0 error)
      if (subset.Value == 0) { continue; }

      // Split subset into {A} || {B} = {x1, x2, ..., xN-1} || {xN}
      HashSet<string> subsetAB = subset.Key;
      HashSet<string> subsetA = new(subsetAB.Take(subsetAB.Count - 1));
      HashSet<string> subsetB = new(subsetAB.Skip(subsetAB.Count - 1));

      // Calculate analysis indicators
      int frequencyA = subsets[subsetA];
      int frequencyB = subsets[subsetB];
      int frequencyAB = subsets[subsetAB];

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
      string a = "{ " + string.Join(", ", subsetA) + " }";
      string b = subsetAB.Last();
      Console.WriteLine($"{a} => {b}");

      // Print analysis results
      Console.WriteLine($"Support = {supportAB}");
      Console.WriteLine($"Confidence = {confidence}");
      Console.WriteLine($"Lift = {lift}\n");
    }
  }
}
