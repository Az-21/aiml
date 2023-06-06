using Apriori.Configuration;

namespace Apriori.Algorithm;
internal static class Frequency
{
  internal static Dictionary<string, int> OfIndividualItemsWithMinimumThreshold(in HashSet<HashSet<string>> hashedCsv, in Config config)
  {
    Dictionary<string, int> items = Frequency.OfIndividualItems(in hashedCsv, in config);
    return Frequency.PruneBasedOnThreshold(in items, in config);
  }

  private static Dictionary<string, int> OfIndividualItems(in HashSet<HashSet<string>> hashedCsv, in Config config)
  {
    // Dictionary with k:Item, v:Frequency
    Dictionary<string, int> items = new();

    foreach (HashSet<string> line in hashedCsv)
    {
      foreach (string item in line)
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

  internal static void UpdateFrequencyInplace(ref Dictionary<HashSet<string>, int> subsets, in HashSet<HashSet<string>> hashedCsv)
  {
    // Check for occurance of subset in current line
    foreach (KeyValuePair<HashSet<string>, int> subset in subsets)
    {
      HashSet<string> subsetItems = subset.Key;
      foreach (HashSet<string> line in hashedCsv)
      {
        if (subsetItems.IsSubsetOf(line)) { subsets[subsetItems]++; }
      }
    }
  }

  internal static HashSet<HashSet<string>> HashCsvLines(in Config config)
  {
    HashSet<HashSet<string>> hashedCsv = new();
    using StreamReader reader = new(config.DataSource);
    string line;
    while ((line = reader.ReadLine()!) is not null)
    {
      // Pre-process line
      line = line.Trim().ToLowerInvariant();

      // Convert line to hash set => Add to hashList
      hashedCsv.Add(new(line.Split(',')));
    }

    return hashedCsv;
  }
}
