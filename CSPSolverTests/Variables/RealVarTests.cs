using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSPSolver.Variable;
using CSPSolver.common;
using CSPSolver.State;

namespace CSPSolverTests.Variables;

[TestClass]
public class RealVarTests
{
    private static (IState state, RealVar variable) GetVar(double min, double max)
    {
        var sb = new StateBuilder();
        var variable = new RealVar(min, sb.AddDouble(), max, sb.AddDouble());
        var state = sb.GetState();
        variable.Initialise(state);
        return (state, variable);
    }

    [TestMethod]
    public void GetInitialiseTest()
    {
        var (s, v) = GetVar(3.3, 6.6);
        Assert.AreEqual(3.3, v.GetDomainMin(s));
        Assert.AreEqual(6.6, v.GetDomainMax(s));
    }

    [TestMethod]
    public void SetMaxTest()
    {
        var max = 66457.6547457;
        var min = 333.355;
        var (s, v) = GetVar(min, max);

        Assert.IsFalse(v.SetMax(s, max + 10));
        Assert.AreNotEqual(DomainState.Empty, v.GetDomainState(s));
        Assert.AreNotEqual(DomainState.Instantiated, v.GetDomainState(s));

        max -= 100;

        Assert.IsTrue(v.SetMax(s, max));
        Assert.AreEqual(max, v.GetDomainMax(s));
        Assert.AreNotEqual(DomainState.Empty, v.GetDomainState(s));
        Assert.AreNotEqual(DomainState.Instantiated, v.GetDomainState(s));
        Assert.IsFalse(v.SetMax(s, max));
        Assert.AreNotEqual(DomainState.Empty, v.GetDomainState(s));
        Assert.AreNotEqual(DomainState.Instantiated, v.GetDomainState(s));

        Assert.IsTrue(v.SetMax(s, min));
        Assert.AreEqual(min, v.GetDomainMax(s));
        Assert.AreNotEqual(DomainState.Empty, v.GetDomainState(s));
        Assert.AreEqual(DomainState.Instantiated, v.GetDomainState(s));

        Assert.IsTrue(v.SetMax(s, min - 10));
        Assert.AreEqual(DomainState.Empty, v.GetDomainState(s));
    }

    [TestMethod]
    public void SetMinTest()
    {
        var max = 66457.6547457;
        var min = 333.355;
        var (s, v) = GetVar(min, max);

        Assert.IsFalse(v.SetMin(s, min - 10));
        Assert.AreNotEqual(DomainState.Empty, v.GetDomainState(s));
        Assert.AreNotEqual(DomainState.Instantiated, v.GetDomainState(s));

        min += 100;

        Assert.IsTrue(v.SetMin(s, min));
        Assert.AreEqual(min, v.GetDomainMin(s));
        Assert.AreNotEqual(DomainState.Empty, v.GetDomainState(s));
        Assert.AreNotEqual(DomainState.Instantiated, v.GetDomainState(s));
        Assert.IsFalse(v.SetMin(s, min));
        Assert.AreNotEqual(DomainState.Empty, v.GetDomainState(s));
        Assert.AreNotEqual(DomainState.Instantiated, v.GetDomainState(s));

        Assert.IsTrue(v.SetMin(s, max));
        Assert.AreEqual(max, v.GetDomainMin(s));
        Assert.AreNotEqual(DomainState.Empty, v.GetDomainState(s));
        Assert.AreEqual(DomainState.Instantiated, v.GetDomainState(s));

        Assert.IsTrue(v.SetMin(s, max + 10));
        Assert.AreEqual(DomainState.Empty, v.GetDomainState(s));
    }
}