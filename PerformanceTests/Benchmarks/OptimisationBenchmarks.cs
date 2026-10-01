using System.Linq;

using BenchmarkDotNet.Attributes;

using CSPSolver.common;
using CSPSolver.Model;

namespace PerformanceTests.Benchmarks;

/// <summary>
/// A small bounded knapsack: maximise value subject to a weight limit.
/// Exercises objective-driven search, where each solution tightens the bound.
/// </summary>
[MemoryDiagnoser]
public class OptimisationBenchmarks
{
    private static readonly int[] Weights = [12, 7, 11, 8, 9, 6, 14, 5];
    private static readonly int[] Values = [24, 13, 23, 15, 16, 11, 28, 8];
    private const int Capacity = 60;

    [Benchmark]
    public ISolution Knapsack()
    {
        var mb = new ModelBuilder();
        var counts = mb.AddIntVarArray(0, 3, Weights.Length);

        mb.AddConstraint(ModelIntVar.SumOf(counts.Select((c, i) => c * Weights[i])) <= Capacity);
        mb.AddObjective(ModelIntVar.SumOf(counts.Select((c, i) => c * Values[i])), maximise: true);

        return mb.Search().Last();
    }
}
