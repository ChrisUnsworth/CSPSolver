using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSPSolver.Variable;
using CSPSolver.common;
using CSPSolver.State;
using CSPSolver.common.search;

namespace CSPSolverTests.Variables
{
    [TestClass]
    public class IntDomainVarTests
    {
        private (IState state, IntDomainVar variable) GetVar(int min, int size)
        {
            var sb = new StateBuilder();
            var stateRef = sb.AddDomain(size);
            var variable = new IntDomainVar(min, size, stateRef);
            var state = sb.GetState();
            variable.initialise(state);
            return (state, variable);
        }

        [TestMethod]
        public void GetInitialiseTest()
        {
            var min = 5;
            var size = 50;

            var (s, v) = GetVar(min, size);

            Assert.AreEqual(min, v.GetDomainMin(s));

            Assert.AreEqual(min + size - 1, v.GetDomainMax(s));
        }

        [TestMethod]
        public void GetSetSmallDomainMinTest()
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
        public void GetSetLargeDomainMinTest()
        {
            var min = 5;
            var size = 100;

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

            min = 70;
            Assert.IsTrue(v.SetMin(s, min));
            Assert.AreEqual(min, v.GetDomainMin(s));
            Assert.IsFalse(v.TryGetValue(s, out _));
            Assert.IsFalse(v.isEmpty(s));

            lowerMin = 60;
            Assert.IsFalse(v.SetMin(s, lowerMin));
            Assert.AreEqual(min, v.GetDomainMin(s));
            Assert.IsFalse(v.TryGetValue(s, out _));
            Assert.IsFalse(v.isEmpty(s));

            min = 104;
            Assert.IsTrue(v.SetMin(s, min));
            Assert.AreEqual(min, v.GetDomainMin(s));
            Assert.IsFalse(v.isEmpty(s));
            Assert.IsTrue(v.TryGetValue(s, out int val));
            Assert.AreEqual(min, val);

            min = 105;
            Assert.IsTrue(v.SetMin(s, min));
            Assert.IsTrue(v.isEmpty(s));
        }

        [TestMethod]
        public void GetSetSmallDomainMaxTest()
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

        [TestMethod]
        public void GetSetLargeDomainMaxTest()
        {
            var min = 5;
            var size = 100;
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

            max = 90;
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

        [TestMethod]
        public void PrettySmallDomainTest()
        {
            var (s, v) = GetVar(3, 5);
            Assert.AreEqual("{ 3, 4, 5, 6, 7 }", v.PrettyDomain(s));

            v.RemoveValue(s, 5);
            Assert.AreEqual("{ 3, 4, 6, 7 }", v.PrettyDomain(s));

            v.RemoveValue(s, 7);
            Assert.AreEqual("{ 3, 4, 6 }", v.PrettyDomain(s));

            v.RemoveValue(s, 3);
            Assert.AreEqual("{ 4, 6 }", v.PrettyDomain(s));

            v.RemoveValue(s, 4);
            Assert.AreEqual("{ 6 }", v.PrettyDomain(s));

            v.RemoveValue(s, 6);
            Assert.AreEqual("{  }", v.PrettyDomain(s));
        }

        [TestMethod]
        public void PrettyLargeDomainTest()
        {
            var (s, v) = GetVar(3, 50);
            Assert.AreEqual("{ 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52 }", v.PrettyDomain(s));

            v.RemoveValue(s, 5);
            Assert.AreEqual("{ 3, 4, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52 }", v.PrettyDomain(s));

            v.RemoveValue(s, 47);
            Assert.AreEqual("{ 3, 4, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 48, 49, 50, 51, 52 }", v.PrettyDomain(s));

            v.RemoveValue(s, 32);
            Assert.AreEqual("{ 3, 4, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 48, 49, 50, 51, 52 }", v.PrettyDomain(s));
        }
    }
}
