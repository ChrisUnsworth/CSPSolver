using System;
using System.Collections.Generic;
using System.Text;
using CSPSolver.common;
using CSPSolver.Constraint.Plus;
using CSPSolver.State;
using CSPSolver.Variable;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSPSolverTests.Variables
{
    [TestClass]
    public class PlusIntDomainTests
    {
        private (IState state, IntSmallDomainVar v1, IntSmallDomainVar v2) GetVar(int min1, int size1, int min2, int size2)
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
        public void PlusMaxTest()
        {
            var (state, v1, v2) = GetVar(1, 3, 1, 3);
            var plus = new PlusIntDomain(v1, v2);

            Assert.AreEqual(6, plus.GetDomainMax(state));

            v1.SetMax(state, 2);
            Assert.AreEqual(5, plus.GetDomainMax(state));

            v2.SetMax(state, 2);
            Assert.AreEqual(4, plus.GetDomainMax(state));
        }

        [TestMethod]
        public void PlusMinTest()
        {
            var (state, v1, v2) = GetVar(1, 3, 1, 3);
            var plus = new PlusIntDomain(v1, v2);

            Assert.AreEqual(2, plus.GetDomainMin(state));

            v1.SetMin(state, 2);
            Assert.AreEqual(3, plus.GetDomainMin(state));

            v2.SetMin(state, 2);
            Assert.AreEqual(4, plus.GetDomainMin(state));
        }

        [TestMethod]
        public void SetMaxTest()
        {
            var (state, v1, v2) = GetVar(1, 3, 1, 3);
            var plus = new PlusIntDomain(v1, v2);

            plus.SetMax(state, 4);
            Assert.AreEqual(3, v1.GetDomainMax(state));
            Assert.AreEqual(3, v2.GetDomainMax(state));

            plus.SetMax(state, 3);
            Assert.AreEqual(2, v1.GetDomainMax(state));
            Assert.AreEqual(2, v2.GetDomainMax(state));

            plus.SetMax(state, 2);
            Assert.AreEqual(1, v1.GetDomainMax(state));
            Assert.AreEqual(1, v2.GetDomainMax(state));

            plus.SetMax(state, 1);
            Assert.IsTrue(plus.IsEmpty(state));
        }

        [TestMethod]
        public void SetMinTest()
        {
            var (state, v1, v2) = GetVar(1, 3, 1, 3);
            var plus = new PlusIntDomain(v1, v2);

            Assert.IsFalse(plus.SetMin(state, 1));
            Assert.AreEqual(1, v1.GetDomainMin(state));
            Assert.AreEqual(1, v2.GetDomainMin(state));

            Assert.IsFalse(plus.SetMin(state, 4));
            Assert.AreEqual(1, v1.GetDomainMin(state));
            Assert.AreEqual(1, v2.GetDomainMin(state));

            Assert.IsTrue(plus.SetMin(state, 5));
            Assert.AreEqual(2, v1.GetDomainMin(state));
            Assert.AreEqual(2, v2.GetDomainMin(state));

            Assert.IsTrue(plus.SetMin(state, 6));
            Assert.AreEqual(3, v1.GetDomainMin(state));
            Assert.AreEqual(3, v2.GetDomainMin(state));

            Assert.IsTrue(plus.SetMin(state, 7));
            Assert.IsTrue(plus.IsEmpty(state));
        }
    }
}
