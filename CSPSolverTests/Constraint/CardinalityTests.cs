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
public class CardinalityTests
{
    private static (IState state, IBoolVar[] vars) GetVars(int count)
    {
        var mb = new ModelBuilder();
        var vars = mb.AddBoolVarArray(count).Select(v => v.Variable).ToArray();

        var model = mb.GetModel();
        var statePool = new StatePool(mb.GetStateSize());
        var state = statePool.Empty();
        model.Initialise(state);

        return (state, vars);
    }

    [TestMethod]
    public void ForcesRemainingFalseOnceCountIsReached()
    {
        var (state, vars) = GetVars(4);
        var constraint = new Cardinality(vars, 1);

        vars[0].SetValue(state, true);
        var changed = constraint.Propagate(state).ToList();

        Assert.AreEqual(3, changed.Count);
        Assert.IsTrue(vars[1].IsFalse(state));
        Assert.IsTrue(vars[2].IsFalse(state));
        Assert.IsTrue(vars[3].IsFalse(state));
    }

    [TestMethod]
    public void ForcesRemainingTrueOnceThereIsNoOtherWayToReachTheCount()
    {
        var (state, vars) = GetVars(4);
        var constraint = new Cardinality(vars, 3);

        vars[0].SetValue(state, false);
        var changed = constraint.Propagate(state).ToList();

        Assert.AreEqual(3, changed.Count);
        Assert.IsTrue(vars[1].IsTrue(state));
        Assert.IsTrue(vars[2].IsTrue(state));
        Assert.IsTrue(vars[3].IsTrue(state));
    }

    [TestMethod]
    public void EmptiesADomainWhenAlreadyTooManyAreTrue()
    {
        var (state, vars) = GetVars(3);
        var constraint = new Cardinality(vars, 1);

        vars[0].SetValue(state, true);
        vars[1].SetValue(state, true);
        constraint.Propagate(state);

        Assert.IsTrue(vars.Any(v => v.IsEmpty(state)));
    }

    [TestMethod]
    public void EmptiesADomainWhenTooFewCanStillBecomeTrue()
    {
        var (state, vars) = GetVars(3);
        var constraint = new Cardinality(vars, 3);

        vars[0].SetValue(state, false);
        vars[1].SetValue(state, false);
        constraint.Propagate(state);

        Assert.IsTrue(vars.Any(v => v.IsEmpty(state)));
    }

    [TestMethod]
    public void IsMetOnlyOnceEveryVarIsDecidedAndTheCountMatches()
    {
        var (state, vars) = GetVars(3);
        var constraint = new Cardinality(vars, 2);

        Assert.IsFalse(constraint.IsMet(state));

        vars[0].SetValue(state, true);
        vars[1].SetValue(state, true);
        Assert.IsFalse(constraint.IsMet(state));

        vars[2].SetValue(state, false);
        Assert.IsTrue(constraint.IsMet(state));
    }

    [TestMethod]
    public void CanBeMetReflectsTheAchievableRange()
    {
        var (state, vars) = GetVars(3);
        var constraint = new Cardinality(vars, 2);

        Assert.IsTrue(constraint.CanBeMet(state));

        vars[0].SetValue(state, false);
        vars[1].SetValue(state, false);
        Assert.IsFalse(constraint.CanBeMet(state));
    }

    [TestMethod]
    public void NegativePropagateForcesTheLastVarAwayFromCompletingTheCount()
    {
        var (state, vars) = GetVars(3);
        var constraint = new Cardinality(vars, 2);

        vars[0].SetValue(state, true);
        vars[1].SetValue(state, false);
        // Only vars[2] is undecided; setting it true would make the count exactly
        // 2, which the negation must avoid.
        var changed = constraint.NegativePropagate(state).ToList();

        Assert.AreEqual(1, changed.Count);
        Assert.IsTrue(vars[2].IsFalse(state));
    }

    [TestMethod]
    public void NegativePropagateEmptiesADomainWhenTheCountAlreadyMatchesExactly()
    {
        var (state, vars) = GetVars(2);
        var constraint = new Cardinality(vars, 1);

        vars[0].SetValue(state, true);
        vars[1].SetValue(state, false);
        constraint.NegativePropagate(state);

        Assert.IsTrue(vars.Any(v => v.IsEmpty(state)));
    }

    [TestMethod]
    public void MatchesBruteForceAcrossEveryCountAndSize()
    {
        for (var size = 1; size <= 5; size++)
        {
            for (var count = 0; count <= size; count++)
            {
                AssertMatchesBruteForce(size, count);
            }
        }
    }

    private static void AssertMatchesBruteForce(int size, int count)
    {
        var mb = new ModelBuilder();
        var vars = mb.AddBoolVarArray(size);
        mb.AddConstraint(Cardinality(vars, count));

        var found = mb.Search()
            .Select(s => string.Concat(vars.Select(v => s.GetValue(v) ? '1' : '0')))
            .ToHashSet();

        var expected = new HashSet<string>();
        for (var bits = 0; bits < (1 << size); bits++)
        {
            if (System.Numerics.BitOperations.PopCount((uint)bits) != count) continue;

            var assignment = new char[size];
            for (var i = 0; i < size; i++)
            {
                assignment[i] = (bits & (1 << i)) != 0 ? '1' : '0';
            }
            expected.Add(new string(assignment));
        }

        CollectionAssert.AreEquivalent(
            expected.ToList(),
            found.ToList(),
            $"size {size}, count {count}");
    }
}