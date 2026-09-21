using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSPSolver.Variable;
using CSPSolver.common;
using CSPSolver.State;

namespace CSPSolverTests.Variables;

[TestClass]
public class LongRealVarTests
{
    private static (IState state, LongRealVar variable) GetVar(double min, double max, int dp)
    {
        var sb = new StateBuilder();
        var variable = new LongRealVar(min, sb.AddLong(), max, sb.AddLong(), dp);
        var state = sb.GetState();
        variable.Initialise(state);
        return (state, variable);
    }

    [TestMethod]
    public void GetInitialiseTest()
    {
        var (s, v) = GetVar(0, 1e9, 3);
        Assert.AreEqual(0d, v.GetDomainMin(s));
        Assert.AreEqual(1e9, v.GetDomainMax(s));
    }

    [TestMethod]
    public void SetMaxTest()
    {
        var max = 1e9;
        var min = 0d;
        var (s, v) = GetVar(min, max, 3);

        Assert.IsFalse(v.SetMax(s, max + 10));
        Assert.IsFalse(v.IsEmpty(s));
        Assert.IsFalse(v.IsInstantiated(s));

        max -= 100;

        Assert.IsTrue(v.SetMax(s, max));
        Assert.AreEqual(max, v.GetDomainMax(s));
        Assert.IsFalse(v.IsEmpty(s));
        Assert.IsFalse(v.IsInstantiated(s));
        Assert.IsFalse(v.SetMax(s, max));
        Assert.IsFalse(v.IsEmpty(s));
        Assert.IsFalse(v.IsInstantiated(s));

        Assert.IsTrue(v.SetMax(s, min));
        Assert.AreEqual(min, v.GetDomainMax(s));
        Assert.IsFalse(v.IsEmpty(s));
        Assert.IsTrue(v.IsInstantiated(s));

        Assert.IsTrue(v.SetMax(s, min - 10));
        Assert.IsTrue(v.IsEmpty(s));
    }

    [TestMethod]
    public void SetMinTest()
    {
        var max = 1e9;
        var min = 0d;
        var (s, v) = GetVar(min, max, 3);

        Assert.IsFalse(v.SetMin(s, min - 10));
        Assert.IsFalse(v.IsEmpty(s));
        Assert.IsFalse(v.IsInstantiated(s));

        min += 100;

        Assert.IsTrue(v.SetMin(s, min));
        Assert.AreEqual(min, v.GetDomainMin(s));
        Assert.IsFalse(v.IsEmpty(s));
        Assert.IsFalse(v.IsInstantiated(s));
        Assert.IsFalse(v.SetMin(s, min));
        Assert.IsFalse(v.IsEmpty(s));
        Assert.IsFalse(v.IsInstantiated(s));

        Assert.IsTrue(v.SetMin(s, max));
        Assert.AreEqual(max, v.GetDomainMin(s));
        Assert.IsFalse(v.IsEmpty(s));
        Assert.IsTrue(v.IsInstantiated(s));

        Assert.IsTrue(v.SetMin(s, max + 10));
        Assert.IsTrue(v.IsEmpty(s));
    }

    [TestMethod]
    public void SetMinRoundsATieOutwardNotToNearest()
    {
        var (s, v) = GetVar(0, 10, 0);

        // 0.5 is exactly midway on a unit grid. Math.Round ties to even, which
        // gives 0 here -- too small for a min. SetMin must round up instead.
        Assert.IsTrue(v.SetMin(s, 0.5));
        Assert.AreEqual(1, v.GetDomainMin(s));
    }

    [TestMethod]
    public void SetMaxRoundsATieOutwardNotToNearest()
    {
        var (s, v) = GetVar(0, 10, 0);

        // 9.5 ties to 10 (the even neighbour) under nearest-rounding -- too large
        // for a max. SetMax must round down instead.
        Assert.IsTrue(v.SetMax(s, 9.5));
        Assert.AreEqual(9, v.GetDomainMax(s));
    }
}
