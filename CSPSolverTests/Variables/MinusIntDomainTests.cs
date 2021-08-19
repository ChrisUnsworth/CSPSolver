using System;
using System.Collections.Generic;
using System.Text;
using CSPSolver.common;
using CSPSolver.Constraint.Minus;
using CSPSolver.State;
using CSPSolver.Variable;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSPSolverTests.Variables
{
    [TestClass]
    public class MinusIntDomainTests
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
        public void MinusMaxTest()
        {
            var (state, v1, v2) = GetVar(1, 5, 1, 5);
            var plus = new MinusIntDomain(v1, v2);

            Assert.AreEqual(4, plus.GetDomainMax(state));

            v2.SetMin(state, 3);
            Assert.AreEqual(2, plus.GetDomainMax(state));

            v1.SetMax(state, 3);
            Assert.AreEqual(0, plus.GetDomainMax(state));
        }


        [TestMethod]
        public void MinusMinTest()
        {
            var (state, v1, v2) = GetVar(1, 5, 1, 5);
            var plus = new MinusIntDomain(v1, v2);

            Assert.AreEqual(-4, plus.GetDomainMin(state));

            v2.SetMax(state, 3);
            Assert.AreEqual(-2, plus.GetDomainMin(state));

            v1.SetMin(state, 3);
            Assert.AreEqual(0, plus.GetDomainMin(state));
        }

        [TestMethod]
        public void SetMaxTest()
        {
            var (state, v1, v2) = GetVar(1, 5, 1, 5);
            var minus = new MinusIntDomain(v1, v2);

            minus.SetMax(state, 0);
            Assert.AreEqual("{ 1, 2, 3, 4, 5 }", v1.PrettyDomain(state));
            Assert.AreEqual("{ 1, 2, 3, 4, 5 }", v2.PrettyDomain(state));

            minus.SetMax(state, -1);
            Assert.AreEqual("{ 1, 2, 3, 4 }", v1.PrettyDomain(state));
            Assert.AreEqual("{ 2, 3, 4, 5 }", v2.PrettyDomain(state));

            minus.SetMax(state, -4);
            Assert.AreEqual("{ 1 }", v1.PrettyDomain(state));
            Assert.AreEqual("{ 5 }", v2.PrettyDomain(state));

            minus.SetMax(state, -5);
            Assert.IsTrue(minus.IsEmpty(state));
        }

        [TestMethod]
        public void SetMinTest()
        {
            var (state, v1, v2) = GetVar(1, 5, 1, 5);
            var minus = new MinusIntDomain(v1, v2);

            Assert.IsFalse(minus.SetMin(state, 0));
            Assert.AreEqual("{ 1, 2, 3, 4, 5 }", v1.PrettyDomain(state));
            Assert.AreEqual("{ 1, 2, 3, 4, 5 }", v2.PrettyDomain(state));

            Assert.IsTrue(minus.SetMin(state, 1));
            Assert.AreEqual("{ 2, 3, 4, 5 }", v1.PrettyDomain(state));
            Assert.AreEqual("{ 1, 2, 3, 4 }", v2.PrettyDomain(state));

            Assert.IsTrue(minus.SetMin(state, 4));
            Assert.AreEqual("{ 5 }", v1.PrettyDomain(state));
            Assert.AreEqual("{ 1 }", v2.PrettyDomain(state));

            Assert.IsTrue(minus.SetMin(state, 5));
            Assert.IsTrue(minus.IsEmpty(state));
        }
    }
}
