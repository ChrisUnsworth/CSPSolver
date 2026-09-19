using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSPSolver.Model;
using CSPSolver.State;
using CSPSolver.Search;

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
        private const string RegionLetters = "ABCDEFG";

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

        // Each touching pair is listed once: down, right, and both diagonals.
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
            {
                var row = new List<ModelIntVar>();
                for (var c = 0; c < N; c++) row.Add(At(r, c));
                mb.AddConstraint(SumOf(row) == 1);
            }

            for (var c = 0; c < N; c++)
            {
                var column = new List<ModelIntVar>();
                for (var r = 0; r < N; r++) column.Add(At(r, c));
                mb.AddConstraint(SumOf(column) == 1);
            }

            foreach (var region in RegionLetters)
            {
                var regionCells = new List<ModelIntVar>();
                for (var r = 0; r < N; r++)
                    for (var c = 0; c < N; c++)
                        if (Regions[r][c] == region) regionCells.Add(At(r, c));

                mb.AddConstraint(SumOf(regionCells) == 1);
            }

            for (var r = 0; r < N; r++)
            {
                for (var c = 0; c < N; c++)
                {
                    foreach (var (dr, dc) in HalfNeighbourhood)
                    {
                        var nr = r + dr;
                        var nc = c + dc;
                        if (nr < 0 || nr >= N || nc < 0 || nc >= N) continue;

                        mb.AddConstraint(At(r, c) + At(nr, nc) <= 1);
                    }
                }
            }

            var expectedColumn = new[] { 1, 5, 2, 4, 0, 6, 3 };
            var search = new Search(mb);
            var solutionCount = 0;

            while (search.MoveNext())
            {
                var solution = search.Current;
                solutionCount++;

                for (var r = 0; r < N; r++)
                {
                    var placedColumn = -1;
                    for (var c = 0; c < N; c++)
                        if (solution.GetValue(At(r, c)) == 1) placedColumn = c;

                    Assert.AreEqual(expectedColumn[r], placedColumn);
                }
            }

            Assert.AreEqual(1, solutionCount);
        }

        private static ModelIntVar SumOf(IReadOnlyList<ModelIntVar> vars)
        {
            var total = vars[0];
            for (var i = 1; i < vars.Count; i++) total += vars[i];
            return total;
        }
    }
}