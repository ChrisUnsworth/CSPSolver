using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSPSolver.Model;
using CSPSolver.State;
using CSPSolver.common;
using CSPSolver.Constraint.Equal;
using CSPSolver.Constraint.Plus;
using CSPSolver.Search;

namespace CSPSolverTests.Solve
{
    [TestClass]
    public class SimpleModels
    {
        private IModelBuilder GetModelBuilder()
        {
            var sb = new StateBuilder();
            var mb = new ModelBuilder(sb);
            return mb;
        }

        [TestMethod]
        public void XequalsY()
        {
            var mb = GetModelBuilder();
            var x = mb.AddSmallIntVar(1, 5);
            var y = mb.AddSmallIntVar(1, 5);

            mb.AddConstraint(new EqualIntVar(x, y));

            var model = mb.GetModel();

            var search = new Search(model);

            var count = 0;

            while (search.MoveNext())
            {
                var solution = search.Current;

                Assert.IsNotNull(solution);
                Assert.AreEqual(solution.GetValue(x), solution.GetValue(y));

                count++;
            }

            Assert.AreEqual(5, count);
        }

        [TestMethod]
        public void XplusYequalsZ()
        {
            var mb = GetModelBuilder();
            var x = mb.AddSmallIntVar(1, 10);
            var y = mb.AddSmallIntVar(1, 10);
            var z = mb.AddSmallIntVar(1, 10);

            mb.AddConstraint(new EqualIntVar(new PlusSmallIntDomain(x, y), z));

            var model = mb.GetModel();

            var search = new Search(model);

            var count = 0;

            while (search.MoveNext())
            {
                var solution = search.Current;

                Assert.IsNotNull(solution);
                Assert.AreEqual(solution.GetValue(z), solution.GetValue(x) + solution.GetValue(y));

                count++;
            }

            Assert.AreEqual(0, count);
        }
    }
}
