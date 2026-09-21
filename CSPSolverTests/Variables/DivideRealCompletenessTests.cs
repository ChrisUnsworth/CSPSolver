using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSPSolver.Math.Divide;
using CSPSolver.Model;

namespace CSPSolverTests.Variables;

/// <summary>
/// The real-valued equivalent of DivideCompletenessTests. Uses dp = 0 so every
/// real var is backed by SmallRealVar over an integer grid, making it directly
/// comparable against brute force the same way the int sweep is.
/// </summary>
[TestClass]
public class DivideRealCompletenessTests
{
    private const int ZLo = -20;
    private const int ZHi = 20;

    private readonly record struct Window(int XLo, int XHi, int YLo, int YHi)
    {
        public override string ToString() => $"x[{XLo},{XHi}] y[{YLo},{YHi}]";
    }

    private static IEnumerable<Window> Windows()
    {
        for (var xlo = -4; xlo <= 3; xlo++)
            for (var xhi = xlo; xhi <= Math.Min(xlo + 4, 4); xhi++)
                for (var ylo = -3; ylo <= 2; ylo++)
                    for (var yhi = ylo; yhi <= Math.Min(ylo + 3, 3); yhi++)
                        if (ylo != 0 || yhi != 0)
                            yield return new Window(xlo, xhi, ylo, yhi);
    }

    private static ISet<(int x, int y, int z)> Expected(Window w)
    {
        var expected = new HashSet<(int, int, int)>();

        // Real division doesn't truncate, so a solution only exists where the
        // exact quotient happens to land on one of z's integer grid points.
        for (var x = w.XLo; x <= w.XHi; x++)
            for (var y = w.YLo; y <= w.YHi; y++)
                if (y != 0 && x % y == 0 && x / y >= ZLo && x / y <= ZHi)
                    expected.Add((x, y, x / y));

        return expected;
    }

    private static (Type divide, ISet<(int x, int y, int z)> found) Solve(Window w)
    {
        var mb = new ModelBuilder();
        var x = mb.AddRealVar(w.XLo, w.XHi, 0, isDecisionVar: true);
        var y = mb.AddRealVar(w.YLo, w.YHi, 0, isDecisionVar: true);
        var z = mb.AddRealVar(ZLo, ZHi, 0, isDecisionVar: true);

        var divide = (x / y).Variable.GetType();
        mb.AddConstraint(x / y == z);

        var found = mb.Search()
                      .Select(s => ((int)s.GetValue(x), (int)s.GetValue(y), (int)s.GetValue(z)))
                      .ToHashSet();

        return (divide, found);
    }

    private static void AssertCompleteFor(Type divideVariable)
    {
        var faults = new List<string>();
        var checkedWindows = 0;

        foreach (var window in Windows())
        {
            var (divide, found) = Solve(window);
            if (divide != divideVariable) continue;

            checkedWindows++;
            var expected = Expected(window);
            var missing = expected.Except(found).ToList();
            var bogus = found.Except(expected).ToList();

            if (missing.Count > 0 || bogus.Count > 0)
            {
                var detail = string.Join(" ", missing.Take(3).Select(m => $"missing {m.x}/{m.y}=={m.z}")
                                    .Concat(bogus.Take(3).Select(b => $"bogus {b.x}/{b.y}=={b.z}")));
                faults.Add($"{window}: {missing.Count} missing, {bogus.Count} bogus  {detail}");
            }
        }

        Assert.AreNotEqual(0, checkedWindows, $"no window routed to {divideVariable.Name}");
        Assert.AreEqual(
            0,
            faults.Count,
            $"{faults.Count} of {checkedWindows} windows wrong for {divideVariable.Name}:{Environment.NewLine}" +
            string.Join(Environment.NewLine, faults.Take(10)));
    }

    [TestMethod]
    public void DividePositiveRealFindsEverySolution() => AssertCompleteFor(typeof(DividePositiveRealVar));

    [TestMethod]
    public void MixedSignDivideRealFindsEverySolution() => AssertCompleteFor(typeof(MixedSignDivideRealVar));
}
