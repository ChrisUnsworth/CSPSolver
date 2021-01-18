using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSPSolver.State;

namespace CSPSolverTests.State
{
    [TestClass]
    public class StateBuilderTests
    {

        [TestMethod]
        public void MakeAState()
        {
            var sb = new StateBuilder();
            var stateRef = sb.AddDomain(5);

            var value = 0b10101;
            var state = sb.GetState();

            state.SetDomain(stateRef, 5, value);

            var result = state.GetDomain(stateRef, 5);
            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void MakeAStateMultipleDomains()
        {
            var sb = new StateBuilder();
            var stateRef1 = sb.AddDomain(16);
            var stateRef2 = sb.AddDomain(8);
            var stateRef3 = sb.AddDomain(16);

            var value1 = 0b1010_1111_0010_1000;
            var value2 = 0b1010_1000;
            var value3 = 0b1110_1101_1010_1001;
            var state = sb.GetState();

            state.SetDomain(stateRef1, 16, value1);
            state.SetDomain(stateRef2, 8, value2);
            state.SetDomain(stateRef3, 16, value3);

            var result1 = state.GetDomain(stateRef1, 16);
            Assert.AreEqual(value1, result1);

            var result2 = state.GetDomain(stateRef2, 8);
            Assert.AreEqual(value2, result2);

            var result3 = state.GetDomain(stateRef3, 16);
            Assert.AreEqual(value3, result3);
        }
    }
}
