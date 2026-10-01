using System.Linq;

using BenchmarkDotNet.Attributes;

using CSPSolver.Model;

using static CSPSolver.Model.ModelConstraint;

namespace PerformanceTests.Benchmarks;

/// <summary>
/// 9x9 Sudoku modelled with AllDiff over rows, columns and boxes.
/// </summary>
[MemoryDiagnoser]
public class SudokuBenchmarks
{
    [Params("Easy", "Hard")]
    public string Puzzle { get; set; }

    private static readonly int[,] Easy =
    {
        { 3, 8, 2, 9, 0, 0, 0, 0, 1 },
        { 0, 0, 0, 0, 0, 0, 0, 5, 2 },
        { 0, 1, 0, 0, 2, 7, 3, 0, 0 },
        { 0, 0, 0, 0, 4, 0, 0, 2, 7 },
        { 8, 0, 0, 2, 0, 9, 0, 0, 5 },
        { 2, 4, 0, 0, 6, 0, 0, 0, 0 },
        { 0, 0, 8, 4, 7, 0, 0, 1, 0 },
        { 5, 2, 0, 0, 0, 0, 0, 0, 0 },
        { 7, 0, 0, 0, 0, 8, 2, 9, 4 }
    };

    // Arto Inkala's "world's hardest sudoku".
    private static readonly int[,] Hard =
    {
        { 8, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 3, 6, 0, 0, 0, 0, 0 },
        { 0, 7, 0, 0, 9, 0, 2, 0, 0 },
        { 0, 5, 0, 0, 0, 7, 0, 0, 0 },
        { 0, 0, 0, 0, 4, 5, 7, 0, 0 },
        { 0, 0, 0, 1, 0, 0, 0, 3, 0 },
        { 0, 0, 1, 0, 0, 0, 0, 6, 8 },
        { 0, 0, 8, 5, 0, 0, 0, 1, 0 },
        { 0, 9, 0, 0, 0, 0, 4, 0, 0 }
    };

    private static ModelBuilder BuildModel(int[,] givens)
    {
        var mb = new ModelBuilder();
        var vars = mb.AddIntVarArray(1, 9, 9 * 9);
        ModelIntVar Cell(int row, int col) => vars[row * 9 + col];

        for (int i = 0; i < 9; i++)
        {
            mb.AddConstraint(AllDiff(Enumerable.Range(0, 9).Select(j => Cell(i, j))));
            mb.AddConstraint(AllDiff(Enumerable.Range(0, 9).Select(j => Cell(j, i))));
            mb.AddConstraint(AllDiff(Enumerable.Range(0, 9).Select(j => Cell(i / 3 * 3 + j / 3, i % 3 * 3 + j % 3))));
        }

        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (givens[i, j] != 0) mb.AddConstraint(Cell(i, j) == givens[i, j]);
            }
        }

        return mb;
    }

    // Enumerates the whole tree so the time includes proving the solution unique.
    [Benchmark]
    public int Solve() => BuildModel(Puzzle == "Hard" ? Hard : Easy).Search().Count();
}
