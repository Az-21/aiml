namespace Apriori.Algorithm;
internal static class Subset
{
  // Note: This function is the performance limiting part of this program
  // It does not prune supersets if the subset itself does not satisfy the minimum threshold
  internal static Dictionary<HashSet<string>, int> Generate(in Dictionary<string, int> input)
  {
    Dictionary<HashSet<string>, int> subsets = new(HashSet<string>.CreateSetComparer())
    {
        { new HashSet<string>(), 0 }
    };

    foreach (var item in input)
    {
      int count = item.Value;
      foreach (var subset in subsets.Keys.ToList())
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

  internal static HashSet<HashSet<string>> GenerateX(in HashSet<string> input)
  {
    HashSet<HashSet<string>> subsets = new(HashSet<string>.CreateSetComparer())
    {
        new HashSet<string>()
    };

    foreach (string item in input)
    {
      HashSet<HashSet<string>> newSubsets = new(subsets, HashSet<string>.CreateSetComparer());
      foreach (var subset in subsets)
      {
        HashSet<string> newSubset = new(subset) { item };
        newSubsets.Add(newSubset);
      }
      subsets = newSubsets;
    }

    return subsets;
  }
}
