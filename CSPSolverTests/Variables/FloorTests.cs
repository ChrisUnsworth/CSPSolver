using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSPSolver.Variable;
using CSPSolver.common;
using CSPSolver.State;
using CSPSolver.Math.Round;

using static System.Math;

namespace CSPSolverTests.Variables
{
    [TestClass]
    public class FloorTests
    {
        private static (IState state, RealVar variable, Floor truncate) GetVar(double min, double max)
        {
            var sb = new StateBuilder();
            var variable = new RealVar(min, sb.AddDouble(), max, sb.AddDouble());
            var state = sb.GetState();
            variable.Initialise(state);
            return (state, variable, new Floor(variable));
        }

        [TestMethod]
        public void GetMax()
        {
            var min = 1.5;
            var max = 7.8;
            var (s, v, t) = GetVar(min, max);

            Assert.AreEqual(Floor(max), t.GetDomainMax(s));

            max = 6.2;

            v.SetMax(s, max);

            Assert.AreEqual(Floor(max), t.GetDomainMax(s));
        }

        [TestMethod]
        public void GetNegativeMax()
        {
            var min = -11.5;
            var max = -2.8;
            var (s, v, t) = GetVar(min, max);

            Assert.AreEqual(Floor(max), t.GetDomainMax(s));

            max = -6.2;

            v.SetMax(s, max);

            Assert.AreEqual(Floor(max), t.GetDomainMax(s));
        }

        [TestMethod]
        public void GetMin()
        {
            var min = 1.5;
            var max = 7.8;
            var (s, v, t) = GetVar(min, max);

            Assert.AreEqual(Floor(min), t.GetDomainMin(s));

            min = 6.2;

            v.SetMin(s, min);

            Assert.AreEqual(Floor(min), t.GetDomainMin(s));
        }

        [TestMethod]
        public void GetNegativeMin()
        {
            var min = -11.5;
            var max = -1.8;
            var (s, v, t) = GetVar(min, max);

            Assert.AreEqual(Floor(min), t.GetDomainMin(s));

            min = -6.2;

            v.SetMin(s, min);

            Assert.AreEqual(Floor(min), t.GetDomainMin(s));
        }
    }
}
