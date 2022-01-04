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

        public static string Pretty(IIntVar[] variables, IState state) =>
            string.Join(", ", variables.Select(v => v.PrettyDomain(state)));

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

            Assert.AreEqual("{ 3 }, { 1, 2 }, { 1, 2 }", Pretty(variables, state));

            variables[1].SetMin(state, 2);

            Assert.AreEqual(1, constraint.Propagate(state).Count());

            Assert.AreEqual("{ 3 }, { 2 }, { 1 }", Pretty(variables, state));

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

            Assert.AreEqual("{ 3 }, { 2 }, { 2 }", Pretty(variables, state));

            state = GetState(mb);
            variables[0].SetValue(state, 1);
            variables[1].SetValue(state, 2);
            variables[2].SetMin(state, 3);

            Assert.AreNotEqual(0, constraint.NegativePropagate(state).Count());

            Assert.IsTrue(variables.Any(v => v.IsEmpty(state)));
        }

        [TestMethod]
        public void AllDiffMixedSmallIntDomainPropagate1()
        {
            var mb = new ModelBuilder();
            var variables = mb.AddIntVarArray(1, 3, 3).Select(v => v.GetVariable() as ISmallIntDomainVar).ToArray();
            var state = GetState(mb);
            var constraint = new AllDiffMixedSmallIntDomain(variables);

            Assert.AreEqual(0, constraint.Propagate(state).Count());

            variables[0].SetMin(state, 2);

            Assert.AreEqual(0, constraint.Propagate(state).Count());

            variables[0].SetMin(state, 3);

            Assert.AreEqual(2, constraint.Propagate(state).Count());

            Assert.AreEqual("{ 3 }, { 1, 2 }, { 1, 2 }", Pretty(variables, state));

            variables[1].SetMin(state, 2);

            Assert.AreEqual(1, constraint.Propagate(state).Count());

            Assert.AreEqual("{ 3 }, { 2 }, { 1 }", Pretty(variables, state));

            state = GetState(mb);
            variables[0].SetMin(state, 2);
            variables[1].SetMin(state, 2);

            Assert.AreEqual(1, constraint.Propagate(state).Count());

            Assert.AreEqual("{ 2, 3 }, { 2, 3 }, { 1 }", Pretty(variables, state));

            variables[0].SetMax(state, 2);
            variables[1].SetMax(state, 2);

            Assert.AreNotEqual(0, constraint.Propagate(state).Count());

            Assert.IsTrue(variables.Any(v => v.IsEmpty(state)));
        }

        [TestMethod]
        public void AllDiffMixedSmallIntDomainPropagate2()
        {
            var mb = new ModelBuilder();
            var x = mb.AddIntDomainVar(1, 5).GetVariable() as ISmallIntDomainVar;
            var y = mb.AddIntDomainVar(2, 6).GetVariable() as ISmallIntDomainVar;
            var z = mb.AddIntDomainVar(3, 7).GetVariable() as ISmallIntDomainVar;
            var state = GetState(mb);
            var variables = new[] { x, y, z };
            var constraint = new AllDiffMixedSmallIntDomain(variables);

            Assert.AreEqual(0, constraint.Propagate(state).Count());

            x.SetValue(state, 3);

            Assert.AreEqual(2, constraint.Propagate(state).Count());

            Assert.AreEqual("{ 3 }, { 2, 4, 5, 6 }, { 4, 5, 6, 7 }", Pretty(variables, state));

            y.SetValue(state, 5);

            Assert.AreEqual(1, constraint.Propagate(state).Count());

            Assert.AreEqual("{ 3 }, { 5 }, { 4, 6, 7 }", Pretty(variables, state));

            state = GetState(mb);
            x.SetMin(state, 3);
            x.SetMax(state, 4);
            y.SetMin(state, 3);
            y.SetMax(state, 4);

            Assert.AreEqual(1, constraint.Propagate(state).Count());

            Assert.AreEqual("{ 3, 4 }, { 3, 4 }, { 5, 6, 7 }", Pretty(variables, state));

            x.SetValue(state, 3);
            y.SetValue(state, 3);

            Assert.AreNotEqual(0, constraint.Propagate(state).Count());

            Assert.IsTrue(variables.Any(v => v.IsEmpty(state)));
        }

        [TestMethod]
        public void AllDiffMixedSmallIntDomainIsMet()
        {
            var mb = new ModelBuilder();
            var x = mb.AddIntDomainVar(1, 5).GetVariable() as ISmallIntDomainVar;
            var y = mb.AddIntDomainVar(2, 6).GetVariable() as ISmallIntDomainVar;
            var z = mb.AddIntDomainVar(3, 7).GetVariable() as ISmallIntDomainVar;
            var state = GetState(mb);
            var variables = new[] { x, y, z };
            var constraint = new AllDiffMixedSmallIntDomain(variables);

            Assert.IsFalse(constraint.IsMet(state));

            x.SetMax(state, 3);

            Assert.IsFalse(constraint.IsMet(state));

            y.SetMin(state, 4);
            y.SetMax(state, 5);

            Assert.IsFalse(constraint.IsMet(state));

            z.SetMin(state, 6);

            Assert.IsTrue(constraint.IsMet(state));
        }

        [TestMethod]
        public void AllDiffMixedSmallIntDomainCanBeMet()
        {
            var mb = new ModelBuilder();
            var x = mb.AddIntDomainVar(1, 5).GetVariable() as ISmallIntDomainVar;
            var y = mb.AddIntDomainVar(2, 6).GetVariable() as ISmallIntDomainVar;
            var z = mb.AddIntDomainVar(3, 7).GetVariable() as ISmallIntDomainVar;
            var state = GetState(mb);
            var variables = new[] { x, y, z };
            var constraint = new AllDiffMixedSmallIntDomain(variables);

            Assert.IsTrue(constraint.CanBeMet(state));

            x.SetMin(state, 3);
            x.SetMax(state, 4);

            Assert.IsTrue(constraint.CanBeMet(state));

            y.SetMin(state, 3);
            y.SetMax(state, 4);

            Assert.IsTrue(constraint.CanBeMet(state));

            z.SetMin(state, 3);
            z.SetMax(state, 4);

            Assert.IsFalse(constraint.CanBeMet(state));
        }

        [TestMethod]
        public void AllDiffMixedSmallIntDomainNegativePropagate1()
        {
            var mb = new ModelBuilder();
            var variables = mb.AddIntVarArray(1, 3, 3).Select(v => v.GetVariable() as ISmallIntDomainVar).ToArray();
            var state = GetState(mb);
            var constraint = new AllDiffMixedSmallIntDomain(variables);

            Assert.AreEqual(0, constraint.NegativePropagate(state).Count());

            variables[0].SetValue(state, 3);

            Assert.AreEqual(0, constraint.NegativePropagate(state).Count());

            variables[1].SetValue(state, 2);
            variables[2].SetMax(state, 2);

            Assert.AreEqual(1, constraint.NegativePropagate(state).Count());

            Assert.AreEqual("{ 3 }, { 2 }, { 2 }", Pretty(variables, state));
        }

        [TestMethod]
        public void AllDiffMixedSmallIntDomainNegativePropagate2()
        {
            var mb = new ModelBuilder();
            var x = mb.AddIntDomainVar(1, 5).GetVariable() as ISmallIntDomainVar;
            var y = mb.AddIntDomainVar(2, 6).GetVariable() as ISmallIntDomainVar;
            var z = mb.AddIntDomainVar(3, 7).GetVariable() as ISmallIntDomainVar;
            var state = GetState(mb);
            var variables = new[] { x, y, z };
            var constraint = new AllDiffMixedSmallIntDomain(variables);

            Assert.AreEqual(0, constraint.NegativePropagate(state).Count());

            x.SetValue(state, 1);

            Assert.AreEqual(2, constraint.NegativePropagate(state).Count());

            Assert.AreEqual("{ 1 }, { 3, 4, 5, 6 }, { 3, 4, 5, 6 }", Pretty(variables, state));

            y.RemoveValue(state, 4);

            Assert.AreEqual(1, constraint.NegativePropagate(state).Count());

            Assert.AreEqual("{ 1 }, { 3, 5, 6 }, { 3, 5, 6 }", Pretty(variables, state));

            z.SetValue(state, 3);

            Assert.AreEqual(1, constraint.NegativePropagate(state).Count());

            Assert.AreEqual("{ 1 }, { 3 }, { 3 }", Pretty(variables, state));

            state = GetState(mb);
            x.SetValue(state, 1);
            y.SetValue(state, 2);
            z.SetMin(state, 3);

            Assert.AreNotEqual(0, constraint.NegativePropagate(state).Count());

            Assert.IsTrue(variables.Any(v => v.IsEmpty(state)));
        }

        [TestMethod]
        public void AllDiffLongDomainPropagate1()
        {
            var mb = new ModelBuilder();
            var variables = mb.AddIntVarArray(1, 50, 3).Select(v => v.GetVariable() as ILongDomainVar).ToArray();
            var state = GetState(mb);
            var constraint = new AllDiffLongDomain(variables);
            foreach (var v in variables) { v.SetMax(state, 3); }

            Assert.AreEqual(0, constraint.Propagate(state).Count());

            variables[0].SetMin(state, 2);

            Assert.AreEqual(0, constraint.Propagate(state).Count());

            variables[0].SetMin(state, 3);

            Assert.AreEqual(2, constraint.Propagate(state).Count());

            Assert.AreEqual("{ 3 }, { 1, 2 }, { 1, 2 }", Pretty(variables, state));

            variables[1].SetMin(state, 2);

            Assert.AreEqual(1, constraint.Propagate(state).Count());

            Assert.AreEqual("{ 3 }, { 2 }, { 1 }", Pretty(variables, state));

            state = GetState(mb);
            variables[0].SetMin(state, 2);
            variables[1].SetMin(state, 2);
            foreach (var v in variables) { v.SetMax(state, 3); }

            Assert.AreEqual(1, constraint.Propagate(state).Count());

            Assert.AreEqual("{ 2, 3 }, { 2, 3 }, { 1 }", Pretty(variables, state));

            variables[0].SetMax(state, 2);
            variables[1].SetMax(state, 2);

            Assert.AreNotEqual(0, constraint.Propagate(state).Count());

            Assert.IsTrue(variables.Any(v => v.IsEmpty(state)));
        }

        [TestMethod]
        public void AllDiffLongDomainPropagate2()
        {
            var mb = new ModelBuilder();
            var x = mb.AddIntDomainVar(1, 50).GetVariable() as ILongDomainVar;
            var y = mb.AddIntDomainVar(2, 50).GetVariable() as ILongDomainVar;
            var z = mb.AddIntDomainVar(3, 50).GetVariable() as ILongDomainVar;
            var state = GetState(mb);
            x.SetMax(state, 5);
            y.SetMax(state, 6);
            z.SetMax(state, 7);
            var variables = new[] { x, y, z };
            var constraint = new AllDiffLongDomain(variables);

            Assert.AreEqual(0, constraint.Propagate(state).Count());

            x.SetValue(state, 3);

            Assert.AreEqual(2, constraint.Propagate(state).Count());

            Assert.AreEqual("{ 3 }, { 2, 4, 5, 6 }, { 4, 5, 6, 7 }", Pretty(variables, state));

            y.SetValue(state, 5);

            Assert.AreEqual(1, constraint.Propagate(state).Count());

            Assert.AreEqual("{ 3 }, { 5 }, { 4, 6, 7 }", Pretty(variables, state));

            state = GetState(mb);
            x.SetMax(state, 5);
            y.SetMax(state, 6);
            z.SetMax(state, 7);

            x.SetMin(state, 3);
            x.SetMax(state, 4);
            y.SetMin(state, 3);
            y.SetMax(state, 4);

            Assert.AreEqual(1, constraint.Propagate(state).Count());

            Assert.AreEqual("{ 3, 4 }, { 3, 4 }, { 5, 6, 7 }", Pretty(variables, state));

            x.SetValue(state, 3);
            y.SetValue(state, 3);

            Assert.AreNotEqual(0, constraint.Propagate(state).Count());

            Assert.IsTrue(variables.Any(v => v.IsEmpty(state)));
        }

        [TestMethod]
        public void AllDiffLongDomainIsMet()
        {
            var mb = new ModelBuilder();
            var x = mb.AddIntDomainVar(1, 50).GetVariable() as ILongDomainVar;
            var y = mb.AddIntDomainVar(2, 50).GetVariable() as ILongDomainVar;
            var z = mb.AddIntDomainVar(3, 50).GetVariable() as ILongDomainVar;
            var state = GetState(mb);
            x.SetMax(state, 5);
            y.SetMax(state, 6);
            z.SetMax(state, 7);
            var variables = new[] { x, y, z };
            var constraint = new AllDiffLongDomain(variables);

            Assert.IsFalse(constraint.IsMet(state));

            x.SetMax(state, 3);

            Assert.IsFalse(constraint.IsMet(state));

            y.SetMin(state, 4);
            y.SetMax(state, 5);

            Assert.IsFalse(constraint.IsMet(state));

            z.SetMin(state, 6);

            Assert.IsTrue(constraint.IsMet(state));
        }

        [TestMethod]
        public void AllDiffLongDomainCanBeMet()
        {
            var mb = new ModelBuilder();
            var x = mb.AddIntDomainVar(1, 50).GetVariable() as ILongDomainVar;
            var y = mb.AddIntDomainVar(2, 50).GetVariable() as ILongDomainVar;
            var z = mb.AddIntDomainVar(3, 50).GetVariable() as ILongDomainVar;
            var state = GetState(mb);
            x.SetMax(state, 5);
            y.SetMax(state, 6);
            z.SetMax(state, 7);
            var variables = new[] { x, y, z };
            var constraint = new AllDiffLongDomain(variables);

            Assert.IsTrue(constraint.CanBeMet(state));

            x.SetMin(state, 3);
            x.SetMax(state, 4);

            Assert.IsTrue(constraint.CanBeMet(state));

            y.SetMin(state, 3);
            y.SetMax(state, 4);

            Assert.IsTrue(constraint.CanBeMet(state));

            z.SetMin(state, 3);
            z.SetMax(state, 4);

            Assert.IsFalse(constraint.CanBeMet(state));
        }

        [TestMethod]
        public void AllDiffLongDomainNegativePropagate1()
        {
            var mb = new ModelBuilder();
            var variables = mb.AddIntVarArray(1, 50, 3).Select(v => v.GetVariable() as ILongDomainVar).ToArray();
            var state = GetState(mb);
            var constraint = new AllDiffLongDomain(variables);
            foreach (var v in variables) { v.SetMax(state, 3); }

            Assert.AreEqual(0, constraint.NegativePropagate(state).Count());

            variables[0].SetValue(state, 3);

            Assert.AreEqual(0, constraint.NegativePropagate(state).Count());

            variables[1].SetValue(state, 2);
            variables[2].SetMax(state, 2);

            Assert.AreEqual(1, constraint.NegativePropagate(state).Count());

            Assert.AreEqual("{ 3 }, { 2 }, { 2 }", Pretty(variables, state));
        }

        [TestMethod]
        public void AllDiffLongDomainNegativePropagate2()
        {
            var mb = new ModelBuilder();
            var x = mb.AddIntDomainVar(1, 50).GetVariable() as ILongDomainVar;
            var y = mb.AddIntDomainVar(2, 50).GetVariable() as ILongDomainVar;
            var z = mb.AddIntDomainVar(3, 50).GetVariable() as ILongDomainVar;
            var state = GetState(mb);
            x.SetMax(state, 5);
            y.SetMax(state, 6);
            z.SetMax(state, 7);
            var variables = new[] { x, y, z };
            var constraint = new AllDiffLongDomain(variables);

            Assert.AreEqual(0, constraint.NegativePropagate(state).Count());

            x.SetValue(state, 1);

            Assert.AreEqual(2, constraint.NegativePropagate(state).Count());

            Assert.AreEqual("{ 1 }, { 3, 4, 5, 6 }, { 3, 4, 5, 6 }", Pretty(variables, state));

            y.RemoveValue(state, 4);

            Assert.AreEqual(1, constraint.NegativePropagate(state).Count());

            Assert.AreEqual("{ 1 }, { 3, 5, 6 }, { 3, 5, 6 }", Pretty(variables, state));

            z.SetValue(state, 3);

            Assert.AreEqual(1, constraint.NegativePropagate(state).Count());

            Assert.AreEqual("{ 1 }, { 3 }, { 3 }", Pretty(variables, state));

            state = GetState(mb);
            x.SetMax(state, 5);
            y.SetMax(state, 6);
            z.SetMax(state, 7);

            x.SetValue(state, 1);
            y.SetValue(state, 2);
            z.SetMin(state, 3);

            Assert.AreNotEqual(0, constraint.NegativePropagate(state).Count());

            Assert.IsTrue(variables.Any(v => v.IsEmpty(state)));
        }
    }
}
