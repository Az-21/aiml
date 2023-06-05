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
    Dictionary<string, int> items = Frequency.OfIndividualItems(in config);

  }
}
