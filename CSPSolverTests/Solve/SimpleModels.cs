using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSPSolver.Model;
using CSPSolver.State;
using CSPSolver.common;
using CSPSolver.Constraint.Equal;
using CSPSolver.Math.Plus;
using CSPSolver.Math.Minus;
using CSPSolver.Math.Multiply;
using CSPSolver.Math.Divide;

using static CSPSolver.Model.ModelConstraint;
using static CSPSolver.Model.ModelRealVar;

namespace CSPSolverTests.Solve
{
    [TestClass]
    public class SimpleModels
    {
        private static ModelBuilder GetModelBuilder() => new(new StateBuilder());

        [TestMethod]
        public void XequalsConst()
        {
            var mb = GetModelBuilder();
            var x = mb.AddIntDomainVar(1, 5);

            mb.AddConstraint(x == 3);

            void test(ISolution solution)
            {
                Assert.IsNotNull(solution);
                Assert.AreEqual(3, solution.GetValue(x));
            }

            CheckAll(mb, test, 1);
        }

        [TestMethod]
        public void XNotEqualsConst()
        {
            var mb = GetModelBuilder();
            var x = mb.AddIntDomainVar(1, 5);

            mb.AddConstraint(x != 3);

            void test(ISolution solution)
            {
                Assert.IsNotNull(solution);
                Assert.AreNotEqual(3, solution.GetValue(x));
            }

            CheckAll(mb, test, 4);
        }

        [TestMethod]
        public void XequalsY()
        {
            var mb = GetModelBuilder();
            var x = mb.AddIntDomainVar(1, 5);
            var y = mb.AddIntDomainVar(1, 5);

            mb.AddConstraint(x == y);

            void test(ISolution solution)
            {
                Assert.IsNotNull(solution);
                Assert.AreEqual(solution.GetValue(x), solution.GetValue(y));
            }

            CheckAll(mb, test, 5);
        }

        [TestMethod]
        public void XplusYequalsZ()
        {
            var mb = GetModelBuilder();
            var x = mb.AddIntDomainVar(1, 10);
            var y = mb.AddIntDomainVar(1, 10);
            var z = mb.AddIntDomainVar(1, 10);

            mb.AddConstraint(x + y == z);

            void test(ISolution solution)
            {
                Assert.IsNotNull(solution);
                Assert.AreEqual(solution.GetValue(z), solution.GetValue(x) + solution.GetValue(y));
            }

            CheckAll(mb, test, 45);
        }

        [TestMethod]
        public void NegativeXplusYequalsZ()
        {
            var mb = GetModelBuilder();
            var x = mb.AddIntDomainVar(-10, -1);
            var y = mb.AddIntDomainVar(-10, -1);
            var z = mb.AddIntDomainVar(-10, -1);

            mb.AddConstraint(x + y == z);

            void test(ISolution solution)
            {
                Assert.IsNotNull(solution);
                Assert.AreEqual(solution.GetValue(z), solution.GetValue(x) + solution.GetValue(y));
            }

            CheckAll(mb, test, 45);
        }

        [TestMethod]
        public void XminusYequalsZ()
        {
            var mb = GetModelBuilder();
            var x = mb.AddIntDomainVar(1, 10);
            var y = mb.AddIntDomainVar(1, 10);
            var z = mb.AddIntDomainVar(1, 10);

            mb.AddConstraint(x - y == z);

            void test(ISolution solution)
            {
                Assert.IsNotNull(solution);
                Assert.AreEqual(solution.GetValue(z), solution.GetValue(x) - solution.GetValue(y));
            }

            CheckAll(mb, test, 45);
        }

        [TestMethod]
        public void MixedSignXminusYequalsZ()
        {
            var mb = GetModelBuilder();
            var x = mb.AddIntDomainVar(-5, 5).Variable;
            var y = mb.AddIntDomainVar(-5, 5).Variable;
            var z = mb.AddIntDomainVar(-5, 5).Variable;

            mb.AddConstraint(new EqualIntVar(new MinusIntDomain(x, y), z));

            void test(ISolution solution)
            {
                Assert.IsNotNull(solution);
                Assert.AreEqual(solution.GetValue(z), solution.GetValue(x) - solution.GetValue(y));
            }

            CheckAll(mb, test, 91);
        }

        [TestMethod]
        public void XmultiplyYequalsZ()
        {
            var mb = GetModelBuilder();
            var x = mb.AddIntDomainVar(1, 10).Variable;
            var y = mb.AddIntDomainVar(1, 10).Variable;
            var z = mb.AddIntDomainVar(1, 20).Variable;

            mb.AddConstraint(new EqualIntVar(new PositiveMultiplyIntVar(x, y), z));

            void test(ISolution solution)
            {
                Assert.IsNotNull(solution);
                Assert.AreEqual(solution.GetValue(z), solution.GetValue(x) * solution.GetValue(y));
            }

            CheckAll(mb, test, 46);
        }

