using System.Linq;

using BenchmarkDotNet.Attributes;

using CSPSolver.Model;

using static CSPSolver.Model.ModelConstraint;

namespace PerformanceTests.Benchmarks;

/// <summary>
/// SEND + MORE = MONEY: AllDiff combined with a single wide linear equation.
/// </summary>
[MemoryDiagnoser]
public class SendMoreMoneyBenchmarks
{
    [Benchmark]
    public int Solve()
    {
        var mb = new ModelBuilder();
        var s = mb.AddIntDomainVar(1, 9);
        var e = mb.AddIntDomainVar(0, 9);
        var n = mb.AddIntDomainVar(0, 9);
        var d = mb.AddIntDomainVar(0, 9);
        var m = mb.AddIntDomainVar(1, 9);
        var o = mb.AddIntDomainVar(0, 9);
        var r = mb.AddIntDomainVar(0, 9);
        var y = mb.AddIntDomainVar(0, 9);

        mb.AddConstraint(AllDiff(s, e, n, d, m, o, r, y));
        mb.AddConstraint(
            s * 1000 + e * 100 + n * 10 + d
            + m * 1000 + o * 100 + r * 10 + e
            == m * 10000 + o * 1000 + n * 100 + e * 10 + y);

        return mb.Search().Count();
    }
}
