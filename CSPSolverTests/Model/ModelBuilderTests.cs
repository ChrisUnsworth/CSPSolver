using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSPSolver.Model;
using CSPSolver.State;
using CSPSolver.Variable;

namespace CSPSolverTests.Model;

[TestClass]
public class ModelBuilderTests
{
    [TestMethod]
    public void SmallRealVarClaimsTwoStateWordsOnly()
    {
        var sb = new StateBuilder();
        var mb = new ModelBuilder(sb);

        mb.AddRealVar(0, 10, 2);

        // min and max are each held in a single int.
        Assert.AreEqual(2, mb.GetStateSize());
    }

    [TestMethod]
    public void SmallRangeRealVarIsBackedBySmallRealVar()
    {
        var mb = new ModelBuilder(new StateBuilder());

        var v = mb.AddRealVar(0, 10, 2);

        Assert.IsInstanceOfType<SmallRealVar>(v.Variable);
    }

    [TestMethod]
    public void RangeTooWideForIntIsBackedByLongRealVar()
    {
        var mb = new ModelBuilder(new StateBuilder());

        // 1e9 at 3dp needs 1e12 discrete steps, well past int.MaxValue.
        var v = mb.AddRealVar(0, 1e9, 3);

        Assert.IsInstanceOfType<LongRealVar>(v.Variable);
    }

    [TestMethod]
    public void LongRealVarClaimsFourStateWords()
    {
        var sb = new StateBuilder();
        var mb = new ModelBuilder(sb);

        mb.AddRealVar(0, 1e9, 3);

        // min and max are each held in a long, which spans two words.
        Assert.AreEqual(4, mb.GetStateSize());
    }

    [TestMethod]
    public void LongRealVarBoundsSurviveInitialise()
    {
        var sb = new StateBuilder();
        var mb = new ModelBuilder(sb);
        var v = mb.AddRealVar(0, 1e9, 3);
        var model = mb.GetModel();
        var state = new IntState(mb.GetStateSize());

        model.Initialise(state);

        var real = (LongRealVar)v.Variable;
        Assert.AreEqual(0d, real.GetDomainMin(state));
        Assert.AreEqual(1e9, real.GetDomainMax(state));
    }

    [TestMethod]
    public void AdjacentRealVarsDoNotOverlapInState()
    {
        var sb = new StateBuilder();
        var mb = new ModelBuilder(sb);
        var a = mb.AddRealVar(0, 1e9, 3);
        var b = mb.AddRealVar(-1e9, 0, 3);
        var model = mb.GetModel();
        var state = new IntState(mb.GetStateSize());

        model.Initialise(state);

        // Writing through a must not disturb b's slots.
        Assert.AreEqual(-1e9, ((LongRealVar)b.Variable).GetDomainMin(state));
        Assert.AreEqual(0d, ((LongRealVar)b.Variable).GetDomainMax(state));
        Assert.AreEqual(0d, ((LongRealVar)a.Variable).GetDomainMin(state));
        Assert.AreEqual(1e9, ((LongRealVar)a.Variable).GetDomainMax(state));
    }
}