        [TestMethod]
        public void XdivideYequalsZ()
        {
            var mb = GetModelBuilder();
            var x = mb.AddIntDomainVar(1, 10).Variable;
            var y = mb.AddIntDomainVar(1, 10).Variable;
            var z = mb.AddIntDomainVar(0, 5).Variable;

            mb.AddConstraint(new EqualIntVar(new PositiveDivideIntVar(x, y), z));

            void test(ISolution solution)
            {
                Assert.IsNotNull(solution);
                Assert.AreEqual(solution.GetValue(z), solution.GetValue(x) / solution.GetValue(y));
            }

            CheckAll(mb, test, 95);
        }

        [TestMethod]
        public void NegativeXdivideYequalsZ()
        {
            var mb = GetModelBuilder();
            var x = mb.AddIntDomainVar(-10, -1).Variable;
            var y = mb.AddIntDomainVar(-10, -1).Variable;
            var z = mb.AddIntDomainVar(0, 5).Variable;

            mb.AddConstraint(new EqualIntVar(new NegativeDivideIntVar(x, y), z));

            void test(ISolution solution)
            {
                Assert.IsNotNull(solution);
                Assert.AreEqual(solution.GetValue(z), solution.GetValue(x) / solution.GetValue(y));
            }

            CheckAll(mb, test, 95);
        }

        [TestMethod]
        public void NegativeXmultiplyYequalsZ()
        {
            var mb = GetModelBuilder();
            var x = mb.AddIntDomainVar(-10, -1).Variable;
            var y = mb.AddIntDomainVar(-10, -1).Variable;
            var z = mb.AddIntDomainVar(1, 20).Variable;

            mb.AddConstraint(new EqualIntVar(new NegativeMultiplyIntVar(x, y), z));

            void test(ISolution solution)
            {
                Assert.IsNotNull(solution);
                Assert.AreEqual(solution.GetValue(z), solution.GetValue(x) * solution.GetValue(y));
            }

            CheckAll(mb, test, 46);
        }

        [Ignore]
        public void MixedSignXmultiplyYequalsZ()
        {
            var mb = GetModelBuilder();
            var x = mb.AddIntDomainVar(-3, 3);
            var y = mb.AddIntDomainVar(-3, 3);
            var z = mb.AddIntDomainVar(-10, 10);

            mb.AddConstraint(x * y == z);

            void test(ISolution solution)
            {
                Assert.IsNotNull(solution);
                Assert.AreEqual(solution.GetValue(z), solution.GetValue(x) * solution.GetValue(y));
            }

            CheckAll(mb, test, 49);
        }

        [TestMethod]
        public void AplusBEqualsCminusD()
        {
            var mb = GetModelBuilder();
            var a = mb.AddIntDomainVar(1, 5).Variable;
            var b = mb.AddIntDomainVar(1, 5).Variable;
            var c = mb.AddIntDomainVar(1, 5).Variable;
            var d = mb.AddIntDomainVar(1, 5).Variable;

            mb.AddConstraint(new EqualIntVar(new PlusIntVar(a, b), new MinusIntDomain(c, d)));

            void test(ISolution solution)
            {
                Assert.IsNotNull(solution);
                Assert.AreEqual(solution.GetValue(a) + solution.GetValue(b), solution.GetValue(c) - solution.GetValue(d));
            }

            CheckAll(mb, test, 10);
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

            void test(ISolution solution)
            {
                Assert.IsNotNull(solution);
                Assert.AreEqual(solution.GetValue(a) + solution.GetValue(b), solution.GetValue(c) - solution.GetValue(d));
            }

            CheckAll(mb, test, 10);
        }

        [TestMethod]
        public void AplusConstEqualsCminusDInline()
        {
            var mb = GetModelBuilder();
            var a = mb.AddIntDomainVar(1, 5);
            var b = 2;
            var c = mb.AddIntDomainVar(1, 5);
            var d = mb.AddIntDomainVar(1, 5);

            mb.AddConstraint(a + b == c - d);

            void test(ISolution solution)
            {
                Assert.AreEqual(solution.GetValue(a) + b, solution.GetValue(c) - solution.GetValue(d));
            }

            CheckAll(mb, test, 3);
        }

