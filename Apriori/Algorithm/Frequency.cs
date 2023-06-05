using Apriori.Configuration;

namespace Apriori.Algorithm;
internal static class Frequency
{
  internal static Dictionary<string, int> OfIndividualItems(in Config config)
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
}
