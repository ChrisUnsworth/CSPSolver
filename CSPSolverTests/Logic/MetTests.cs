using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSPSolver.common;
using CSPSolver.common.variables;
using CSPSolver.Constraint.AllDiff;
using CSPSolver.Constraint.Equal;
using CSPSolver.Constraint.Inequality;
using CSPSolver.Model;
using CSPSolver.State;
using CSPSolver.Variable;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSPSolverTests.Logic
{
    [TestClass]
    public class MetTests
    {
        private IState Initialise(StateBuilder sb, params IVariable[] vars)
        {
            var state = sb.GetState();
            foreach (var v in vars) v.Initialise(state);
            return state;
        }

        private IntSmallDomainVar GetIntVar(int min, int max, StateBuilder sb) =>
            new(min, max - min + 1, sb.AddDomain(max - min + 1));

        [TestMethod]
        public void EqualIntVatIsMetTest()
        {
            var sb = new StateBuilder();

            var v1 = GetIntVar(2, 4, sb);
            var v2 = GetIntVar(2, 4, sb);
            var v3 = GetIntVar(2, 2, sb);
            var v4 = GetIntVar(2, 2, sb);
            var v5 = GetIntVar(7, 9, sb);
            var v6 = GetIntVar(2, 4, sb);

            var con1 = new EqualIntVar(v1, v2);
            var con2 = new EqualIntVar(v3, v4);
            var con3 = new EqualIntVar(v5, v6);

            var state = Initialise(sb, v1, v2, v3, v4, v5, v6);

            Assert.IsTrue(con1.CanBeMet(state));
            Assert.IsFalse(con1.IsMet(state));

            Assert.IsTrue(con2.CanBeMet(state));
            Assert.IsTrue(con2.IsMet(state));

            Assert.IsFalse(con3.CanBeMet(state));
            Assert.IsFalse(con3.IsMet(state));
        }

        [TestMethod]
        public void EqualIntDomainConstIsMetTest()
        {
            var sb = new StateBuilder();

            var v1 = GetIntVar(2, 4, sb);
            var v2 = GetIntVar(2, 2, sb);

            var con1 = new EqualIntDomainConst(v1, 3);
            var con2 = new EqualIntDomainConst(v1, 8);
            var con3 = new EqualIntDomainConst(v2, 8);
            var con4 = new EqualIntDomainConst(v2, 2);

            var state = Initialise(sb, v1, v2);

            Assert.IsTrue(con1.CanBeMet(state));
            Assert.IsFalse(con1.IsMet(state));

            Assert.IsFalse(con2.CanBeMet(state));
            Assert.IsFalse(con2.IsMet(state));

            Assert.IsFalse(con3.CanBeMet(state));
            Assert.IsFalse(con3.IsMet(state));

            Assert.IsTrue(con4.CanBeMet(state));
            Assert.IsTrue(con4.IsMet(state));
        }

        [TestMethod]
        public void NotEqualIntDomainConstIsMetTest()
        {
            var sb = new StateBuilder();

            var v1 = GetIntVar(2, 4, sb);
            var v2 = GetIntVar(2, 2, sb);

            var con1 = new NotEqualIntDomainConst(v1, 3);
            var con2 = new NotEqualIntDomainConst(v1, 8);
            var con3 = new NotEqualIntDomainConst(v2, 8);
            var con4 = new NotEqualIntDomainConst(v2, 2);

            var state = Initialise(sb, v1, v2);

            Assert.IsTrue(con1.CanBeMet(state));
            Assert.IsFalse(con1.IsMet(state));

            Assert.IsTrue(con2.CanBeMet(state));
            Assert.IsTrue(con2.IsMet(state));

            Assert.IsTrue(con3.CanBeMet(state));
            Assert.IsTrue(con3.IsMet(state));

            Assert.IsFalse(con4.CanBeMet(state));
            Assert.IsFalse(con4.IsMet(state));
        }

        [TestMethod]
        public void EqualIntDomainSameMinMetTest()
        {
            var sb = new StateBuilder();

            var v1 = GetIntVar(2, 4, sb);
            var v2 = GetIntVar(2, 4, sb);
            var v3 = GetIntVar(2, 4, sb);
            var v4 = GetIntVar(2, 4, sb);

            var con1 = new EqualIntDomainSameMin(v1, v2);
            var con2 = new EqualIntDomainSameMin(v3, v4);

            var state = Initialise(sb, v1, v2, v3, v4);

            Assert.IsTrue(con1.CanBeMet(state));
            Assert.IsFalse(con1.IsMet(state));

            v1.RemoveValue(state, 3);
            v2.SetValue(state, 3);

            Assert.IsFalse(con1.CanBeMet(state));
            Assert.IsFalse(con1.IsMet(state));

            Assert.IsTrue(con2.CanBeMet(state));
            Assert.IsFalse(con2.IsMet(state));

            v3.SetValue(state, 3);
            v4.SetValue(state, 3);

            Assert.IsTrue(con2.CanBeMet(state));
            Assert.IsTrue(con2.IsMet(state));
        }

        [TestMethod]
        public void GreaterEqualIntVarMetTest()
        {
            var sb = new StateBuilder();

            var v1 = GetIntVar(1, 4, sb);
            var v2 = GetIntVar(1, 4, sb);
            var v3 = GetIntVar(1, 4, sb);
            var v4 = GetIntVar(1, 4, sb);

            var con1 = new GreaterEqualIntVar(v1, v2);
            var con2 = new GreaterEqualIntVar(v3, v4);

            var state = Initialise(sb, v1, v2, v3, v4);

            Assert.IsTrue(con1.CanBeMet(state));
            Assert.IsFalse(con1.IsMet(state));

            v1.SetMin(state, 3);
            v2.SetMax(state, 2);

            Assert.IsTrue(con1.CanBeMet(state));
            Assert.IsTrue(con1.IsMet(state));

            Assert.IsTrue(con2.CanBeMet(state));
            Assert.IsFalse(con2.IsMet(state));

            v3.SetMax(state, 2);
            v4.SetMin(state, 3);

            Assert.IsFalse(con2.CanBeMet(state));
            Assert.IsFalse(con2.IsMet(state));
        }

        [TestMethod]
        public void AllDiffSameIntDomainMetTest()
        {
            var sb = new StateBuilder();

            var v1 = GetIntVar(1, 10, sb);
            var v2 = GetIntVar(1, 10, sb);
            var v3 = GetIntVar(1, 10, sb);
            var v4 = GetIntVar(1, 10, sb);
            var v5 = GetIntVar(1, 10, sb);
            var v6 = GetIntVar(1, 10, sb);

            var con1 = new AllDiffSameIntDomain(new ISmallIntDomainVar[] { v1, v2, v3 });
            var con2 = new AllDiffSameIntDomain(new ISmallIntDomainVar[] { v4, v5, v6 });

            var state = Initialise(sb, v1, v2, v3, v4, v5, v6);

            Assert.IsTrue(con1.CanBeMet(state));
            Assert.IsFalse(con1.IsMet(state));

            v1.SetMax(state, 3);
            v2.SetMin(state, 4);
            v2.SetMax(state, 6);
            v3.SetMin(state, 7);

            Assert.IsTrue(con1.CanBeMet(state));
            Assert.IsTrue(con1.IsMet(state));

            Assert.IsTrue(con2.CanBeMet(state));
            Assert.IsFalse(con2.IsMet(state));

            v4.SetMax(state, 2);
            v5.SetMax(state, 1);
            v6.SetMax(state, 2);

            Assert.IsFalse(con2.CanBeMet(state));
            Assert.IsFalse(con2.IsMet(state));
        }
    }
}
