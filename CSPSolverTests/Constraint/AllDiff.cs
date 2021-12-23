using System;
using System.Linq;

using CSPSolver.common;
using CSPSolver.common.variables;
using CSPSolver.Constraint.AllDiff;
using CSPSolver.Model;
using CSPSolver.State;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSPSolverTests.Constraint
{
    [TestClass]
    public class AllDiff
    {
        public static IState GetState(ModelBuilder mb)
        {
            var model = mb.GetModel();
            var statePool = new StatePool(mb.GetStateSize());
            var initialState = statePool.Empty();
            model.Initialise(initialState);
            return initialState;
        }

        [TestMethod]
        public void ForwardCheckingPropagate()
        {
            var mb = new ModelBuilder();
            var variables = mb.AddIntVarArray(1, 3, 3).Select(v => v.GetVariable() as IIntVar).ToArray();
            var state = GetState(mb);
            var constraint = new ForwardCheckingAllDiff(variables);

            Assert.AreEqual(0, constraint.Propagate(state).Count());

            variables[0].SetMin(state, 2);

            Assert.AreEqual(0, constraint.Propagate(state).Count());

            variables[0].SetMin(state, 3);

            Assert.AreEqual(2, constraint.Propagate(state).Count());

            Assert.AreEqual(
                "{ 3 }, { 1, 2 }, { 1, 2 }", 
                string.Join(", ", variables.Select(v => v.PrettyDomain(state))));

            variables[1].SetMin(state, 2);

            Assert.AreEqual(1, constraint.Propagate(state).Count());

            Assert.AreEqual(
                "{ 3 }, { 2 }, { 1 }",
                string.Join(", ", variables.Select(v => v.PrettyDomain(state))));

            state = GetState(mb);
            variables[0].SetMin(state, 2);
            variables[1].SetMin(state, 2);

            Assert.AreEqual(0, constraint.Propagate(state).Count());

            variables[0].SetMax(state, 2);
            variables[1].SetMax(state, 2);

            Assert.AreNotEqual(0, constraint.Propagate(state).Count());

            Assert.IsTrue(variables.Any(v => v.IsEmpty(state)));
        }

        [TestMethod]
        public void ForwardCheckingIsMet()
        {
            var mb = new ModelBuilder();
            var variables = mb.AddIntVarArray(1, 10, 3).Select(v => v.GetVariable() as IIntVar).ToArray();
            var state = GetState(mb);
            var constraint = new ForwardCheckingAllDiff(variables);

            Assert.IsFalse(constraint.IsMet(state));

            variables[0].SetMin(state, 7);

            Assert.IsFalse(constraint.IsMet(state));

            variables[2].SetMax(state, 3);

            Assert.IsFalse(constraint.IsMet(state));

            variables[1].SetMin(state, 4);
            variables[1].SetMax(state, 6);

            Assert.IsTrue(constraint.IsMet(state));
        }

        [TestMethod]
        public void ForwardCheckingCanBeMet()
        {
            var mb = new ModelBuilder();
            var variables = mb.AddIntVarArray(1, 10, 3).Select(v => v.GetVariable() as IIntVar).ToArray();
            var state = GetState(mb);
            var constraint = new ForwardCheckingAllDiff(variables);

            Assert.IsTrue(constraint.CanBeMet(state));

            variables[0].SetMax(state, 1);

            Assert.IsTrue(constraint.CanBeMet(state));

            variables[1].SetMax(state, 1);

            Assert.IsFalse(constraint.CanBeMet(state));
        }

        [TestMethod]
        public void ForwardCheckingNegativePropagate()
        {
            var mb = new ModelBuilder();
            var variables = mb.AddIntVarArray(1, 3, 3).Select(v => v.GetVariable() as IIntVar).ToArray();
            var state = GetState(mb);
            var constraint = new ForwardCheckingAllDiff(variables);

            Assert.AreEqual(0, constraint.NegativePropagate(state).Count());

            variables[0].SetValue(state, 3);

            Assert.AreEqual(0, constraint.NegativePropagate(state).Count());

            variables[1].SetValue(state, 2);
            variables[2].SetMax(state, 2);

            Assert.AreEqual(1, constraint.NegativePropagate(state).Count());

            Assert.AreEqual(
                "{ 3 }, { 2 }, { 2 }",
                string.Join(", ", variables.Select(v => v.PrettyDomain(state))));
        }
    }
}
