using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSPSolver.Model;
using CSPSolver.State;
using CSPSolver.Search;

namespace CSPSolverTests.Solve
{
    [TestClass]
    public class Optimisation
    {
        private static ModelBuilder GetModelBuilder() => new(new StateBuilder());

        [TestMethod]
        public void MaxX()
        {
            var max = 5;
            var mb = GetModelBuilder();
            var x = mb.AddIntDomainVar(1, max);

            mb.AddObjective(x, maximise: true);

            var search = new Search(mb);

            var value = 0;

            foreach (var solution in search)
            {
                Assert.IsNotNull(solution);
                Assert.IsTrue(solution.GetValue(x) > value);
                value = solution.GetValue(x);
            }

            Assert.AreEqual(max, value);
        }

        [TestMethod]
        public void MinX()
        {
            var min = 1;
            var mb = GetModelBuilder();
            var x = mb.AddIntDomainVar(min, 10);

            mb.AddObjective(x, maximise: false);

            var search = new Search(mb);

            var value = 11;

            foreach (var solution in search)
            {
                Assert.IsNotNull(solution);
                Assert.IsTrue(solution.GetValue(x) < value);
                value = solution.GetValue(x);
            }

            Assert.AreEqual(min, value);
        }
    }
}
