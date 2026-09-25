using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSPSolver.common;
using CSPSolver.common.variables;
using CSPSolver.Math.Sum;
using CSPSolver.Model;
using CSPSolver.State;
using CSPSolver.Variable;

namespace CSPSolverTests.Constraint;

[TestClass]
public class SumOfRealVarTests
{
    private static (IState state, IRealVar[] vars) GetVars(int count, double lo, double hi)
    {
        var sb = new StateBuilder();
        var vars = new IRealVar[count];
        for (var i = 0; i < count; i++)
        {
            vars[i] = new RealVar(lo, sb.AddDouble(), hi, sb.AddDouble());
        }

        var state = sb.GetState();
        foreach (var v in vars) v.Initialise(state);

        return (state, vars);
    }

    [TestMethod]
    public void SetMaxNarrowsEachVarBySubtractingOthersMinimum()
    {
        var (state, vars) = GetVars(3, 0, 5);
        var sum = new SumOfRealVar(vars);

        Assert.IsTrue(sum.SetMax(state, 4));
        Assert.AreEqual(4, vars[0].GetDomainMax(state));
        Assert.AreEqual(4, vars[1].GetDomainMax(state));
        Assert.AreEqual(4, vars[2].GetDomainMax(state));
    }

    [TestMethod]
    public void SetMinNarrowsEachVarBySubtractingOthersMaximum()
    {
        var (state, vars) = GetVars(3, 0, 5);
        var sum = new SumOfRealVar(vars);

        Assert.IsTrue(sum.SetMin(state, 13));
        Assert.AreEqual(3, vars[0].GetDomainMin(state));
        Assert.AreEqual(3, vars[1].GetDomainMin(state));
        Assert.AreEqual(3, vars[2].GetDomainMin(state));
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
        var vars = new ModelRealVar[count];
        for (var i = 0; i < count; i++) vars[i] = mb.AddRealVar(lo, hi, 0, isDecisionVar: true);

        mb.AddConstraint(ModelRealVar.SumOf(vars) == (ModelIntVar)target);

        var found = Solve(mb, vars);
        var expected = BruteForce(count, lo, hi, target);

        CollectionAssert.AreEquivalent(expected.ToList(), found.ToList(), $"target {target}");
    }

    [TestMethod]
    public void MatchesBruteForceWithAVariableTarget()
    {
        var mb = new ModelBuilder();
        var vars = new[]
        {
            mb.AddRealVar(0, 3, 0, isDecisionVar: true),
            mb.AddRealVar(0, 3, 0, isDecisionVar: true),
            mb.AddRealVar(0, 3, 0, isDecisionVar: true),
        };
        var target = mb.AddRealVar(-2, 10, 0, isDecisionVar: true);
        mb.AddConstraint(ModelRealVar.SumOf(vars) == target);

        var found = mb.Search()
            .Select(s => string.Join(",", vars.Select(v => (int)s.GetValue(v))) + "=" + (int)s.GetValue(target))
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

    [TestMethod]
    public void SumOfAMixedIntAndRealCollectionWorks()
    {
        var mb = new ModelBuilder();
        var i1 = mb.AddIntDomainVar(0, 3);
        var i2 = mb.AddIntDomainVar(0, 3);
        var r1 = mb.AddRealVar(0, 3, 0, isDecisionVar: true);

        // ModelIntVar -> ModelRealVar converts implicitly when the list element
        // type is ModelRealVar, so a mixed collection needs no extra wiring.
        mb.AddConstraint(ModelRealVar.SumOf(new ModelRealVar[] { i1, i2, r1 }) == (ModelIntVar)5);

        var found = mb.Search()
            .Select(s => (s.GetValue(i1), s.GetValue(i2), (int)s.GetValue(r1)))
            .ToHashSet();

        var expected = new HashSet<(int, int, int)>();
        for (var a = 0; a <= 3; a++)
            for (var b = 0; b <= 3; b++)
                for (var c = 0; c <= 3; c++)
                    if (a + b + c == 5)
                        expected.Add((a, b, c));

        CollectionAssert.AreEquivalent(expected.ToList(), found.ToList());
    }

    private static ISet<string> Solve(ModelBuilder mb, ModelRealVar[] vars) =>
        mb.Search()
          .Select(s => string.Join(",", vars.Select(v => (int)s.GetValue(v))))
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