        [TestMethod]
        public void XGreaterThanY()
        {
            var mb = GetModelBuilder();
            var x = mb.AddIntDomainVar(1, 5);
            var y = mb.AddIntDomainVar(1, 5);

            mb.AddConstraint(x > y);

            void test(ISolution solution)
            {
                Assert.IsTrue(solution.GetValue(x) > solution.GetValue(y));
            }

            CheckAll(mb, test, 10);
        }

        [TestMethod]
        public void XGreaterEqualY()
        {
            var mb = GetModelBuilder();
            var x = mb.AddIntDomainVar(1, 5);
            var y = mb.AddIntDomainVar(1, 5);

            mb.AddConstraint(x >= y);

            void test(ISolution solution)
            {
                Assert.IsTrue(solution.GetValue(x) >= solution.GetValue(y));
            }

            CheckAll(mb, test, 15);
        }

        [TestMethod]
        public void XLessThanY()
        {
            var mb = GetModelBuilder();
            var x = mb.AddIntDomainVar(1, 5);
            var y = mb.AddIntDomainVar(1, 5);

            mb.AddConstraint(x < y);

            void test(ISolution solution)
            {
                Assert.IsTrue(solution.GetValue(x) < solution.GetValue(y));
            }

            CheckAll(mb, test, 10);
        }

        [TestMethod]
        public void XLessEqualY()
        {
            var mb = GetModelBuilder();
            var x = mb.AddIntDomainVar(1, 5);
            var y = mb.AddIntDomainVar(1, 5);

            mb.AddConstraint(x <= y);

            void test(ISolution solution)
            {
                Assert.IsTrue(solution.GetValue(x) <= solution.GetValue(y));
            }

            CheckAll(mb, test, 15);
        }

        [TestMethod]
        public void AllDiffTest()
        {
            var mb = GetModelBuilder();
            var vars = mb.AddIntVarArray(1, 3, 3);

            mb.AddConstraint(AllDiff(vars));

            void test(ISolution solution)
            {
                Assert.IsTrue(solution.GetValue(vars[0]) != solution.GetValue(vars[1]));
                Assert.IsTrue(solution.GetValue(vars[0]) != solution.GetValue(vars[2]));
                Assert.IsTrue(solution.GetValue(vars[1]) != solution.GetValue(vars[2]));
            }

            CheckAll(mb, test, 6);
        }

        [TestMethod]
        public void NotAllDiffTest()
        {
            var mb = GetModelBuilder();
            var vars = mb.AddIntVarArray(1, 3, 3);

            mb.AddConstraint(Not(AllDiff(vars)));

            void test(ISolution solution)
            {
                Assert.IsTrue((solution.GetValue(vars[0]) == solution.GetValue(vars[1]))
                           || (solution.GetValue(vars[0]) == solution.GetValue(vars[2]))
                           || (solution.GetValue(vars[1]) == solution.GetValue(vars[2])));
            }

            CheckAll(mb, test, 21);
        }

        [TestMethod]
        public void TruncateTest()
        {
            var mb = new ModelBuilder();
            var x = mb.AddRealVar(1, 3);
            var y = mb.AddIntDomainVar(0, 10);

            mb.AddConstraint(Truncate(x) == y);

            void test(ISolution solution)
            {
                var (min, max) = solution.GetValueRange(x);
                Assert.IsTrue((int)min == solution.GetValue(y));
                Assert.IsTrue((int)max == solution.GetValue(y));
            }

            CheckAll(mb, test, 3);
        }

        [TestMethod]
        public void TestDoubleEqual()
        {
            var mb = new ModelBuilder();
            var x = mb.AddRealVar(1, 3);
            var y = mb.AddIntDomainVar(0, 10);

            mb.AddConstraint(x == y);

            void test(ISolution solution)
            {
                var (min, max) = solution.GetValueRange(x);
                Assert.IsTrue(min == solution.GetValue(y));
                Assert.IsTrue(max == solution.GetValue(y));
            }

            CheckAll(mb, test, 3);
        }

        [TestMethod]
        public void TestDoubleNotEqual()
        {
            // TODO: needs revisiting when variable/value ordering works with RealVal
            var mb = new ModelBuilder();
            var x = mb.AddRealVar(1, 3);
            var y = mb.AddIntDomainVar(0, 10);

            mb.AddConstraint(x != y);

            void test(ISolution solution)
            {
                var (min, max) = solution.GetValueRange(x);
                Assert.IsTrue(min != solution.GetValue(y) || max != solution.GetValue(y));
            }

            CheckAll(mb, test, 11);
        }

        private static void CheckAll(ModelBuilder mb, Action<ISolution> test, int expected)
        {
            var count = 0;

            foreach (var solution in mb.Search())
            {
                test(solution);
                count++;
            }

            Assert.AreEqual(expected, count);
        }
    }
}
