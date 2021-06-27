using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSPSolver.common;
using CSPSolver.Constraint.Multiply;
using CSPSolver.State;
using CSPSolver.Variable;

namespace CSPSolverTests.Variables
{
    [TestClass]
    public class PositiveDivideIntVarTests
    {
        private (IState state, IntSmallDomainVar v1, IntSmallDomainVar v2) GetVar(int min1, int size1, int min2, int size2)
        {
            var sb = new StateBuilder();
            var variable1 = new IntSmallDomainVar(min1, size1, sb.AddDomain(size1));
            var variable2 = new IntSmallDomainVar(min2, size2, sb.AddDomain(size2));
            var state = sb.GetState();
            variable1.initialise(state);
            variable2.initialise(state);
            return (state, variable1, variable2);
        }

        [TestMethod]
        public void MaxTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void MinTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void SetMaxTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void SetMinTest()
        {
            throw new NotImplementedException();
        }
    }
}
