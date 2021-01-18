using CSPSolver.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace CSPSolverTests
{
    [TestClass]
    public class IntStateTests
    {
        [TestMethod]
        public void SetAndGet()
        {
            var value = 0b10101;
            var state = new IntState(1);
            var stateRef = new StateRef(0, 0);

            state.SetDomain(stateRef, 5, value);

            var result = state.GetDomain(stateRef, 5);
            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void SetAndGetOffset()
        {
            var value = 0b10101;
            var state = new IntState(1);
            var stateRef = new StateRef(0, 5);

            state.SetDomain(stateRef, 5, value);

            var result = state.GetDomain(stateRef, 5);
            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void SetAndGet2values()
        {
            var value1 = 0b10101;
            var value2 = 0b10111;
            var state = new IntState(1);
            var stateRef1 = new StateRef(0, 0);
            var stateRef2 = new StateRef(0, 5);

            state.SetDomain(stateRef1, 5, value1);
            state.SetDomain(stateRef2, 5, value2);

            var result1 = state.GetDomain(stateRef1, 5);
            var result2 = state.GetDomain(stateRef2, 5);
            Assert.AreEqual(value1, result1);
            Assert.AreEqual(value2, result2);
        }

        [TestMethod]
        public void SetAndGet2valuesFull()
        {
            var value1 = 0b1111_0000_1010_1111;
            var value2 = 0b1001_1001_1001_1001;
            var state = new IntState(1);
            var stateRef1 = new StateRef(0, 0);
            var stateRef2 = new StateRef(0, 16);

            state.SetDomain(stateRef1, 16, value1);
            state.SetDomain(stateRef2, 16, value2);

            var result1 = state.GetDomain(stateRef1, 16);
            var result2 = state.GetDomain(stateRef2, 16);
            Assert.AreEqual(value1, result1);
            Assert.AreEqual(value2, result2);
        }

        [TestMethod]
        public void GetAndSetBigDomain()
        {
            var values = new int[] { 46, 77, 0b1111_0000_1010_1111 };
            var state = new IntState(3);
            var stateRef = new StateRef(0, 0);

            state.SetLargeDomain(stateRef, 80, values);

            var results = state.GetLargeDomain(stateRef, 80);

            values.Zip(results).ToList().ForEach(x => Assert.AreEqual(x.First, x.Second));
        }

        [TestMethod]
        public void SetAndGetLong()
        {
            var value = 648654854854865488L;
            var state = new IntState(2);
            var stateRef = new StateRef(0, 0);

            state.SetLong(stateRef, value);

            var result = state.GetLong(stateRef);
            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void SetAndGetDouble()
        {
            var value = -345345e-345;
            var state = new IntState(2);
            var stateRef = new StateRef(0, 0);

            state.SetDouble(stateRef, value);

            var result = state.GetDouble(stateRef);
            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void SetAndGetFloat()
        {
            float value = 345345e-345F;
            var state = new IntState(2);
            var stateRef = new StateRef(0, 0);

            state.SetFloat(stateRef, value);

            var result = state.GetFloat(stateRef);
            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void GetDomainMax()
        {
            var state = new IntState(1);
            var stateRef = new StateRef(0, 0);

            state.SetDomain(stateRef, 16, 0b1010_1000_0000_0000);

            Assert.AreEqual(15, state.GetDomainMax(stateRef, 16));

            state.SetDomain(stateRef, 16, 0b0010_1000_0000_0000);

            Assert.AreEqual(13, state.GetDomainMax(stateRef, 16));

            state.SetDomain(stateRef, 16, 0b0000_1000_0000_0000);

            Assert.AreEqual(11, state.GetDomainMax(stateRef, 16));

            state.SetDomain(stateRef, 16, 0b0000_0000_0000_0001);

            Assert.AreEqual(0, state.GetDomainMax(stateRef, 16));
        }

        [TestMethod]
        public void GetDomainMin()
        {
            var state = new IntState(1);
            var stateRef = new StateRef(0, 0);

            state.SetDomain(stateRef, 16, 0b1010_1000_1000_0000);

            Assert.AreEqual(7, state.GetDomainMin(stateRef, 16));

            state.SetDomain(stateRef, 16, 0b0010_1000_0010_0000);

            Assert.AreEqual(5, state.GetDomainMin(stateRef, 16));

            state.SetDomain(stateRef, 16, 0b0000_1000_0000_0000);

            Assert.AreEqual(11, state.GetDomainMin(stateRef, 16));

            state.SetDomain(stateRef, 16, 0b0000_0000_0000_0001);

            Assert.AreEqual(0, state.GetDomainMin(stateRef, 16));
        }
    }
}
