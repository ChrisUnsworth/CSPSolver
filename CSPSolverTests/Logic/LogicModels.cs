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
using CSPSolver.Constraint.Divide;
using CSPSolver.Constraint.Logic;

namespace CSPSolverTests.Logic
{
    [TestClass]
    public class LogicModels
    {
        private static ModelBuilder GetModelBuilder() => new(new StateBuilder());

        [TestMethod]
        public void OrTest()
        {
            var mb = GetModelBuilder();

            var x = mb.AddIntDomainVar(1, 3);
            var y = mb.AddIntDomainVar(1, 3);

            mb.AddConstraint(x == 2 | y == 2);

            void test(ISolution solution)
            {
                Assert.IsTrue(solution.GetValue(x) == 2 || solution.GetValue(y) == 2);
            }

            CheckAll(mb, test, 5);
        }

        [TestMethod]
        public void AndTest()
        {
            var mb = GetModelBuilder();

            var x = mb.AddIntDomainVar(1, 3);
            var y = mb.AddIntDomainVar(1, 3);

            mb.AddConstraint(x == 2 & y == 2);

            void test(ISolution solution)
            {
                Assert.IsTrue(solution.GetValue(x) == 2);
                Assert.IsTrue(solution.GetValue(y) == 2);
            }

            CheckAll(mb, test, 1);
        }

        [TestMethod]
        public void IfThenTest()
        {
            var mb = GetModelBuilder();

            var x = mb.AddIntDomainVar(1, 3);
            var y = mb.AddIntDomainVar(1, 3);

            mb.AddConstraint(ModelConstraint.IfThen(x == 2, y == 2));

            void test(ISolution solution)
            {
                if (solution.GetValue(x) == 2) Assert.IsTrue(solution.GetValue(y) == 2);
            }

            CheckAll(mb, test, 7);
        }

        [TestMethod]
        public void IfThenElseTest()
        {
            var mb = GetModelBuilder();

            var x = mb.AddIntDomainVar(1, 3);
            var y = mb.AddIntDomainVar(1, 3);

            mb.AddConstraint(ModelConstraint.IfThenElse(x == 2, y == 2, y == 1));

            void test(ISolution solution)
            {
                if (solution.GetValue(x) == 2) Assert.IsTrue(solution.GetValue(y) == 2);
                else Assert.IsTrue(solution.GetValue(y) == 1);
            }

            CheckAll(mb, test, 3);
        }

        [TestMethod]
        public void BoolVarTest()
        {
            var mb = GetModelBuilder();

            var x = mb.AddBoolVar();
            var y = mb.AddBoolVar();
            var z = mb.AddBoolVar();

            mb.AddConstraint(x | y);
            mb.AddConstraint(y | z);


            void test(ISolution solution)
            {
                Assert.IsTrue(solution.GetValue(x) || solution.GetValue(y));
                Assert.IsTrue(solution.GetValue(y) || solution.GetValue(z));
            }

            CheckAll(mb, test, 5);
        }

        private static void CheckAll(ModelBuilder mb, Action<ISolution> test, int expected)
        {
            var search = new Search(mb);
            var count = 0;

            while (search.MoveNext())
            {
                var solution = search.Current;
                test(solution);
                count++;
            }

            Assert.AreEqual(expected, count);
        }
    }
}
