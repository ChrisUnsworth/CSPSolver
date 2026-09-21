using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSPSolver.Model;

namespace CSPSolverTests.Model;

[TestClass]
public class ModelVarEqualityTests
{
    [TestMethod]
    public void RealVarsOverTheSameVariableAreEqual()
    {
        var mb = new ModelBuilder();
        var x = mb.AddRealVar(0, 10, 2);
        var same = new ModelRealVar { Variable = x.Variable };

        Assert.IsTrue(x.Equals(same));
        Assert.AreEqual(x.GetHashCode(), same.GetHashCode());
    }

    [TestMethod]
    public void RealVarsOverDifferentVariablesAreNotEqual()
    {
        var mb = new ModelBuilder();
        var x = mb.AddRealVar(0, 10, 2);
        var y = mb.AddRealVar(0, 10, 2);

        Assert.IsFalse(x.Equals(y));
    }

    [TestMethod]
    public void RealVarIsNotEqualToNullOrAnotherType()
    {
        var mb = new ModelBuilder();
        var x = mb.AddRealVar(0, 10, 2);

        Assert.IsFalse(x.Equals(null));
        Assert.IsFalse(x.Equals("not a variable"));
    }

    [TestMethod]
    public void IntVarsOverTheSameVariableAreEqual()
    {
        var mb = new ModelBuilder();
        var x = mb.AddIntDomainVar(1, 5);
        var same = new ModelIntVar { Variable = x.Variable };

        Assert.IsTrue(x.Equals(same));
        Assert.AreEqual(x.GetHashCode(), same.GetHashCode());
    }

    [TestMethod]
    public void EqualityOperatorBuildsAConstraintRatherThanComparing()
    {
        var mb = new ModelBuilder();
        var x = mb.AddRealVar(0, 10, 2);
        var y = mb.AddRealVar(0, 10, 2);

        // This is the deliberate divergence: == asks the solver to make the two
        // equal, while Equals reports whether they already denote the same variable.
        Assert.IsInstanceOfType<ModelConstraint>(x == y);
        Assert.IsFalse(x.Equals(y));
    }

    [TestMethod]
    public void BoolVarsOverTheSameVariableAreEqual()
    {
        var mb = new ModelBuilder();
        var x = mb.AddBoolVar();
        var same = new ModelBoolVar { Variable = x.Variable };

        Assert.IsTrue(x.Equals(same));
        Assert.AreEqual(x.GetHashCode(), same.GetHashCode());
    }

    [TestMethod]
    public void BoolVarsOverDifferentVariablesAreNotEqual()
    {
        var mb = new ModelBuilder();
        var x = mb.AddBoolVar();
        var y = mb.AddBoolVar();

        Assert.IsFalse(x.Equals(y));
    }

    [TestMethod]
    public void BoolVarIsNotEqualToNullOrAnotherType()
    {
        var mb = new ModelBuilder();
        var x = mb.AddBoolVar();

        Assert.IsFalse(x.Equals(null));
        Assert.IsFalse(x.Equals("not a variable"));
    }

    [TestMethod]
    public void BoolVarEqualityOperatorBuildsAConstraintRatherThanComparing()
    {
        var mb = new ModelBuilder();
        var x = mb.AddBoolVar();
        var y = mb.AddBoolVar();

        Assert.IsInstanceOfType<ModelConstraint>(x == y);
        Assert.IsFalse(x.Equals(y));
    }
}