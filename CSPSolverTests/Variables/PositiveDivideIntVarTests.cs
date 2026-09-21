using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSPSolver.common;
using CSPSolver.State;
using CSPSolver.Variable;
using CSPSolver.Math.Divide;

namespace CSPSolverTests.Variables;

[TestClass]
public class PositiveDivideIntVarTests
{
    private static (IState state, IntSmallDomainVar v1, IntSmallDomainVar v2) GetVar(int min1, int size1, int min2, int size2)
    {
        var sb = new StateBuilder();
        var variable1 = new IntSmallDomainVar(min1, size1, sb.AddDomain(size1));
        var variable2 = new IntSmallDomainVar(min2, size2, sb.AddDomain(size2));
        var state = sb.GetState();
        variable1.Initialise(state);
        variable2.Initialise(state);
        return (state, variable1, variable2);
    }

    [TestMethod]
    public void MaxTest()
    {
        var (state, v1, v2) = GetVar(1, 10, 1, 10);
        var divide = new PositiveDivideIntVar(v1, v2);

        Assert.AreEqual(10, divide.GetDomainMax(state));

        v1.SetMax(state, 6);
        Assert.AreEqual(6, divide.GetDomainMax(state));

        v2.SetMin(state, 2);
        Assert.AreEqual(3, divide.GetDomainMax(state));
    }

    [TestMethod]
    public void MinTest()
    {
        var (state, v1, v2) = GetVar(1, 10, 1, 10);
        var divide = new PositiveDivideIntVar(v1, v2);

        Assert.AreEqual(0, divide.GetDomainMin(state));

        v1.SetMin(state, 6);
        Assert.AreEqual(0, divide.GetDomainMin(state));

        v2.SetMax(state, 3);
        Assert.AreEqual(2, divide.GetDomainMin(state));
    }

    [TestMethod]
    public void SetMaxTest()
    {
        var (state, v1, v2) = GetVar(7, 4, 2, 4);
        var divide = new PositiveDivideIntVar(v1, v2);

        Assert.IsFalse(divide.SetMax(state, 3));
        Assert.AreEqual(10, v1.GetDomainMax(state));
        Assert.AreEqual(2, v2.GetDomainMin(state));
        Assert.IsFalse(divide.IsEmpty(state));

        Assert.IsTrue(divide.SetMax(state, 1));
        Assert.AreEqual(9, v1.GetDomainMax(state));
        Assert.AreEqual(4, v2.GetDomainMin(state));
        Assert.IsFalse(divide.IsEmpty(state));

        Assert.IsFalse(divide.SetMax(state, 1));
        Assert.AreEqual(9, v1.GetDomainMax(state));
        Assert.AreEqual(4, v2.GetDomainMin(state));
        Assert.IsFalse(divide.IsEmpty(state));

        Assert.IsTrue(divide.SetMax(state, 0));
        Assert.IsTrue(divide.IsEmpty(state));
    }

    [TestMethod]
    public void SetMinTest()
    {
        var (state, v1, v2) = GetVar(1, 10, 1, 10);
        var divide = new PositiveDivideIntVar(v1, v2);

        Assert.IsFalse(divide.SetMin(state, 1));
        Assert.AreEqual(1, v1.GetDomainMin(state));
        Assert.AreEqual(10, v2.GetDomainMax(state));
        Assert.IsFalse(divide.IsEmpty(state));

        Assert.IsTrue(divide.SetMin(state, 2));
        Assert.AreEqual(2, v1.GetDomainMin(state));
        Assert.AreEqual(5, v2.GetDomainMax(state));
        Assert.IsFalse(divide.IsEmpty(state));

        Assert.IsTrue(divide.SetMin(state, 5));
        Assert.AreEqual(5, v1.GetDomainMin(state));
        Assert.AreEqual(2, v2.GetDomainMax(state));
        Assert.IsFalse(divide.IsEmpty(state));

        Assert.IsFalse(divide.SetMin(state, 5));
        Assert.AreEqual(5, v1.GetDomainMin(state));
        Assert.AreEqual(2, v2.GetDomainMax(state));
        Assert.IsFalse(divide.IsEmpty(state));

        Assert.IsTrue(divide.SetMin(state, 10));
        Assert.AreEqual(10, v1.GetDomainMin(state));
        Assert.AreEqual(1, v2.GetDomainMax(state));
        Assert.IsFalse(divide.IsEmpty(state));

        Assert.IsTrue(divide.SetMin(state, 11));
        Assert.IsTrue(divide.IsEmpty(state));
    }
    [TestMethod]
    public void DenominatorContainingZeroIsRejected()
    {
        var (_, v1, v2) = GetVar(1, 10, 0, 4); // denominator is 0 .. 3

        // A domain only shrinks, so barring zero once at construction keeps it
        // out for the whole search and leaves the bounds free of guards.
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new PositiveDivideIntVar(v1, v2));
    }

    [TestMethod]
    public void DenominatorOfOnlyZeroIsRejected()
    {
        var (_, v1, v2) = GetVar(1, 10, 0, 1); // denominator is exactly { 0 }

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new PositiveDivideIntVar(v1, v2));
    }

    [TestMethod]
    public void NegativeUpperBoundEmptiesTheNumerator()
    {
        var (state, v1, v2) = GetVar(1, 10, 1, 3);
        var divide = new PositiveDivideIntVar(v1, v2);

        // Both domains are non negative, so no negative quotient is reachable.
        // -1 is the case that used to divide by (max + 1).
        divide.SetMax(state, -1);

        Assert.IsTrue(v1.IsEmpty(state));
    }

    [TestMethod]
    public void UpperBoundOfZeroForcesNumeratorUnderDenominator()
    {
        var (state, v1, v2) = GetVar(1, 10, 1, 3); // v1 1..10, v2 1..3

        var divide = new PositiveDivideIntVar(v1, v2);

        divide.SetMax(state, 0); // quotient 0 means v1 < v2

        Assert.AreEqual(2, v1.GetDomainMax(state));
        Assert.IsFalse(v1.IsEmpty(state));
    }
}