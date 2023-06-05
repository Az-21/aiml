namespace Apriori.Algorithm;
internal static class Calculate
{
  // A => Subset, B => Subset
  // AB => A implies B
  internal static double Support(in int frequencyAB, in int transactions) => (double)frequencyAB / transactions;
  internal static double Confidence(in double supportAB, in double supportA) => supportAB / supportA;
  internal static double Lift(in double confidenceAB, in double supportB) => confidenceAB / supportB;
}
