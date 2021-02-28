using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSPSolver.Variable;
using CSPSolver.common;
using CSPSolver.State;

namespace CSPSolverTests.Variables
{
    [TestClass]
    public class IntSmallDomainVarTests
    {
        private (IState state, IntSmallDomainVar variable) GetVar(int min, int size)
        {
            var sb = new StateBuilder();
            var stateRef = sb.AddDomain(size);
            var variable = new IntSmallDomainVar(min, size, stateRef);
            var state = sb.GetState();
            variable.initialise(state);
            return (state, variable);
        }

        [TestMethod]
        public void GetInitialiseTest()
        {
            var min = 5;
            var size = 10;

            var (s, v) = GetVar(min, size);

            Assert.AreEqual(min, v.GetDomainMin(s));

            Assert.AreEqual(min + size - 1, v.GetDomainMax(s));
        }

        [TestMethod]
        public void GetSetDomainMinTest()
        {
            var min = 5;
            var size = 10;

            var (s, v) = GetVar(min, size);

            Assert.AreEqual(min, v.GetDomainMin(s));
            Assert.IsFalse(v.SetMin(s, min));
            Assert.AreEqual(min, v.GetDomainMin(s));
            Assert.IsFalse(v.TryGetValue(s, out _));
            Assert.IsFalse(v.isEmpty(s));

            var lowerMin = 3;
            Assert.IsFalse(v.SetMin(s, lowerMin));
            Assert.AreEqual(min, v.GetDomainMin(s));
            Assert.IsFalse(v.TryGetValue(s, out _));
            Assert.IsFalse(v.isEmpty(s));

            min = 7;
            Assert.IsTrue(v.SetMin(s, min));
            Assert.AreEqual(min, v.GetDomainMin(s));
            Assert.IsFalse(v.TryGetValue(s, out _));
            Assert.IsFalse(v.isEmpty(s));

            lowerMin = 6;
            Assert.IsFalse(v.SetMin(s, lowerMin));
            Assert.AreEqual(min, v.GetDomainMin(s));
            Assert.IsFalse(v.TryGetValue(s, out _));
            Assert.IsFalse(v.isEmpty(s));

            min = 14;
            Assert.IsTrue(v.SetMin(s, min));
            Assert.AreEqual(min, v.GetDomainMin(s));
            Assert.IsFalse(v.isEmpty(s));
            Assert.IsTrue(v.TryGetValue(s, out int val));
            Assert.AreEqual(min, val);

            min = 15;
            Assert.IsTrue(v.SetMin(s, min));
            Assert.IsTrue(v.isEmpty(s));
        }

        [TestMethod]
        public void GetSetDomainMaxTest()
        {
            var min = 5;
            var size = 10;
            var max = min + size - 1;

            var (s, v) = GetVar(min, size);

            Assert.AreEqual(max, v.GetDomainMax(s));
            Assert.IsFalse(v.SetMax(s, max + 1));
            Assert.AreEqual(max, v.GetDomainMax(s));
            Assert.IsFalse(v.TryGetValue(s, out _));
            Assert.IsFalse(v.isEmpty(s));

            Assert.IsFalse(v.SetMax(s, max));
            Assert.AreEqual(max, v.GetDomainMax(s));
            Assert.IsFalse(v.TryGetValue(s, out _));
            Assert.IsFalse(v.isEmpty(s));

            max = 10;
            Assert.IsTrue(v.SetMax(s, max));
            Assert.AreEqual(max, v.GetDomainMax(s));
            Assert.IsFalse(v.TryGetValue(s, out _));
            Assert.IsFalse(v.isEmpty(s));

            Assert.IsFalse(v.SetMax(s, max));
            Assert.AreEqual(max, v.GetDomainMax(s));
            Assert.IsFalse(v.TryGetValue(s, out _));
            Assert.IsFalse(v.isEmpty(s));

            max = 5;
            Assert.IsTrue(v.SetMax(s, max));
            Assert.AreEqual(max, v.GetDomainMax(s));
            Assert.IsTrue(v.TryGetValue(s, out int val));
            Assert.AreEqual(max, val);
            Assert.IsFalse(v.isEmpty(s));

            max = 4;
            Assert.IsTrue(v.SetMax(s, max));
            Assert.IsTrue(v.isEmpty(s));
        }
    }
}
