using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSPSolver.Math.Multiply;
using CSPSolver.Model;

namespace CSPSolverTests.Variables
{
    /// <summary>
    /// Sweeps small domains across every operand sign combination and compares what
    /// search finds against brute force, one test per multiply variable so a failure
    /// names the implementation at fault.
    /// </summary>
    [TestClass]
    public class MultiplyCompletenessTests
    {
        private const int ZLo = -20;
        private const int ZHi = 20;

        private readonly record struct Window(int XLo, int XHi, int YLo, int YHi)
        {
            public override string ToString() => $"x[{XLo},{XHi}] y[{YLo},{YHi}]";
        }

        private static IEnumerable<Window> Windows()
        {
            for (var xlo = -4; xlo <= 3; xlo++)
                for (var xhi = xlo; xhi <= Math.Min(xlo + 4, 4); xhi++)
                    for (var ylo = -3; ylo <= 2; ylo++)
                        for (var yhi = ylo; yhi <= Math.Min(ylo + 3, 3); yhi++)
                            yield return new Window(xlo, xhi, ylo, yhi);
        }

        private static ISet<(int x, int y, int z)> Expected(Window w)
        {
            var expected = new HashSet<(int, int, int)>();

            for (var x = w.XLo; x <= w.XHi; x++)
                for (var y = w.YLo; y <= w.YHi; y++)
                    if (x * y >= ZLo && x * y <= ZHi)
                        expected.Add((x, y, x * y));

            return expected;
        }

        private static (Type multiply, ISet<(int x, int y, int z)> found) Solve(Window w)
        {
            var mb = new ModelBuilder();
            var x = mb.AddIntDomainVar(w.XLo, w.XHi);
            var y = mb.AddIntDomainVar(w.YLo, w.YHi);
            var z = mb.AddIntDomainVar(ZLo, ZHi);

            var multiply = (x * y).Variable.GetType();
            mb.AddConstraint(x * y == z);

            var found = mb.Search()
                          .Select(s => (s.GetValue(x.Variable), s.GetValue(y.Variable), s.GetValue(z.Variable)))
                          .ToHashSet();

            return (multiply, found);
        }

        private static void AssertCompleteFor(Type multiplyVariable)
        {
            var faults = new List<string>();
            var checkedWindows = 0;

            foreach (var window in Windows())
            {
                var (multiply, found) = Solve(window);
                if (multiply != multiplyVariable) continue;

                checkedWindows++;
                var expected = Expected(window);
                var missing = expected.Except(found).ToList();
                var bogus = found.Except(expected).ToList();

                if (missing.Count > 0 || bogus.Count > 0)
                {
                    var detail = string.Join(" ", missing.Take(3).Select(m => $"missing {m.x}*{m.y}=={m.z}")
                                        .Concat(bogus.Take(3).Select(b => $"bogus {b.x}*{b.y}=={b.z}")));
                    faults.Add($"{window}: {missing.Count} missing, {bogus.Count} bogus  {detail}");
                }
            }

            Assert.AreNotEqual(0, checkedWindows, $"no window routed to {multiplyVariable.Name}");
            Assert.AreEqual(
                0,
                faults.Count,
                $"{faults.Count} of {checkedWindows} windows wrong for {multiplyVariable.Name}:{Environment.NewLine}" +
                string.Join(Environment.NewLine, faults.Take(10)));
        }

        [TestMethod]
        public void PositiveMultiplyFindsEverySolution() => AssertCompleteFor(typeof(PositiveMultiplyIntVar));

        [TestMethod]
        public void NegativeMultiplyFindsEverySolution() => AssertCompleteFor(typeof(NegativeMultiplyIntVar));

        [TestMethod]
        public void MixedSignMultiplyFindsEverySolution() => AssertCompleteFor(typeof(MixedSignMultiplyIntVar));
    }
}
