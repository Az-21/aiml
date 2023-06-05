﻿namespace Apriori.Algorithm;
internal static class Calculate
{
  // A => Subset, B => Subset
  // AB => A implies B
  internal static double Support(in int frequencyAB, in int transactions) => (double)frequencyAB / transactions;
  internal static double Confidence(in int frequencyAB, in int frequencyA) => (double)frequencyAB / frequencyA;
  internal static double Lift(in double confidence, in double support) => confidence / support;
}