using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSPSolver.common;
using CSPSolver.common.variables;
using CSPSolver.Constraint.Equal;
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

        [TestMethod]
        public void EqualIntVatIsMetTest()
        {
            var sb = new StateBuilder();

            var v1 = new IntSmallDomainVar(2, 2, sb.AddDomain(2));
            var v2 = new IntSmallDomainVar(2, 2, sb.AddDomain(2));
            var v3 = new IntSmallDomainVar(2, 1, sb.AddDomain(1));
            var v4 = new IntSmallDomainVar(2, 1, sb.AddDomain(1));
            var v5 = new IntSmallDomainVar(7, 2, sb.AddDomain(2));
            var v6 = new IntSmallDomainVar(2, 2, sb.AddDomain(2));

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
    }
}
