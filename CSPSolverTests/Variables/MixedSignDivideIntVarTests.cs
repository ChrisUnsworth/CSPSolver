using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSPSolver.common;
using CSPSolver.State;
using CSPSolver.Variable;
using CSPSolver.Constraint.Divide;
using CSPSolver.Constraint.Equal;

namespace CSPSolverTests.Variables
{
    [TestClass]
    public class MixedSignDivideIntVarTests
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

        [Ignore]
        public void PropergateTest()
        {
            var sb = new StateBuilder();
            var a = new IntSmallDomainVar(-5, 1, sb.AddDomain(1));
            var b = new IntSmallDomainVar(4, 2, sb.AddDomain(11));
            var c = new IntSmallDomainVar(-1, 1, sb.AddDomain(11));
            var state = sb.GetState();
            a.initialise(state);
            b.initialise(state);
            c.initialise(state);
            var divide = new MixedSignDivideIntVar(a, b);
            var constraint = new EqualIntVar(divide, c);

            var result = constraint.Propagate(state);
            Assert.IsFalse(result.Any());

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void PosMaxTest()
        {
            var (state, v1, v2) = GetVar(1, 10, 1, 10);
            var divide = new MixedSignDivideIntVar(v1, v2);

            Assert.AreEqual(10, divide.GetDomainMax(state));

            v1.SetMax(state, 6);
            Assert.AreEqual(6, divide.GetDomainMax(state));

            v2.SetMin(state, 2);
            Assert.AreEqual(3, divide.GetDomainMax(state));
        }

        [TestMethod]
        public void NegMaxTest()
        {
            var (state, v1, v2) = GetVar(-10, 10, -10, 10);
            var divide = new MixedSignDivideIntVar(v1, v2);

            Assert.AreEqual(10, divide.GetDomainMax(state));

            v1.SetMin(state, -6);
            Assert.AreEqual(6, divide.GetDomainMax(state));

            v2.SetMax(state, -2);
            Assert.AreEqual(3, divide.GetDomainMax(state));
        }

        [TestMethod]
        public void MixedMaxTest()
        {
            var (state, v1, v2) = GetVar(-5, 11, -5, 11);
            var divide = new MixedSignDivideIntVar(v1, v2);

            Assert.AreEqual(5, divide.GetDomainMax(state));

            v1.SetMin(state, -4);
            Assert.AreEqual(5, divide.GetDomainMax(state));

            v1.SetMax(state, 4);
            Assert.AreEqual(4, divide.GetDomainMax(state));

            v2.SetMin(state, 2);
            Assert.AreEqual(2, divide.GetDomainMax(state));

            (state, v1, v2) = GetVar(-5, 11, -5, 11);
            divide = new MixedSignDivideIntVar(v1, v2);

            v2.SetMax(state, -2);
            Assert.AreEqual(2, divide.GetDomainMax(state));
        }



        [TestMethod]
        public void PosMinTest()
        {
            var (state, v1, v2) = GetVar(1, 10, 1, 10);
            var divide = new MixedSignDivideIntVar(v1, v2);

            Assert.AreEqual(0, divide.GetDomainMin(state));

            v1.SetMin(state, 6);
            Assert.AreEqual(0, divide.GetDomainMin(state));

            v2.SetMax(state, 3);
            Assert.AreEqual(2, divide.GetDomainMin(state));
        }


        [TestMethod]
        public void NegMinTest()
        {
            var (state, v1, v2) = GetVar(-10, 10, -10, 10);
            var divide = new MixedSignDivideIntVar(v1, v2);

            Assert.AreEqual(0, divide.GetDomainMin(state));

            v1.SetMax(state, -6);
            Assert.AreEqual(0, divide.GetDomainMin(state));

            v2.SetMin(state, -3);
            Assert.AreEqual(2, divide.GetDomainMin(state));
        }


        [TestMethod]
        public void MixSignBoundsTest()
        {
            var (state, v1, v2) = GetVar(-5, 1, -5, 11);
            var divide = new MixedSignDivideIntVar(v1, v2);

            Assert.AreEqual(-5, divide.GetDomainMin(state));
            Assert.AreEqual(5, divide.GetDomainMax(state));

            v2.SetMin(state, -4);
            Assert.AreEqual(-5, divide.GetDomainMin(state));
            Assert.AreEqual(5, divide.GetDomainMax(state));

            v2.SetMin(state, -3);
            Assert.AreEqual(-5, divide.GetDomainMin(state));
            Assert.AreEqual(5, divide.GetDomainMax(state));

            v2.SetMin(state, -2);
            Assert.AreEqual(-5, divide.GetDomainMin(state));
            Assert.AreEqual(5, divide.GetDomainMax(state));

            v2.SetMin(state, -1);
            Assert.AreEqual(-5, divide.GetDomainMin(state));
            Assert.AreEqual(5, divide.GetDomainMax(state));

            v2.SetMin(state, 0);
            Assert.AreEqual(-5, divide.GetDomainMin(state));
            Assert.AreEqual(-1, divide.GetDomainMax(state));

            v2.SetMin(state, 1);
            Assert.AreEqual(-5, divide.GetDomainMin(state));
            Assert.AreEqual(-1, divide.GetDomainMax(state));

            v2.SetMin(state, 2);
            Assert.AreEqual(-2, divide.GetDomainMin(state));
            Assert.AreEqual(-1, divide.GetDomainMax(state));

            v2.SetMin(state, 3);
            Assert.AreEqual(-1, divide.GetDomainMin(state));
            Assert.AreEqual(-1, divide.GetDomainMax(state));

            v2.SetMin(state, 4);
            Assert.AreEqual(-1, divide.GetDomainMin(state));
            Assert.AreEqual(-1, divide.GetDomainMax(state));

            v2.SetMin(state, 5);
            Assert.AreEqual(-1, divide.GetDomainMin(state));
            Assert.AreEqual(-1, divide.GetDomainMax(state));
        }


            [TestMethod]
        public void MixSignMinTest()
        {
            var (state, v1, v2) = GetVar(-10, 21, -10, 21);
            var divide = new MixedSignDivideIntVar(v1, v2);

            Assert.AreEqual(-10, divide.GetDomainMin(state));

            v1.SetMax(state, 6);
            Assert.AreEqual(-10, divide.GetDomainMin(state));

            v1.SetMin(state, -6);
            Assert.AreEqual(-6, divide.GetDomainMin(state));

            v2.SetMin(state, -3);
            Assert.AreEqual(-6, divide.GetDomainMin(state));

            v2.SetMin(state, 3);
            Assert.AreEqual(-2, divide.GetDomainMin(state));

            (state, v1, v2) = GetVar(-10, 21, -10, 21);
            divide = new MixedSignDivideIntVar(v1, v2);

            v1.SetMax(state, 6);
            v1.SetMin(state, -6);
            v2.SetMax(state, -3);
            Assert.AreEqual(-2, divide.GetDomainMin(state));

            (state, v1, v2) = GetVar(-10, 21, -10, 21);
            divide = new MixedSignDivideIntVar(v1, v2);

            v1.SetMax(state, 0);
            Assert.AreEqual(-10, divide.GetDomainMin(state));

            v1.SetMin(state, 0);
            Assert.AreEqual(0, divide.GetDomainMin(state));

            (state, v1, v2) = GetVar(-10, 21, -10, 21);
            divide = new MixedSignDivideIntVar(v1, v2);

            v2.SetMax(state, 0);
            Assert.AreEqual(-10, divide.GetDomainMin(state));

            (state, v1, v2) = GetVar(-10, 21, -10, 21);
            divide = new MixedSignDivideIntVar(v1, v2);

            v2.SetMin(state, 0);
            Assert.AreEqual(-10, divide.GetDomainMin(state));

            (state, v1, v2) = GetVar(-10, 21, -10, 21);
            divide = new MixedSignDivideIntVar(v1, v2);

            v1.SetMin(state, 0);
            v2.SetMin(state, 0);
            Assert.AreEqual(0, divide.GetDomainMin(state));
        }



        [Ignore]
        public void MixedSetMinTest()
        {
            var (state, v1, v2) = GetVar(-5, 1, 4, 2);
            var divide = new MixedSignDivideIntVar(v1, v2);

            Assert.AreEqual(-1, divide.GetDomainMin(state));
            Assert.AreEqual(-1, divide.GetDomainMax(state));

            Assert.AreEqual(-5, v1.GetDomainMax(state));
            Assert.AreEqual(-5, v1.GetDomainMin(state));
            Assert.AreEqual(-5, v2.GetDomainMin(state));
            Assert.AreEqual(-1, v2.GetDomainMax(state));
        }
    }
}
