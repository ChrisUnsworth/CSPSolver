using System.Linq;

using BenchmarkDotNet.Attributes;

using CSPSolver.Model;

using static CSPSolver.Model.ModelConstraint;

namespace PerformanceTests.Benchmarks;

/// <summary>
/// AllDiff nested in logic constraints, which reach the paths a plain AllDiff never uses:
/// Not calls NegativePropagate, and Or calls IsMet and CanBeMet.
/// </summary>
[MemoryDiagnoser]
public class LogicAllDiffBenchmarks
{
    // 6 vars over 1..6 that are not all different: 6^6 - 6! = 45,936 solutions.
    [Benchmark]
    public int NotAllDiff()
    {
        var mb = new ModelBuilder();
        var vars = mb.AddIntVarArray(1, 6, 6);

        mb.AddConstraint(Not(AllDiff(vars)));

        return mb.Search().Count();
    }

    // Two groups of 4 vars over 1..4, at least one all different: 2 * 4! * 4^4 - (4!)^2 = 11,712 solutions.
    [Benchmark]
    public int OrAllDiff()
    {
        var mb = new ModelBuilder();
        var x = mb.AddIntVarArray(1, 4, 4);
        var y = mb.AddIntVarArray(1, 4, 4);

        mb.AddConstraint(AllDiff(x) | AllDiff(y));

        return mb.Search().Count();
    }
}
