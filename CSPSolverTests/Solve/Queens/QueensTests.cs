using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSPSolver.Model;
using CSPSolver.State;
using CSPSolver.Search;

using static CSPSolver.Model.ModelConstraint;

namespace CSPSolverTests.Solve.Queens;

/// <summary>
/// LinkedIn's "Queens" puzzle: place one queen per row, column and coloured
/// region so that no two queens touch, including diagonally -- a stricter
/// adjacency rule than classic N-Queens, which only forbids sharing a diagonal
/// line. Modelled as a 0/1 grid with row/column/region cardinality constraints
/// rather than AllDiff, unlike the Sudoku suite.
/// </summary>
[TestClass]
public class QueensTests
{
    // Each touching pair is listed once: down, right, and both diagonals.
    private static readonly (int dr, int dc)[] HalfNeighbourhood =
    [
        (1, 0), (0, 1), (1, 1), (1, -1)
    ];

    private static (ModelBuilder mb, ModelBoolVar[,] board) BuildBaseModel(int size)
    {
        var mb = new ModelBuilder(new StateBuilder());
        var vars = mb.AddBoolVarArray(size * size);

        var board = new ModelBoolVar[size, size];
        foreach (var (r, c) in AllCells(size))
        {
            board[r, c] = vars[r * size + c];
        }

        foreach (var cells in AllCells(size).GroupBy(cell => cell.r))
        {
            var row = cells.Select(cell => board[cell.r, cell.c]);
            mb.AddConstraint(Cardinality(row, 1));
        }

        foreach (var cells in AllCells(size).GroupBy(cell => cell.c))
        {
            var column = cells.Select(cell => board[cell.r, cell.c]);
            mb.AddConstraint(Cardinality(column, 1));
        }

        foreach (var (r, c) in AllCells(size))
        {
            foreach (var (dr, dc) in HalfNeighbourhood)
            {
                var nr = r + dr;
                var nc = c + dc;
                if (nr >= 0 && nr < size && nc >= 0 && nc < size)
                {
                    mb.AddConstraint(NAnd(board[r, c], board[nr, nc]));
                }
            }
        }

        return (mb, board);
    }

    private static IEnumerable<(int r, int c)> AllCells(int size)
    {
        for (var r = 0; r < size; r++)
        {
            for (var c = 0; c < size; c++)
            {
                yield return (r, c);
            }
        }
    }

    private static void SetInstance(ModelBuilder mb, ModelBoolVar[,] board, string[] regions) =>
        AllCells(regions.Length)
            .Select(cell => (region: regions[cell.r][cell.c], var: board[cell.r, cell.c]))
            .GroupBy(p => p.region)
            .Select(g => g.Select(p => p.var).ToArray())
            .ToList()
            .ForEach(region => mb.AddConstraint(Cardinality(region, 1)));

    private static void RunTest(ModelBuilder mb, ModelBoolVar[,] board, string[] regions, int[] expectedColumn)
    {
        SetInstance(mb, board, regions);

        var size = regions.Length;
        var search = new Search(mb);
        var solutionCount = 0;

        while (search.MoveNext())
        {
            var solution = search.Current;
            solutionCount++;

            for (var r = 0; r < size; r++)
            {
                var placedColumn = -1;
                for (var c = 0; c < size; c++)
                {
                    if (solution.GetValue(board[r, c]))
                    {
                        placedColumn = c;
                    }
                }

                Assert.AreEqual(expectedColumn[r], placedColumn);
            }
        }

        Assert.AreEqual(1, solutionCount);
    }

    [TestMethod]
    public void QueensTest1()
    {
        var (mb, board) = BuildBaseModel(Instances.SevenBySeven.Regions.Length);

        RunTest(mb, board, Instances.SevenBySeven.Regions, Instances.SevenBySeven.Solution);
    }
}
