using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSPSolver.Model;
using CSPSolver.State;

namespace CSPSolverTests.Solve.Queens
{
    /// <summary>
    /// LinkedIn's "Queens" puzzle: place one queen per row, column and coloured
    /// region so that no two queens touch, including diagonally -- a stricter
    /// adjacency rule than classic N-Queens, which only forbids sharing a diagonal
    /// line. Modelled as a 0/1 grid with row/column/region sum constraints rather
    /// than AllDiff, unlike the Sudoku suite. The board below is original, built
    /// and checked for a unique solution independently of this solver before being
    /// wired in here.
    /// </summary>
    [TestClass]
    public class QueensTests
    {
        private const int N = 7;

        private static readonly string[] Regions =
        {
            "AAABBBB",
            "AAABBBB",
            "CACCBBB",
            "CCCCDBD",
            "ECCDDDD",
            "EEGDDDF",
            "EGGGGGF",
        };

        private static readonly (int dr, int dc)[] HalfNeighbourhood =
        {
            (1, 0), (0, 1), (1, 1), (1, -1)
        };

        [TestMethod]
        public void FindsTheUniquePlacement()
        {
            var mb = new ModelBuilder(new StateBuilder());
            var cells = mb.AddIntVarArray(0, 1, N * N);

            ModelIntVar At(int r, int c) => cells[r * N + c];

            for (var r = 0; r < N; r++)
                mb.AddConstraint(Enumerable.Range(0, N).Select(c => At(r, c)).Aggregate((a, b) => a + b) == 1);

            for (var c = 0; c < N; c++)
                mb.AddConstraint(Enumerable.Range(0, N).Select(r => At(r, c)).Aggregate((a, b) => a + b) == 1);

            foreach (var region in Regions.SelectMany(row => row).Distinct())
            {
                var regionCells =
                    from r in Enumerable.Range(0, N)
                    from c in Enumerable.Range(0, N)
                    where Regions[r][c] == region
                    select At(r, c);

                mb.AddConstraint(regionCells.Aggregate((a, b) => a + b) == 1);
            }

            for (var r = 0; r < N; r++)
                for (var c = 0; c < N; c++)
                    foreach (var (dr, dc) in HalfNeighbourhood)
                    {
                        var (nr, nc) = (r + dr, c + dc);
                        if (nr >= 0 && nr < N && nc >= 0 && nc < N)
                            mb.AddConstraint(At(r, c) + At(nr, nc) <= 1);
                    }

            var solutions = mb.Search()
                .Select(s => Enumerable.Range(0, N)
                    .Select(r => Enumerable.Range(0, N).Single(c => s.GetValue(At(r, c)) == 1))
                    .ToArray())
                .ToList();

            Assert.AreEqual(1, solutions.Count);
            CollectionAssert.AreEqual(new[] { 1, 5, 2, 4, 0, 6, 3 }, solutions[0]);
        }
    }
}