using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSPSolver.common;
using CSPSolver.Constraint.Equal;
using CSPSolver.State;
using CSPSolver.Variable;

namespace CSPSolverTests.Constraint;

[TestClass]
public class EqualRealVarTests
{
    private static (IState state, RealVar v1, RealVar v2) GetVars(double min1, double max1, double min2, double max2)
    {
        var sb = new StateBuilder();
        var v1 = new RealVar(min1, sb.AddDouble(), max1, sb.AddDouble());
        var v2 = new RealVar(min2, sb.AddDouble(), max2, sb.AddDouble());
        var state = sb.GetState();
        v1.Initialise(state);
        v2.Initialise(state);
        return (state, v1, v2);
    }

    [TestMethod]
    public void CanBeMetWhenTheRangesOverlap()
    {
        var (state, v1, v2) = GetVars(1, 3, 0, 10);
        var constraint = new EqualRealVar(v1, v2);

        Assert.IsTrue(constraint.CanBeMet(state));
    }

    [TestMethod]
    public void CanBeMetWhenOneRangeFullyContainsTheOther()
    {
        var (state, v1, v2) = GetVars(2, 2, 0, 10);
        var constraint = new EqualRealVar(v1, v2);

        Assert.IsTrue(constraint.CanBeMet(state));
    }

    [TestMethod]
    public void CannotBeMetWhenTheRangesAreFarApart()
    {
        var (state, v1, v2) = GetVars(1, 3, 100, 200);
        var constraint = new EqualRealVar(v1, v2);

        Assert.IsFalse(constraint.CanBeMet(state));
    }
}
