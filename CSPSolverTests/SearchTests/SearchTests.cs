using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSPSolver.common;
using CSPSolver.Model;
using CSPSolver.Search;

namespace CSPSolverTests.SearchTests;

[TestClass]
public class SearchTests
{
    private static (ModelBuilder mb, ModelIntVar x, ModelIntVar y) SumsToSix()
    {
        var mb = new ModelBuilder();
        var x = mb.AddIntDomainVar(1, 5);
        var y = mb.AddIntDomainVar(1, 5);
        mb.AddConstraint(x + y == 6);
        return (mb, x, y);
    }

    private static IList<(int x, int y)> Pairs(IEnumerable<ISolution> search, ModelIntVar x, ModelIntVar y) =>
        search.Select(s => (s.GetValue(x.Variable), s.GetValue(y.Variable))).ToList();

    [TestMethod]
    public void EnumeratingASearchTwiceThrows()
    {
        var (mb, x, y) = SumsToSix();
        var search = new Search(mb);

        Assert.AreEqual(5, Pairs(search, x, y).Count);

        // Silently resuming from where the last pass stopped would report no
        // solutions at all, which is worse than a search that costs twice over.
        Assert.ThrowsExactly<InvalidOperationException>(() => Pairs(search, x, y));
    }

    [TestMethod]
    public void ResetAllowsASearchToRunAgainWithTheSameResult()
    {
        var (mb, x, y) = SumsToSix();
        var search = new Search(mb);

        var first = Pairs(search, x, y);
        search.Reset();
        var second = Pairs(search, x, y);

        CollectionAssert.AreEquivalent(first.ToList(), second.ToList());
        Assert.AreEqual(5, first.Count);
    }

    [TestMethod]
    public void PartiallyConsumingASearchStillBlocksASecondPass()
    {
        var (mb, _, _) = SumsToSix();
        var search = new Search(mb);

        using (var enumerator = search.GetEnumerator())
        {
            Assert.IsTrue(enumerator.MoveNext());
        }

        Assert.ThrowsExactly<InvalidOperationException>(() => search.GetEnumerator());
    }

    [TestMethod]
    public void ResetClearsTheObjectiveBoundFromThePreviousRun()
    {
        var mb = new ModelBuilder();
        var x = mb.AddIntDomainVar(1, 5);
        mb.AddObjective(x, maximise: true);
        var search = new Search(mb);

        var first = search.Cast<ISolution>().Select(s => s.GetValue(x.Variable)).ToList();
        search.Reset();
        var second = search.Cast<ISolution>().Select(s => s.GetValue(x.Variable)).ToList();

        // Without clearing Current, the second run starts bounded by the first
        // run's best and reports nothing.
        Assert.AreEqual(5, first.Last());
        CollectionAssert.AreEqual(first, second);
    }

    [TestMethod]
    public void DirectMoveNextUseIsUnaffected()
    {
        var (mb, _, _) = SumsToSix();
        var search = new Search(mb);

        var count = 0;
        while (search.MoveNext()) count++;

        Assert.AreEqual(5, count);
    }

    [TestMethod]
    public void AConstraintOverOnlyConstantsThatCannotHoldFailsImmediately()
    {
        var mb = new ModelBuilder();
        ModelIntVar five = 5;
        ModelIntVar six = 6;
        mb.AddConstraint(five == six);
        var search = new Search(mb);

        // No decision variables means IsSolved is vacuously true, and MakeEmpty
        // on a constant is a no-op, so nothing but an upfront CanBeMet check
        // catches this.
        Assert.IsFalse(search.MoveNext());
    }

    [TestMethod]
    public void AConstraintOverOnlyConstantsThatCanHoldStillSucceeds()
    {
        var mb = new ModelBuilder();
        ModelIntVar five = 5;
        ModelIntVar alsoFive = 5;
        mb.AddConstraint(five == alsoFive);
        var search = new Search(mb);

        Assert.IsTrue(search.MoveNext());
        Assert.IsFalse(search.MoveNext());
    }

    [TestMethod]
    public void ResetReappliesTheFeasibilityGuard()
    {
        var mb = new ModelBuilder();
        ModelIntVar five = 5;
        ModelIntVar six = 6;
        mb.AddConstraint(five == six);
        var search = new Search(mb);

        Assert.IsFalse(search.MoveNext());
        search.Reset();
        Assert.IsFalse(search.MoveNext());
    }
}