using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSPSolver.common;
using CSPSolver.common.variables;
using CSPSolver.Math.Sum;
using CSPSolver.Model;
using CSPSolver.State;

namespace CSPSolverTests.Constraint;

[TestClass]
public class SumOfIntVarTests
{
    private static (IState state, IIntVar[] vars) GetVars(int count, int lo, int hi)
    {
        var mb = new ModelBuilder();
        var vars = mb.AddIntVarArray(lo, hi, count).Select(v => v.Variable).ToArray();

        var model = mb.GetModel();
        var statePool = new StatePool(mb.GetStateSize());
        var state = statePool.Empty();
        model.Initialise(state);

        return (state, vars);
    }

    [TestMethod]
    public void SetMaxNarrowsEachVarBySubtractingOthersMinimum()
    {
        var (state, vars) = GetVars(3, 0, 5);
        var sum = new SumOfIntVar(vars);

        // Others' combined min is 0, so each var's own max is capped at the target.
        Assert.IsTrue(sum.SetMax(state, 4));
        Assert.AreEqual(4, vars[0].GetDomainMax(state));
        Assert.AreEqual(4, vars[1].GetDomainMax(state));
        Assert.AreEqual(4, vars[2].GetDomainMax(state));
    }

    [TestMethod]
    public void SetMinNarrowsEachVarBySubtractingOthersMaximum()
    {
        var (state, vars) = GetVars(3, 0, 5);
        var sum = new SumOfIntVar(vars);

        // Others' combined max is 10 (two vars at 5 each), so each var's own min
        // is forced up to 13 - 10 = 3.
        Assert.IsTrue(sum.SetMin(state, 13));
        Assert.AreEqual(3, vars[0].GetDomainMin(state));
        Assert.AreEqual(3, vars[1].GetDomainMin(state));
        Assert.AreEqual(3, vars[2].GetDomainMin(state));
    }

    [TestMethod]
    public void RemoveValueOnlyActsOnceEveryOtherVarIsFixed()
    {
        var (state, vars) = GetVars(3, 0, 5);
        var sum = new SumOfIntVar(vars);

        Assert.IsFalse(sum.RemoveValue(state, 10));

        vars[0].SetValue(state, 2);
        vars[1].SetValue(state, 3);

        // Only vars[2] is undecided; a sum of 10 would need it to be 5, its
        // current max, so removing that possibility drops the max to 4.
        Assert.IsTrue(sum.RemoveValue(state, 10));
        Assert.AreEqual(4, vars[2].GetDomainMax(state));
    }

    [TestMethod]
    public void RemoveValueEmptiesAVarWhenAllAreFixedAndSumMatches()
    {
        var (state, vars) = GetVars(3, 0, 5);
        var sum = new SumOfIntVar(vars);

        vars[0].SetValue(state, 2);
        vars[1].SetValue(state, 3);
        vars[2].SetValue(state, 5);

        // Every var is fixed and they sum to 10; removing 10 has no other
        // var left to absorb the change, so it must empty one of them.
        Assert.IsTrue(sum.RemoveValue(state, 10));
        Assert.IsTrue(sum.IsEmpty(state));
    }

    [TestMethod]
    public void RemoveValueDoesNothingWhenAllAreFixedAndSumDiffers()
    {
        var (state, vars) = GetVars(3, 0, 5);
        var sum = new SumOfIntVar(vars);

        vars[0].SetValue(state, 2);
        vars[1].SetValue(state, 3);
        vars[2].SetValue(state, 5);

        Assert.IsFalse(sum.RemoveValue(state, 11));
        Assert.IsFalse(sum.IsEmpty(state));
    }

    [TestMethod]
    public void MatchesBruteForceWithAConstantTarget()
    {
        for (var target = -2; target <= 10; target++)
        {
            AssertMatchesBruteForceConstant(3, 0, 3, target);
        }
    }

    private static void AssertMatchesBruteForceConstant(int count, int lo, int hi, int target)
    {
        var mb = new ModelBuilder();
        var vars = mb.AddIntVarArray(lo, hi, count);
        mb.AddConstraint(ModelIntVar.SumOf(vars) == target);

        var found = Solve(mb, vars);
        var expected = BruteForce(count, lo, hi, target);

        CollectionAssert.AreEquivalent(expected.ToList(), found.ToList(), $"target {target}");
    }

    [TestMethod]
    public void MatchesBruteForceWithAVariableTarget()
    {
        var mb = new ModelBuilder();
        var vars = mb.AddIntVarArray(0, 3, 3);
        var target = mb.AddIntDomainVar(-2, 10);
        mb.AddConstraint(ModelIntVar.SumOf(vars) == target);

        var found = mb.Search()
            .Select(s => string.Join(",", vars.Select(v => s.GetValue(v))) + "=" + Values(s, target))
            .ToHashSet();

        var expected = new HashSet<string>();
        for (var a = 0; a <= 3; a++)
            for (var b = 0; b <= 3; b++)
                for (var c = 0; c <= 3; c++)
                {
                    var sum = a + b + c;
                    if (sum >= -2 && sum <= 10)
                    {
                        expected.Add($"{a},{b},{c}={sum}");
                    }
                }

        CollectionAssert.AreEquivalent(expected.ToList(), found.ToList());
    }

    private static int Values(ISolution s, ModelIntVar v) => s.GetValue(v);

    private static ISet<string> Solve(ModelBuilder mb, ModelIntVar[] vars) =>
        mb.Search()
          .Select(s => string.Join(",", vars.Select(v => s.GetValue(v))))
          .ToHashSet();

    private static ISet<string> BruteForce(int count, int lo, int hi, int target)
    {
        var results = new HashSet<string>();
        Recurse(new int[count], 0, lo, hi, target, results);
        return results;
    }

    private static void Recurse(int[] values, int index, int lo, int hi, int target, ISet<string> results)
    {
        if (index == values.Length)
        {
            if (values.Sum() == target)
            {
                results.Add(string.Join(",", values));
            }

            return;
        }

        for (var v = lo; v <= hi; v++)
        {
            values[index] = v;
            Recurse(values, index + 1, lo, hi, target, results);
        }
    }
}