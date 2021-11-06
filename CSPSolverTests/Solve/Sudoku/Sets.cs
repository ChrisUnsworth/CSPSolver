using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPSolverTests.Solve.Sudoku
{
    public static class Sets
    {
        public static readonly IList<IList<(int x, int y)>> Rows =
            new List<IList<(int x, int y)>>
            {
                new List<(int x, int y)> { (0,0), (1,0), (2,0), (3,0), (4,0), (5,0), (6,0), (7,0), (8,0) },
                new List<(int x, int y)> { (0,1), (1,1), (2,1), (3,1), (4,1), (5,1), (6,1), (7,1), (8,1) },
                new List<(int x, int y)> { (0,2), (1,2), (2,2), (3,2), (4,2), (5,2), (6,2), (7,2), (8,2) },
                new List<(int x, int y)> { (0,3), (1,3), (2,3), (3,3), (4,3), (5,3), (6,3), (7,3), (8,3) },
                new List<(int x, int y)> { (0,4), (1,4), (2,4), (3,4), (4,4), (5,4), (6,4), (7,4), (8,4) },
                new List<(int x, int y)> { (0,5), (1,5), (2,5), (3,5), (4,5), (5,5), (6,5), (7,5), (8,5) },
                new List<(int x, int y)> { (0,6), (1,6), (2,6), (3,6), (4,6), (5,6), (6,6), (7,6), (8,6) },
                new List<(int x, int y)> { (0,7), (1,7), (2,7), (3,7), (4,7), (5,7), (6,7), (7,7), (8,7) },
                new List<(int x, int y)> { (0,8), (1,8), (2,8), (3,8), (4,8), (5,8), (6,8), (7,8), (8,8) }
            };

        public static IEnumerable<(int x, int y)> All => Rows.SelectMany(l => l);


        public static readonly IList<IList<(int x, int y)>> Columns =
            new List<IList<(int x, int y)>>
            {
                new List<(int x, int y)> { (0,0), (0,1), (0,2), (0,3), (0,4), (0,5), (0,6), (0,7), (0,8) },
                new List<(int x, int y)> { (1,0), (1,1), (1,2), (1,3), (1,4), (1,5), (1,6), (1,7), (1,8) },
                new List<(int x, int y)> { (2,0), (2,1), (2,2), (2,3), (2,4), (2,5), (2,6), (2,7), (2,8) },
                new List<(int x, int y)> { (3,0), (3,1), (3,2), (3,3), (3,4), (3,5), (3,6), (3,7), (3,8) },
                new List<(int x, int y)> { (4,0), (4,1), (4,2), (4,3), (4,4), (4,5), (4,6), (4,7), (4,8) },
                new List<(int x, int y)> { (5,0), (5,1), (5,2), (5,3), (5,4), (5,5), (5,6), (5,7), (5,8) },
                new List<(int x, int y)> { (6,0), (6,1), (6,2), (6,3), (6,4), (6,5), (6,6), (6,7), (6,8) },
                new List<(int x, int y)> { (7,0), (7,1), (7,2), (7,3), (7,4), (7,5), (7,6), (7,7), (7,8) },
                new List<(int x, int y)> { (8,0), (8,1), (8,2), (8,3), (8,4), (8,5), (8,6), (8,7), (8,8) }
            };


        public static readonly IList<IList<(int x, int y)>> Squares =
            new List<IList<(int x, int y)>>
            {
                new List<(int x, int y)> { (0, 0), (1, 0), (2, 0), (0, 1), (1, 1), (2, 1), (0, 2), (1, 2), (2, 2) },
                new List<(int x, int y)> { (3, 0), (4, 0), (5, 0), (3, 1), (4, 1), (5, 1), (3, 2), (4, 2), (5, 2) },
                new List<(int x, int y)> { (6, 0), (7, 0), (8, 0), (6, 1), (7, 1), (8, 1), (6, 2), (7, 2), (8, 2) },
                new List<(int x, int y)> { (0, 3), (1, 3), (2, 3), (0, 4), (1, 4), (2, 4), (0, 5), (1, 5), (2, 5) },
                new List<(int x, int y)> { (3, 3), (4, 3), (5, 3), (3, 4), (4, 4), (5, 4), (3, 5), (4, 5), (5, 5) },
                new List<(int x, int y)> { (6, 3), (7, 3), (8, 3), (6, 4), (7, 4), (8, 4), (6, 5), (7, 5), (8, 5) },
                new List<(int x, int y)> { (0, 6), (1, 6), (2, 6), (0, 7), (1, 7), (2, 7), (0, 8), (1, 8), (2, 8) },
                new List<(int x, int y)> { (3, 6), (4, 6), (5, 6), (3, 7), (4, 7), (5, 7), (3, 8), (4, 8), (5, 8) },
                new List<(int x, int y)> { (6, 6), (7, 6), (8, 6), (6, 7), (7, 7), (8, 7), (6, 8), (7, 8), (8, 8) }
            };

        public static IList<(int x, int y)> RowContaining((int x, int y) p) => Rows[p.y];
        public static IList<(int x, int y)> ColumnContaining((int x, int y) p) => Columns[p.x];
        public static IList<(int x, int y)> SquareContaining((int x, int y) p) => Squares[(p.x / 3) + (p.y / 3 * 3)];

        public static IList<IList<(int x, int y)>> ContainingSets((int x, int y) p) => new List<IList<(int x, int y)>> { RowContaining(p), ColumnContaining(p), SquareContaining(p) };
    }
}
