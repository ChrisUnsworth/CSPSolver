using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSPSolver.Model;
using CSPSolver.State;
using CSPSolver.Variable;

namespace CSPSolverTests.Variables
{
    /// <summary>
    /// Sweeps domain sizes across the boundaries where ModelBuilder switches
    /// variable backing. Every other test in the suite uses domains under about
    /// thirty values, so most of the size range has never been exercised.
    /// </summary>
    [TestClass]
    public class DomainSizeTests
    {
        private const int LargestEnumerable = 200;
        private const int LargestSize = 200;

        [TestMethod]
        public void ReportsCorrectBoundsForEveryDomainSize()
        {
            var faults = new List<string>();

            for (var size = 1; size <= LargestSize; size++)
            {
                var (backing, min, max) = Bounds(size);

                if (min != 0 || max != size - 1)
                    faults.Add($"size {size} ({backing}) reports {min}..{max}, expected 0..{size - 1}");
            }

            Assert.AreEqual(
                0,
                faults.Count,
                $"{faults.Count} of {LargestSize} sizes report the wrong bounds:{Environment.NewLine}" +
                string.Join(Environment.NewLine, faults.Take(12)));
        }

        [TestMethod]
        public void EnumeratesEveryValueForEveryDomainSize()
        {
            var faults = new List<string>();

            for (var size = 1; size <= LargestEnumerable; size++)
            {
                var mb = new ModelBuilder();
                var v = mb.AddIntDomainVar(0, size - 1);

                var found = mb.Search().Select(s => s.GetValue(v.Variable)).OrderBy(i => i).ToList();
                var expected = Enumerable.Range(0, size).ToList();

                if (!found.SequenceEqual(expected))
                    faults.Add($"size {size} enumerated {found.Count} values, expected {size}");
            }

            Assert.AreEqual(
                0,
                faults.Count,
                $"{faults.Count} of {LargestEnumerable} sizes enumerate the wrong values:{Environment.NewLine}" +
                string.Join(Environment.NewLine, faults.Take(12)));
        }

        [TestMethod]
        public void InitialisingALargeDomainLeavesAPackedNeighbourAlone()
        {
            // A domain whose size is not a multiple of 32 leaves spare bits in its
            // last word, and StateBuilder packs the next small domain into them.
            var sb = new StateBuilder();
            var large = new IntDomainVar(0, 65, sb.AddDomain(65));
            var neighbour = sb.AddDomain(5);
            var state = sb.GetState();

            state.SetDomain(neighbour, 5, 0b10101);
            large.Initialise(state);

            Assert.AreEqual(0b10101u, state.GetDomain(neighbour, 5));
        }

        private static (string backing, int min, int max) Bounds(int size)
        {
            var mb = new ModelBuilder(new StateBuilder());
            var v = mb.AddIntDomainVar(0, size - 1);
            var state = new IntState(mb.GetStateSize());

            v.Variable.Initialise(state);

            return (v.Variable.GetType().Name, v.Variable.GetDomainMin(state), v.Variable.GetDomainMax(state));
        }
    }
}
