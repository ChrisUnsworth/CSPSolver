using System.Linq;

using BenchmarkDotNet.Attributes;

using CSPSolver.common;
using CSPSolver.Model;

namespace PerformanceTests.Benchmarks;

/// <summary>
/// Classic N-Queens with one int var per column holding the queen's row.
/// Exercises binary not-equal propagation and search over many branches.
/// </summary>
[MemoryDiagnoser]
public class NQueensBenchmarks
{
    [Params(6, 8, 10)]
    public int N { get; set; }

    private static ModelBuilder BuildModel(int n)
    {
        var mb = new ModelBuilder();
        var queens = mb.AddIntVarArray(0, n - 1, n);

        for (int i = 0; i < n; i++)
        {
            for (int j = i + 1; j < n; j++)
            {
                mb.AddConstraint(queens[i] != queens[j]);
                mb.AddConstraint(queens[i] != queens[j] + (j - i));
                mb.AddConstraint(queens[j] != queens[i] + (j - i));
            }
        }

        return mb;
    }

    [Benchmark]
    public ISolution FirstSolution() => BuildModel(N).Search().First();

    [Benchmark]
    public int AllSolutions() => BuildModel(N).Search().Count();
}
