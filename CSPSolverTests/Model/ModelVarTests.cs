using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSPSolver.common;
using CSPSolver.Model;
using CSPSolver.State;

namespace CSPSolverTests.Model
{
    [TestClass]
    public class ModelVarTests
    {
        private static (IState state, ModelBuilder mb, ModelIntVar x) IntVar(int min, int max)
        {
            var mb = new ModelBuilder();
            var x = mb.AddIntDomainVar(min, max);
            var state = new IntState(mb.GetStateSize());
            mb.GetModel().Initialise(state);
            return (state, mb, x);
        }

        [TestMethod]
        public void InitialiseDelegatesToTheUnderlyingVariable()
        {
            var mb = new ModelBuilder();
            var x = mb.AddIntDomainVar(1, 5);
            var state = new IntState(mb.GetStateSize());

            x.Initialise(state);

            Assert.IsFalse(x.IsEmpty(state));
            Assert.AreEqual(x.Variable.PrettyDomain(state), x.PrettyDomain(state));
        }

        [TestMethod]
        public void IsEmptyDelegatesToTheUnderlyingVariable()
        {
            var (state, _, x) = IntVar(1, 5);

            Assert.IsFalse(x.IsEmpty(state));

            x.Variable.SetMin(state, 99);

            Assert.IsTrue(x.IsEmpty(state));
        }

        [TestMethod]
        public void IsInstantiatedDelegatesToTheUnderlyingVariable()
        {
            var (state, _, x) = IntVar(1, 5);

            Assert.IsFalse(x.IsInstantiated(state));

            x.Variable.SetMax(state, 1);

            Assert.IsTrue(x.IsInstantiated(state));
        }

        [TestMethod]
        public void SetValueDelegatesToTheUnderlyingVariable()
        {
            var (state, _, x) = IntVar(1, 5);

            Assert.IsTrue(x.SetValue(state, 3));

            Assert.IsTrue(x.TryGetValue(state, out var value));
            Assert.AreEqual(3, value);
        }

        [TestMethod]
        public void RemoveValueDelegatesToTheUnderlyingVariable()
        {
            var (state, _, x) = IntVar(1, 5);

            Assert.IsTrue(x.RemoveValue(state, 1));

            Assert.AreEqual(2, x.Variable.GetDomainMin(state));
        }

        [TestMethod]
        public void PrettyDomainDelegatesToTheUnderlyingVariable()
        {
            var (state, _, x) = IntVar(1, 3);

            Assert.AreEqual(x.Variable.PrettyDomain(state), x.PrettyDomain(state));
        }

        [TestMethod]
        public void GetValuesReturnsEveryValueInOrder()
        {
            var mb = new ModelBuilder();
            var x = mb.AddIntDomainVar(1, 5);
            var y = mb.AddIntDomainVar(1, 5);
            var z = mb.AddIntDomainVar(1, 5);
            mb.AddConstraint(x == 1);
            mb.AddConstraint(y == 2);
            mb.AddConstraint(z == 3);

            var solution = mb.Search().First();
            var vars = new List<IVariable<int>> { x.Variable, y.Variable, z.Variable };

            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, solution.GetValues(vars).ToArray());
        }
    }
}
