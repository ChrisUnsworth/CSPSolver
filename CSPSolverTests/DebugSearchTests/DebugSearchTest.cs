using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSPSolver.State;
using CSPSolver.Model;
using CSPSolver.Search;
using CSPSolverTests.Solve.Sudoku;

using static CSPSolver.Model.ModelConstraint;

namespace CSPSolverTests.DebugSearchTests
{
    [TestClass]
    public class DebugSearchTest
    {
        private static (ModelBuilder mb, ModelIntVar[,] varMatrix) BuildBaseModel()
        {

            var mb = new ModelBuilder(new StateBuilder());

            var vars = mb.AddIntVarArray(1, 9, 9 * 9);

            var varMatrix = new ModelIntVar[9, 9];

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    varMatrix[i, j] = vars[i * 9 + j];
                }
            }

            foreach (var row in Sets.Rows)
            {
                mb.AddConstraint(AllDiff(row.Select(c => varMatrix[c.x, c.y])));
            }

            foreach (var column in Sets.Columns)
            {
                mb.AddConstraint(AllDiff(column.Select(c => varMatrix[c.x, c.y])));
            }

            foreach (var square in Sets.Squares)
            {
                mb.AddConstraint(AllDiff(square.Select(c => varMatrix[c.x, c.y])));
            }

            return (mb, varMatrix);
        }

        private static void RunTest(ModelBuilder mb, ModelIntVar[,] varMatrix, int[,] input, int[,] solution)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (input[i, j] != 0) mb.AddConstraint(varMatrix[i, j] == input[i, j]);
                }
            }

            var search = new DebugSearch(mb);

            var count = 0;

            while (search.MoveNext())
            {
                var result = search.Current;
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        Assert.AreEqual(solution[i, j], result.GetValue(varMatrix[i, j]));
                    }
                }

                count++;
            }

            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void SudokuTest1()
        {
            var (mb, varMatrix) = BuildBaseModel();

            var data = new int[,]
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

            var expected = new int[,]
            {
                { 3, 8, 2, 9, 5, 4, 7, 6, 1 },
                { 6, 9, 7, 3, 8, 1, 4, 5, 2 },
                { 4, 1, 5, 6, 2, 7, 3, 8, 9 },
                { 1, 5, 6, 8, 4, 3, 9, 2, 7 },
                { 8, 7, 3, 2, 1, 9, 6, 4, 5 },
                { 2, 4, 9, 7, 6, 5, 1, 3, 8 },
                { 9, 3, 8, 4, 7, 2, 5, 1, 6 },
                { 5, 2, 4, 1, 9, 6, 8, 7, 3 },
                { 7, 6, 1, 5, 3, 8, 2, 9, 4 }
            };

            RunTest(mb, varMatrix, data, expected);
        }
    }
}
