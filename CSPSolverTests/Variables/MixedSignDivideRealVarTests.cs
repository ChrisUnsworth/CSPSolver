using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSPSolver.common;
using CSPSolver.State;
using CSPSolver.Variable;
using CSPSolver.Math.Divide;

namespace CSPSolverTests.Variables;

[TestClass]
public class MixedSignDivideRealVarTests
{
    private static (IState state, RealVar v1, RealVar v2) GetVar(double min1, double max1, double min2, double max2)
    {
        var sb = new StateBuilder();
        var variable1 = new RealVar(min1, sb.AddDouble(), max1, sb.AddDouble());
        var variable2 = new RealVar(min2, sb.AddDouble(), max2, sb.AddDouble());
        var state = sb.GetState();
        variable1.Initialise(state);
        variable2.Initialise(state);
        return (state, variable1, variable2);
    }

    [TestMethod]
    public void BoundsSpanBothDenominatorSignBlocks()
    {
        var (state, v1, v2) = GetVar(2, 4, -3, 5);
        var divide = new MixedSignDivideRealVar(v1, v2);

        // Corners: 2/-3, 2/5, 4/-3, 4/5 -- min is 4/-3, max is 2/-eps (huge negative
        // y just above the exclusion band), so instead check the achievable range
        // contains both the positive-block and negative-block extremes.
        Assert.IsTrue(divide.GetDomainMin(state) <= -4.0 / 3);
        Assert.IsTrue(divide.GetDomainMax(state) >= 4.0 / 5);
    }

    [TestMethod]
    public void SetMaxNarrowsBothOperandsForAPositiveDenominator()
    {
        // Corner-consistent bounds only prune where the corners themselves are
        // inconsistent with the target, so the window has to be picked to actually
        // force a change rather than merely contain some now-infeasible points.
        var (state, v1, v2) = GetVar(2, 10, 1, 3);
        var divide = new MixedSignDivideRealVar(v1, v2);

        Assert.IsTrue(divide.SetMax(state, 1));
        Assert.AreEqual(3.0, v1.GetDomainMax(state));
        Assert.AreEqual(2.0, v2.GetDomainMin(state));
        Assert.AreEqual(3.0, v2.GetDomainMax(state));
    }

    [TestMethod]
    public void SetMinNarrowsBothOperandsForANegativeDenominator()
    {
        var (state, v1, v2) = GetVar(2, 20, -5, -1);
        var divide = new MixedSignDivideRealVar(v1, v2);

        Assert.IsTrue(divide.SetMin(state, -2));
        Assert.AreEqual(10.0, v1.GetDomainMax(state));
        Assert.AreEqual(-5.0, v2.GetDomainMin(state));
        Assert.AreEqual(-1.0, v2.GetDomainMax(state));
    }

    [TestMethod]
    public void ExcludesZeroFromTheDenominator()
    {
        // The exclusion only fires once an endpoint actually reaches zero -- a
        // domain that merely contains zero in its interior, like [-2, 2], is left
        // alone until search narrows an endpoint there, same as the int version.
        var (state, v1, v2) = GetVar(1, 5, -2, 2);
        var divide = new MixedSignDivideRealVar(v1, v2);

        v2.SetMax(state, 0);
        divide.SetMax(state, 100);

        Assert.IsTrue(v2.GetDomainMax(state) < 0);
    }

    [TestMethod]
    public void DoesNotNarrowTheDenominatorWhenBothOperandsCanBeZero()
    {
        // x can be 0 and q (the target bound) can be 0: x = 0 = q*y is satisfied
        // by every y, so the constraint carries no information about y here.
        var (state, v1, v2) = GetVar(-3, 5, -8, 8);
        var divide = new MixedSignDivideRealVar(v1, v2);

        divide.SetMax(state, 4);

        Assert.AreEqual(-8.0, v2.GetDomainMin(state));
        Assert.AreEqual(8.0, v2.GetDomainMax(state));
    }

    [TestMethod]
    public void EmptiesTheNumeratorWhenNoQuotientCanReachTheBound()
    {
        var (state, v1, v2) = GetVar(1, 5, 1, 3);
        var divide = new MixedSignDivideRealVar(v1, v2);

        // The achievable quotient range is [1/3, 5], so asking for max < 1/3 is unreachable.
        Assert.IsTrue(divide.SetMax(state, 0));
        Assert.IsTrue(divide.IsEmpty(state));
    }
}
