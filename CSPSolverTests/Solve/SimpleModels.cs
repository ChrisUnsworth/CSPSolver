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
using CSPSolver.Constraint.Multiply;

namespace CSPSolverTests.Solve
{
    [TestClass]
    public class SimpleModels
    {
        private ModelBuilder GetModelBuilder() => new ModelBuilder(new StateBuilder());

        [TestMethod]
        public void XequalsConst()
        {
            var mb = GetModelBuilder();
            var x = mb.AddIntDomainVar(1, 5);

            mb.AddConstraint(x == 3);

            var model = mb.GetModel();

            var search = new Search(model);

            var count = 0;

            while (search.MoveNext())
            {
                var solution = search.Current;

                Assert.IsNotNull(solution);
                Assert.AreEqual(3, solution.GetValue(x));

                count++;
            }

            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void XNotEqualsConst()
        {
            var mb = GetModelBuilder();
            var x = mb.AddIntDomainVar(1, 5);

            mb.AddConstraint(x != 3);

            var model = mb.GetModel();

            var search = new Search(model);

            var count = 0;

            while (search.MoveNext())
            {
                var solution = search.Current;

                Assert.IsNotNull(solution);
                Assert.AreNotEqual(3, solution.GetValue(x));

                count++;
            }

            Assert.AreEqual(4, count);
        }

        [TestMethod]
        public void XequalsY()
        {
            var mb = GetModelBuilder();
            var x = mb.AddIntDomainVar(1, 5).variable;
            var y = mb.AddIntDomainVar(1, 5).variable;

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
            var x = mb.AddIntDomainVar(1, 10).variable;
            var y = mb.AddIntDomainVar(1, 10).variable;
            var z = mb.AddIntDomainVar(1, 10).variable;

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
        public void negativeXplusYequalsZ()
        {
            var mb = GetModelBuilder();
            var x = mb.AddIntDomainVar(-10, -1).variable;
            var y = mb.AddIntDomainVar(-10, -1).variable;
            var z = mb.AddIntDomainVar(-10, -1).variable;

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
            var x = mb.AddIntDomainVar(1, 10).variable;
            var y = mb.AddIntDomainVar(1, 10).variable;
            var z = mb.AddIntDomainVar(1, 10).variable;

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
        public void XmultiplyYequalsZ()
        {
            var mb = GetModelBuilder();
            var x = mb.AddIntDomainVar(1, 10).variable;
            var y = mb.AddIntDomainVar(1, 10).variable;
            var z = mb.AddIntDomainVar(1, 20).variable;

            mb.AddConstraint(new EqualIntVar(new PositiveMultiplyIntVar(x, y), z));

            var model = mb.GetModel();

            var search = new Search(model);

            var count = 0;

            while (search.MoveNext())
            {
                var solution = search.Current;

                Assert.IsNotNull(solution);
                Assert.AreEqual(solution.GetValue(z), solution.GetValue(x) * solution.GetValue(y));

                count++;
            }

            Assert.AreEqual(46, count);
        }

        [TestMethod]
        public void negativeXmultiplyYequalsZ()
        {
            var mb = GetModelBuilder();
            var x = mb.AddIntDomainVar(-10, -1).variable;
            var y = mb.AddIntDomainVar(-10, -1).variable;
            var z = mb.AddIntDomainVar(1, 20).variable;

            mb.AddConstraint(new EqualIntVar(new NegativeMultiplyIntVar(x, y), z));

            var model = mb.GetModel();

            var search = new Search(model);

            var count = 0;

            while (search.MoveNext())
            {
                var solution = search.Current;

                Assert.IsNotNull(solution);
                Assert.AreEqual(solution.GetValue(z), solution.GetValue(x) * solution.GetValue(y));

                count++;
            }

            Assert.AreEqual(46, count);
        }

        [TestMethod]
        public void MixedSignXmultiplyYequalsZ()
        {
            var mb = GetModelBuilder();
            var x = mb.AddIntDomainVar(-3, 3);
            var y = mb.AddIntDomainVar(-3, 3);
            var z = mb.AddIntDomainVar(-10, 10);

            mb.AddConstraint(x * y == z);

            var model = mb.GetModel();

            var search = new Search(model);

            var count = 0;

            while (search.MoveNext())
            {
                var solution = search.Current;

                Assert.IsNotNull(solution);
                Assert.AreEqual(solution.GetValue(z), solution.GetValue(x) * solution.GetValue(y));

                count++;
            }

            Assert.AreEqual(49, count);
        }

        [TestMethod]
        public void AplusBEqualsCminusD()
        {
            var mb = GetModelBuilder();
            var a = mb.AddIntDomainVar(1, 5).variable;
            var b = mb.AddIntDomainVar(1, 5).variable;
            var c = mb.AddIntDomainVar(1, 5).variable;
            var d = mb.AddIntDomainVar(1, 5).variable;

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
            var a = mb.AddIntDomainVar(1, 5);
            var b = mb.AddIntDomainVar(1, 5);
            var c = mb.AddIntDomainVar(1, 5);
            var d = mb.AddIntDomainVar(1, 5);

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

        [TestMethod]
        public void AllDiffTest()
        {
            var mb = GetModelBuilder();
            var vars = mb.AddIntVarArray(1, 3, 3);

            mb.AddAllDiff(vars);

            var model = mb.GetModel();

            var search = new Search(model);

            var count = 0;

            while (search.MoveNext())
            {
                var solution = search.Current;

                Assert.IsNotNull(solution);
                Assert.IsTrue(solution.GetValue(vars[0]) != solution.GetValue(vars[1]));
                Assert.IsTrue(solution.GetValue(vars[0]) != solution.GetValue(vars[2]));
                Assert.IsTrue(solution.GetValue(vars[1]) != solution.GetValue(vars[2]));

                count++;
            }

            Assert.AreEqual(6, count);
        }
    }
}
