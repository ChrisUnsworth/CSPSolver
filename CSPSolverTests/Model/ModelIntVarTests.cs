using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSPSolver.Math.Divide;
using CSPSolver.Model;

namespace CSPSolverTests.Model;

[TestClass]
public class ModelIntVarTests
{
    private static (ModelBuilder mb, ModelIntVar x, ModelIntVar y, ModelIntVar z) Model(
        int xlo, int xhi, int ylo, int yhi, int zlo, int zhi)
    {
        var mb = new ModelBuilder();
        return (mb, mb.AddIntDomainVar(xlo, xhi), mb.AddIntDomainVar(ylo, yhi), mb.AddIntDomainVar(zlo, zhi));
    }

    private static ISet<(int x, int y, int z)> Solve(ModelBuilder mb, ModelIntVar x, ModelIntVar y, ModelIntVar z) =>
        mb.Search()
            .Select(s => (s.GetValue(x.Variable), s.GetValue(y.Variable), s.GetValue(z.Variable)))
            .ToHashSet();

    private static ISet<(int x, int y, int z)> Expected(int xlo, int xhi, int ylo, int yhi, int zlo, int zhi)
    {
        var expected = new HashSet<(int, int, int)>();

        for (var xv = xlo; xv <= xhi; xv++)
            for (var yv = ylo; yv <= yhi; yv++)
                if (yv != 0 && xv / yv >= zlo && xv / yv <= zhi)
                    expected.Add((xv, yv, xv / yv));

        return expected;
    }

    [TestMethod]
    public void DenominatorExcludingZeroUsesPositiveDivide()
    {
        var (_, x, y, _) = Model(1, 10, 1, 3, 0, 10);

        Assert.IsInstanceOfType<PositiveDivideIntVar>((x / y).Variable);
    }

    [TestMethod]
    public void DenominatorContainingZeroUsesMixedSignDivide()
    {
        var (_, x, y, _) = Model(1, 10, 0, 3, 0, 10);

        // PositiveDivideIntVar bars a zero denominator, so these route to the
        // one implementation that prunes zero rather than faulting on it.
        Assert.IsInstanceOfType<MixedSignDivideIntVar>((x / y).Variable);
    }

    [TestMethod]
    public void DividingByADomainContainingZeroBuildsAndSolves()
    {
        var (mb, x, y, z) = Model(1, 10, 0, 3, 0, 10);
        mb.AddConstraint(x / y == z);

        var solutions = Solve(mb, x, y, z);

        // Whatever is found must be arithmetically true and never divide by zero.
        Assert.IsTrue(solutions.Count > 0);
        Assert.IsFalse(solutions.Any(s => s.y == 0), "a zero denominator reached a solution");
        Assert.IsFalse(solutions.Any(s => s.z != s.x / s.y), "a solution had the wrong quotient");
    }

    [TestMethod]
    public void DividingByADomainExcludingZeroFindsEverySolution()
    {
        var (mb, x, y, z) = Model(1, 10, 1, 3, 0, 10);
        mb.AddConstraint(x / y == z);

        CollectionAssert.AreEquivalent(
            Expected(1, 10, 1, 3, 0, 10).ToList(),
            Solve(mb, x, y, z).ToList());
    }

    [TestMethod]
    public void DividingByADomainContainingZeroFindsEverySolution()
    {
        // KNOWN FAILURE - see issue #18. MixedSignDivideIntVar is incomplete and
        // loses 1/2==0 and 1/3==0 here, returning 28 of 30. Routing a zero
        // denominator there trades a DivideByZeroException for silently short
        // results, so this stays red until #18 is fixed.
        var (mb, x, y, z) = Model(1, 10, 0, 3, 0, 10);
        mb.AddConstraint(x / y == z);

        CollectionAssert.AreEquivalent(
            Expected(1, 10, 0, 3, 0, 10).ToList(),
            Solve(mb, x, y, z).ToList());
    }
}