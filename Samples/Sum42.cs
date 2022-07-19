using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSPSolver.Model;

namespace Samples
{
    public static class Sum42
    {
        public static void Run()
        {
            var mb = new ModelBuilder();

            var v8 = mb.AddIntDomainVar(0, 5);
            var v10 = mb.AddIntDomainVar(0, 5);
            var v12 = mb.AddIntDomainVar(0, 5);
            var v14 = mb.AddIntDomainVar(0, 5);
            var v20 = mb.AddIntDomainVar(0, 5);

            mb.AddConstraint(v8 * 8 + v10 * 10 + v12 * 12 + v14 * 14 + v20 * 20 == 42);

            foreach (var solution in mb.Search())
            {
                var numberList = new List<int>();
                numberList.AddRange(Enumerable.Repeat(8, solution.GetValue(v8)));
                numberList.AddRange(Enumerable.Repeat(10, solution.GetValue(v10)));
                numberList.AddRange(Enumerable.Repeat(12, solution.GetValue(v12)));
                numberList.AddRange(Enumerable.Repeat(14, solution.GetValue(v14)));
                numberList.AddRange(Enumerable.Repeat(20, solution.GetValue(v20)));

                Console.WriteLine(string.Join(", ", numberList));
            }
        }
    }
}
