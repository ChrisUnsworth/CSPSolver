using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSPSolver.Variable;
using CSPSolver.common;
using CSPSolver.State;

namespace CSPSolverTests.Variables
{
    [TestClass]
    public class BoolVarTests
    {
        private (IState state, BoolVar variable) GetVar()
        {
            var sb = new StateBuilder();
            var stateRef = sb.AddDomain(2);
            var variable = new BoolVar(stateRef);
            var state = sb.GetState();
            variable.Initialise(state);
            return (state, variable);
        }

        [TestMethod]
        public void GetInitialiseTest()
        {
            var (s, v) = GetVar();
            Assert.AreEqual(0, v.GetDomainMin(s));
            Assert.AreEqual(1, v.GetDomainMax(s));
        }

        [TestMethod]
        public void IsTrueTest()
        {
            var (s, v) = GetVar();

            Assert.IsFalse(v.IsTrue(s));

            v.SetValue(s, true);

            Assert.IsTrue(v.IsTrue(s));

            (s, v) = GetVar();

            Assert.IsFalse(v.IsTrue(s));

            v.SetValue(s, false);

            Assert.IsFalse(v.IsTrue(s));

            v.SetValue(s, true);

            Assert.IsFalse(v.IsTrue(s));
        }

        [TestMethod]
        public void CanBeTrueTest()
        {
            var (s, v) = GetVar();

            Assert.IsTrue(v.CanBeTrue(s));

            v.SetValue(s, true);

            Assert.IsTrue(v.CanBeTrue(s));

            (s, v) = GetVar();

            Assert.IsTrue(v.CanBeTrue(s));

            v.SetValue(s, false);

            Assert.IsFalse(v.CanBeTrue(s));

            v.SetValue(s, true);

            Assert.IsFalse(v.CanBeTrue(s));
        }

        [TestMethod]
        public void IsFalseTest()
        {
            var (s, v) = GetVar();

            Assert.IsFalse(v.IsFalse(s));

            v.SetValue(s, false);

            Assert.IsTrue(v.IsFalse(s));

            (s, v) = GetVar();

            Assert.IsFalse(v.IsFalse(s));

            v.SetValue(s, true);

            Assert.IsFalse(v.IsFalse(s));

            v.SetValue(s, false);

            Assert.IsFalse(v.IsFalse(s));
        }

        [TestMethod]
        public void CanBeFalseTest()
        {
            var (s, v) = GetVar();

            Assert.IsTrue(v.CanBeFalse(s));

            v.SetValue(s, false);

            Assert.IsTrue(v.CanBeFalse(s));

            (s, v) = GetVar();

            Assert.IsTrue(v.CanBeFalse(s));

            v.SetValue(s, true);

            Assert.IsFalse(v.CanBeFalse(s));

            v.SetValue(s, false);

            Assert.IsFalse(v.CanBeFalse(s));
        }

        [TestMethod]
        public void TryGetValueTest()
        {
            var (s, v) = GetVar();

            Assert.IsFalse(v.TryGetValue(s, out _));

            v.SetValue(s, true);

            Assert.IsTrue(v.TryGetValue(s, out int t) && t == 1);

            v.SetValue(s, false);

            Assert.IsFalse(v.TryGetValue(s, out _));

            (s, v) = GetVar();

            v.SetValue(s, false);

            Assert.IsTrue(v.TryGetValue(s, out int f) && f == 0);
        }
    }
}
