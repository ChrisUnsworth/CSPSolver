using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSPSolver.Variable;
using CSPSolver.common;
using CSPSolver.State;
using CSPSolver.Math.Round;

namespace CSPSolverTests.Variables
{
    [TestClass]
    public class TruncateTests
    {
        private static (IState state, RealVar variable, Truncate truncate) GetVar(double min, double max)
        {
            var sb = new StateBuilder();
            var variable = new RealVar(min, sb.AddDouble(), max, sb.AddDouble());
            var state = sb.GetState();
            variable.Initialise(state);
            return (state, variable, new Truncate(variable));
        }

        [TestMethod]
        public void GetMax()
        {
            var min = 1.5;
            var max = 7.8;
            var (s, v, t) = GetVar(min, max);

            Assert.AreEqual((int)max, t.GetDomainMax(s));

            max = 6.2;

            v.SetMax(s, max);

            Assert.AreEqual((int)max, t.GetDomainMax(s));
        }

        [TestMethod]
        public void GetMin()
        {
            var min = 1.5;
            var max = 7.8;
            var (s, v, t) = GetVar(min, max);

            Assert.AreEqual((int)min, t.GetDomainMin(s));

            min = 6.2;

            v.SetMin(s, min);

            Assert.AreEqual((int)min, t.GetDomainMin(s));
        }
    }
}
