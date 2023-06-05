namespace Apriori.Algorithm;
internal static class Subset
{
  internal static Dictionary<HashSet<string>, int> Generate(in Dictionary<string, int> input)
  {
    Dictionary<HashSet<string>, int> subsets = new(HashSet<string>.CreateSetComparer())
    {
      { new HashSet<string>(), 0 }
    };

    // Generate subsets
    foreach (var item in input)
    {
      int count = item.Value;
      foreach (HashSet<string> subset in (HashSet<HashSet<string>>)new(subsets.Keys, HashSet<string>.CreateSetComparer()))
      {
        for (int i = 1; i <= count; i++)
        {
          HashSet<string> newSubset = new(subset) { item.Key };
          if (!subsets.ContainsKey(newSubset)) { subsets.Add(newSubset, 0); }
        }
      }
    }

    return subsets;
  }
}
