using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSPSolver.common;
using CSPSolver.common.variables;
using CSPSolver.Constraint.Bool;
using CSPSolver.Model;
using CSPSolver.State;

using static CSPSolver.Model.ModelConstraint;

namespace CSPSolverTests.Constraint;

[TestClass]
public class CardinalityVarTests
{
    private static (IState state, IBoolVar[] vars, IIntVar count) GetVars(int size, int countLo, int countHi)
    {
        var mb = new ModelBuilder();
        var vars = mb.AddBoolVarArray(size).Select(v => v.Variable).ToArray();
        var count = mb.AddIntDomainVar(countLo, countHi).Variable;

        var model = mb.GetModel();
        var statePool = new StatePool(mb.GetStateSize());
        var state = statePool.Empty();
        model.Initialise(state);

        return (state, vars, count);
    }

    [TestMethod]
    public void SetsCountBoundsFromTheTrueAndUndecidedSplit()
    {
        var (state, vars, count) = GetVars(4, 0, 4);
        var constraint = new CardinalityVar(vars, count);

        vars[0].SetValue(state, true);
        vars[1].SetValue(state, false);
        constraint.Propagate(state);

        // 1 already true, up to 2 more (vars[2], vars[3]) could join it.
        Assert.AreEqual(1, count.GetDomainMin(state));
        Assert.AreEqual(3, count.GetDomainMax(state));
    }

    [TestMethod]
    public void ForcesRemainingFalseOnceCountsMaxIsReached()
    {
        var (state, vars, count) = GetVars(4, 0, 1);
        var constraint = new CardinalityVar(vars, count);

        vars[0].SetValue(state, true);
        constraint.Propagate(state);

        Assert.IsTrue(vars[1].IsFalse(state));
        Assert.IsTrue(vars[2].IsFalse(state));
        Assert.IsTrue(vars[3].IsFalse(state));
    }

    [TestMethod]
    public void ForcesRemainingTrueOnceCountsMinLeavesNoSlack()
    {
        var (state, vars, count) = GetVars(4, 3, 4);
        var constraint = new CardinalityVar(vars, count);

        vars[0].SetValue(state, false);
        constraint.Propagate(state);

        Assert.IsTrue(vars[1].IsTrue(state));
        Assert.IsTrue(vars[2].IsTrue(state));
        Assert.IsTrue(vars[3].IsTrue(state));
    }

    [TestMethod]
    public void EmptiesCountWhenAlreadyTooManyAreTrue()
    {
        var (state, vars, count) = GetVars(3, 0, 1);
        var constraint = new CardinalityVar(vars, count);

        vars[0].SetValue(state, true);
        vars[1].SetValue(state, true);
        constraint.Propagate(state);

        Assert.IsTrue(count.IsEmpty(state));
    }

    [TestMethod]
    public void EmptiesCountWhenTooFewCanStillBecomeTrue()
    {
        var (state, vars, count) = GetVars(3, 3, 3);
        var constraint = new CardinalityVar(vars, count);

        vars[0].SetValue(state, false);
        vars[1].SetValue(state, false);
        constraint.Propagate(state);

        Assert.IsTrue(count.IsEmpty(state));
    }

    [TestMethod]
    public void IsMetOnlyOnceEveryVarIsDecidedAndCountIsInstantiatedToMatch()
    {
        var (state, vars, count) = GetVars(3, 0, 3);
        var constraint = new CardinalityVar(vars, count);

        vars[0].SetValue(state, true);
        vars[1].SetValue(state, true);
        vars[2].SetValue(state, false);
        Assert.IsFalse(constraint.IsMet(state));

        count.SetValue(state, 2);
        Assert.IsTrue(constraint.IsMet(state));
    }

    [TestMethod]
    public void CanBeMetReflectsOverlapBetweenTheAchievableRangeAndCountsDomain()
    {
        var (state, vars, count) = GetVars(3, 2, 3);
        var constraint = new CardinalityVar(vars, count);

        Assert.IsTrue(constraint.CanBeMet(state));

        vars[0].SetValue(state, false);
        vars[1].SetValue(state, false);
        Assert.IsFalse(constraint.CanBeMet(state));
    }

    [TestMethod]
    public void NegativePropagateForcesTheLastVarAwayFromCompletingAFixedCount()
    {
        var (state, vars, count) = GetVars(3, 2, 2);
        var constraint = new CardinalityVar(vars, count);

        vars[0].SetValue(state, true);
        vars[1].SetValue(state, false);
        var changed = constraint.NegativePropagate(state).ToList();

        Assert.AreEqual(1, changed.Count);
        Assert.IsTrue(vars[2].IsFalse(state));
    }

    [TestMethod]
    public void NegativePropagateRemovesTheMatchingValueFromCountWhenVarsAreAllDecided()
    {
        var (state, vars, count) = GetVars(2, 0, 2);
        var constraint = new CardinalityVar(vars, count);

        vars[0].SetValue(state, true);
        vars[1].SetValue(state, false);
        var changed = constraint.NegativePropagate(state).ToList();

        Assert.AreEqual(1, changed.Count);
        Assert.IsFalse(count.RemoveValue(state, 1));
    }

    [TestMethod]
    public void NegativePropagateEmptiesCountWhenItAlreadyMatchesExactly()
    {
        var (state, vars, count) = GetVars(2, 1, 1);
        var constraint = new CardinalityVar(vars, count);

        vars[0].SetValue(state, true);
        vars[1].SetValue(state, false);
        constraint.NegativePropagate(state);

        Assert.IsTrue(vars.Any(v => v.IsEmpty(state)));
    }

    [TestMethod]
    public void MatchesBruteForceAcrossEveryCountDomainAndSize()
    {
        for (var size = 1; size <= 4; size++)
        {
            AssertMatchesBruteForce(size, 0, size);
        }
    }

    private static void AssertMatchesBruteForce(int size, int countLo, int countHi)
    {
        var mb = new ModelBuilder();
        var vars = mb.AddBoolVarArray(size);
        var count = mb.AddIntDomainVar(countLo, countHi);
        mb.AddConstraint(Cardinality(vars, count));

        var found = mb.Search()
            .Select(s => string.Concat(vars.Select(v => s.GetValue(v) ? '1' : '0')) + "=" + s.GetValue(count))
            .ToHashSet();

        var expected = new HashSet<string>();
        for (var bits = 0; bits < (1 << size); bits++)
        {
            var popCount = System.Numerics.BitOperations.PopCount((uint)bits);
            if (popCount < countLo || popCount > countHi) continue;

            var assignment = new char[size];
            for (var i = 0; i < size; i++)
            {
                assignment[i] = (bits & (1 << i)) != 0 ? '1' : '0';
            }
            expected.Add(new string(assignment) + "=" + popCount);
        }

        CollectionAssert.AreEquivalent(
            expected.ToList(),
            found.ToList(),
            $"size {size}, count range [{countLo},{countHi}]");
    }
}
