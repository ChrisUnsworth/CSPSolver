using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSPSolver.common;
using CSPSolver.common.variables;
using CSPSolver.Math.Plus;
using CSPSolver.Model;
using CSPSolver.State;
using CSPSolver.Variable;

namespace CSPSolverTests.Variables;

[TestClass]
public class MakeEmptyTests
{
    private static IState GetState(ModelBuilder mb)
    {
        var model = mb.GetModel();
        var statePool = new StatePool(mb.GetStateSize());
        var state = statePool.Empty();
        model.Initialise(state);
        return state;
    }

    [TestMethod]
    public void EmptiesALeafDomain()
    {
        var mb = new ModelBuilder();
        var x = mb.AddIntDomainVar(1, 5);
        var state = GetState(mb);

        Assert.IsFalse(x.Variable.IsEmpty(state));
        Assert.IsTrue(x.Variable.MakeEmpty(state));
        Assert.IsTrue(x.Variable.IsEmpty(state));
    }

    [TestMethod]
    public void MakeEmptyIsIdempotent()
    {
        var mb = new ModelBuilder();
        var x = mb.AddIntDomainVar(1, 5);
        var state = GetState(mb);

        x.Variable.MakeEmpty(state);

        Assert.IsFalse(x.Variable.MakeEmpty(state));
    }

    [TestMethod]
    public void EmptyingACompoundVarEmptiesAChild()
    {
        var mb = new ModelBuilder();
        var x = mb.AddIntDomainVar(1, 5);
        var y = mb.AddIntDomainVar(1, 5);
        var state = GetState(mb);
        IIntVar plus = new PlusIntVar(x.Variable, y.Variable);

        Assert.IsTrue(plus.MakeEmpty(state));
        Assert.IsTrue(plus.IsEmpty(state));
        Assert.IsTrue(x.Variable.IsEmpty(state) || y.Variable.IsEmpty(state));
    }

    [TestMethod]
    public void ConstantsDoNotEmpty()
    {
        var mb = new ModelBuilder();
        var state = GetState(mb);
        IIntVar five = new IntConstVar(5);

        Assert.IsFalse(five.MakeEmpty(state));
        Assert.IsFalse(five.IsEmpty(state));
    }

    [TestMethod]
    public void ModelVarDelegatesToItsUnderlyingVariable()
    {
        var mb = new ModelBuilder();
        var x = mb.AddIntDomainVar(1, 5);
        var state = GetState(mb);

        Assert.IsTrue(x.MakeEmpty(state));
        Assert.IsTrue(x.Variable.IsEmpty(state));
    }
}
