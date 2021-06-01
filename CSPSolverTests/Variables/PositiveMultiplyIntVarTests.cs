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
    public class PositiveMultiplyIntVarTests
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
            var (state, v1, v2) = GetVar(1, 3, 1, 3);
            var multiply = new PositiveMultiplyIntVar(v1, v2);

            Assert.AreEqual(9, multiply.GetDomainMax(state));

            v1.SetMax(state, 2);
            Assert.AreEqual(6, multiply.GetDomainMax(state));

            v2.SetMax(state, 2);
            Assert.AreEqual(4, multiply.GetDomainMax(state));
        }

        [TestMethod]
        public void MinTest()
        {
            var (state, v1, v2) = GetVar(1, 3, 1, 3);
            var multiply = new PositiveMultiplyIntVar(v1, v2);

            Assert.AreEqual(1, multiply.GetDomainMin(state));

            v1.SetMin(state, 2);
            Assert.AreEqual(2, multiply.GetDomainMin(state));

            v2.SetMin(state, 2);
            Assert.AreEqual(4, multiply.GetDomainMin(state));
        }

        [TestMethod]
        public void SetMaxTest()
        {
            var (state, v1, v2) = GetVar(1, 3, 1, 3);
            var multiply = new PositiveMultiplyIntVar(v1, v2);

            multiply.SetMax(state, 3);
            Assert.AreEqual(3, v1.GetDomainMax(state));
            Assert.AreEqual(3, v2.GetDomainMax(state));
            Assert.IsFalse(multiply.isEmpty(state));

            multiply.SetMax(state, 2);
            Assert.AreEqual(2, v1.GetDomainMax(state));
            Assert.AreEqual(2, v2.GetDomainMax(state));
            Assert.IsFalse(multiply.isEmpty(state));

            multiply.SetMax(state, 1);
            Assert.AreEqual(1, v1.GetDomainMax(state));
            Assert.AreEqual(1, v2.GetDomainMax(state));
            Assert.IsFalse(multiply.isEmpty(state));

            multiply.SetMax(state, 0);
            Assert.IsTrue(multiply.isEmpty(state));
        }

        [TestMethod]
        public void SetMinTest()
        {
            var (state, v1, v2) = GetVar(1, 3, 1, 3);
            var multiply = new PositiveMultiplyIntVar(v1, v2);

            Assert.IsFalse(multiply.SetMin(state, 1));
            Assert.AreEqual(1, v1.GetDomainMin(state));
            Assert.AreEqual(1, v2.GetDomainMin(state));
            Assert.IsFalse(multiply.isEmpty(state));

            Assert.IsFalse(multiply.SetMin(state, 3));
            Assert.AreEqual(1, v1.GetDomainMin(state));
            Assert.AreEqual(1, v2.GetDomainMin(state));
            Assert.IsFalse(multiply.isEmpty(state));

            Assert.IsTrue(multiply.SetMin(state, 4));
            Assert.AreEqual(2, v1.GetDomainMin(state));
            Assert.AreEqual(2, v2.GetDomainMin(state));
            Assert.IsFalse(multiply.isEmpty(state));

            Assert.IsTrue(multiply.SetMin(state, 9));
            Assert.AreEqual(3, v1.GetDomainMin(state));
            Assert.AreEqual(3, v2.GetDomainMin(state));
            Assert.IsFalse(multiply.isEmpty(state));

            Assert.IsTrue(multiply.SetMin(state, 10));
            Assert.IsTrue(multiply.isEmpty(state));
        }
    }
}
