﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSPSolver.Variable;
using CSPSolver.common;
using CSPSolver.State;

namespace CSPSolverTests.Variables
{
    [TestClass]
    public class RealVarTests
    {
        private static (IState state, RealVar variable) GetVar(double min, double max)
        {
            var sb = new StateBuilder();
            var stateRef1 = sb.AddDouble();
            var stateRef2 = sb.AddDouble();
            var variable = new RealVar(min, stateRef1, max, stateRef2);
            var state = sb.GetState();
            variable.Initialise(state);
            return (state, variable);
        }

        [TestMethod]
        public void GetInitialiseTest()
        {
            var (s, v) = GetVar(3.3, 6.6);
            Assert.AreEqual(3.3, v.GetDomainMin(s));
            Assert.AreEqual(6.6, v.GetDomainMax(s));
        }

        [TestMethod]
        public void SetMaxTest()
        {
            var max = 66457.6547457;
            var min = 333.355;
            var (s, v) = GetVar(min, max);

            Assert.IsFalse(v.SetMax(s, max + 10));
            Assert.IsFalse(v.IsEmpty(s));
            Assert.IsFalse(v.IsInstantiated(s));

            max -= 100;

            Assert.IsTrue(v.SetMax(s, max));
            Assert.AreEqual(max, v.GetDomainMax(s));
            Assert.IsFalse(v.IsEmpty(s));
            Assert.IsFalse(v.IsInstantiated(s));
            Assert.IsFalse(v.SetMax(s, max));
            Assert.IsFalse(v.IsEmpty(s));
            Assert.IsFalse(v.IsInstantiated(s));

            Assert.IsTrue(v.SetMax(s, min));
            Assert.AreEqual(min, v.GetDomainMax(s));
            Assert.IsFalse(v.IsEmpty(s));
            Assert.IsTrue(v.IsInstantiated(s));

            Assert.IsTrue(v.SetMax(s, min - 10));
            Assert.IsTrue(v.IsEmpty(s));
        }

        [TestMethod]
        public void SetMinTest()
        {
            var max = 66457.6547457;
            var min = 333.355;
            var (s, v) = GetVar(min, max);

            Assert.IsFalse(v.SetMin(s, min - 10));
            Assert.IsFalse(v.IsEmpty(s));
            Assert.IsFalse(v.IsInstantiated(s));

            min += 100;

            Assert.IsTrue(v.SetMin(s, min));
            Assert.AreEqual(min, v.GetDomainMin(s));
            Assert.IsFalse(v.IsEmpty(s));
            Assert.IsFalse(v.IsInstantiated(s));
            Assert.IsFalse(v.SetMin(s, min));
            Assert.IsFalse(v.IsEmpty(s));
            Assert.IsFalse(v.IsInstantiated(s));

            Assert.IsTrue(v.SetMin(s, max));
            Assert.AreEqual(max, v.GetDomainMin(s));
            Assert.IsFalse(v.IsEmpty(s));
            Assert.IsTrue(v.IsInstantiated(s));

            Assert.IsTrue(v.SetMin(s, max + 10));
            Assert.IsTrue(v.IsEmpty(s));
        }
    }
}
