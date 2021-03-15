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
using CSPSolver.Constraint.Minus;

namespace CSPSolverTests.Solve
{
    [TestClass]
    public class SimpleModels
    {
        private ModelBuilder GetModelBuilder()
        {
            var sb = new StateBuilder();
            var mb = new ModelBuilder(sb);
            return mb;
        }

        [TestMethod]
        public void XequalsY()
        {
            var mb = GetModelBuilder();
            var x = mb.AddSmallIntVar(1, 5).variable;
            var y = mb.AddSmallIntVar(1, 5).variable;

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
            var x = mb.AddSmallIntVar(1, 10).variable;
            var y = mb.AddSmallIntVar(1, 10).variable;
            var z = mb.AddSmallIntVar(1, 10).variable;

            mb.AddConstraint(new EqualIntVar(new PlusIntDomain(x, y), z));

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

            Assert.AreEqual(45, count);
        }

        [TestMethod]
        public void XminusYequalsZ()
        {
            var mb = GetModelBuilder();
            var x = mb.AddSmallIntVar(1, 10).variable;
            var y = mb.AddSmallIntVar(1, 10).variable;
            var z = mb.AddSmallIntVar(1, 10).variable;

            mb.AddConstraint(new EqualIntVar(new MinusIntDomain(x, y), z));

            var model = mb.GetModel();

            var search = new Search(model);

            var count = 0;

            while (search.MoveNext())
            {
                var solution = search.Current;

                Assert.IsNotNull(solution);
                Assert.AreEqual(solution.GetValue(z), solution.GetValue(x) - solution.GetValue(y));

                count++;
            }

            Assert.AreEqual(45, count);
        }

        [TestMethod]
        public void AplusBEqualsCminusD()
        {
            var mb = GetModelBuilder();
            var a = mb.AddSmallIntVar(1, 5).variable;
            var b = mb.AddSmallIntVar(1, 5).variable;
            var c = mb.AddSmallIntVar(1, 5).variable;
            var d = mb.AddSmallIntVar(1, 5).variable;

            mb.AddConstraint(new EqualIntVar(new PlusIntDomain(a, b), new MinusIntDomain(c, d)));

            var model = mb.GetModel();

            var search = new Search(model);

            var count = 0;

            while (search.MoveNext())
            {
                var solution = search.Current;

                Assert.IsNotNull(solution);
                Assert.AreEqual(solution.GetValue(a) + solution.GetValue(b), solution.GetValue(c) - solution.GetValue(d));

                count++;
            }

            Assert.AreEqual(10, count);
        }

        [TestMethod]
        public void AplusBEqualsCminusDInline()
        {
            var mb = GetModelBuilder();
            var a = mb.AddSmallIntVar(1, 5);
            var b = mb.AddSmallIntVar(1, 5);
            var c = mb.AddSmallIntVar(1, 5);
            var d = mb.AddSmallIntVar(1, 5);

            mb.AddConstraint(a + b == c - d);

            var model = mb.GetModel();

            var search = new Search(model);

            var count = 0;

            while (search.MoveNext())
            {
                var solution = search.Current;

                Assert.IsNotNull(solution);
                Assert.AreEqual(solution.GetValue(a) + solution.GetValue(b), solution.GetValue(c) - solution.GetValue(d));

                count++;
            }

            Assert.AreEqual(10, count);
        }
    }
}
