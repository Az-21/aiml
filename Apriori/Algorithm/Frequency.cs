using Apriori.Configuration;

namespace Apriori.Algorithm;
internal static class Frequency
{
  internal static Dictionary<string, int> OfIndividualItemsWithMinimumThreshold(in Config config)
  {
    Dictionary<string, int> items = Frequency.OfIndividualItems(in config);
    return Frequency.PruneBasedOnThreshold(in items, in config);
  }

  private static Dictionary<string, int> OfIndividualItems(in Config config)
  {
    // Dictionary with k:Item, v:Frequency
    Dictionary<string, int> items = new();

    // Read CSV line-by-line => Update items dictionary
    using StreamReader reader = new(config.DataSource);
    string line;
    while ((line = reader.ReadLine()!) is not null)
    {
      // Pre-process line
      line = line.Trim().ToLowerInvariant();

      // Append comma separated values to items dictionary
      foreach (string item in line.Split(','))
      {
        if (items.ContainsKey(item)) { items[item]++; }
        else { items.Add(item, 1); }
      }
    }

    return items;
  }

  private static Dictionary<string, int> PruneBasedOnThreshold(in Dictionary<string, int> items, in Config config)
  {
    Dictionary<string, int> pruned = new();
    foreach (KeyValuePair<string, int> kvp in items)
    {
      if (kvp.Value < config.MinimumFrequency) { continue; }
      // Add to pruned dictionary if the item has the minimum desired frequency
      pruned.Add(kvp.Key, kvp.Value);
    }

    return pruned;
  }

  internal static void UpdateFrequencyInplace(ref Dictionary<HashSet<string>, int> subsets, in Config config)
  {
    // Read CSV line-by-line => Update subset dictionary
    using StreamReader reader = new(config.DataSource);
    string line;
    while ((line = reader.ReadLine()!) is not null)
    {
      // Pre-process line
      line = line.Trim().ToLowerInvariant();

      // Convert line to hash set
      HashSet<string> lineItems = new(line.Split(','));

      // Check for occurance of subset in current line
      foreach (KeyValuePair<HashSet<string>, int> subset in subsets)
      {
        HashSet<string> subsetItems = subset.Key;
        if (!subsetItems.IsSubsetOf(lineItems)) { continue; }
        subsets[subsetItems]++; // Increment the count in the primary dictionary
      }
    }
  }

  internal static int CountNonEmptyLines(in Config config)
  {
    int lineCount = 0;
    string line;
    using StreamReader reader = File.OpenText(config.DataSource);
    while ((line = reader.ReadLine()!) != null)
    {
      line = line.Trim();
      if (!string.IsNullOrEmpty(line)) { lineCount++; }
    }

    return lineCount;
  }
}
