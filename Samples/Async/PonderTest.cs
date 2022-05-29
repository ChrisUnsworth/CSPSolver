using System;
using System.Threading;

using CSPSolver.common;
using CSPSolver.Constraint.Delay;
using CSPSolver.Model;
using CSPSolver.Search;

namespace Samples.Async
{
    public static class PonderTest
    {
        public static async void Run()
        {
            var mb = new ModelBuilder();

            var x = mb.AddIntDomainVar(1, 100);

            mb.AddConstraint(new Ponder(7));

            var cts = new CancellationTokenSource();

            await foreach (var sol in ((Search)mb.Search()).GetSolutionsAsync(cts.Token))
            {
                Console.WriteLine($"x = {sol.GetValue(x)}");
            }
            /*
            var search = mb.AsyncSearch()
            {
                foreach
            });

            foreach (var solution in mb.Search())
            {
                Console.WriteLine($"{solution.GetValue(x)} + {solution.GetValue(y)} == 6");
            }
            */
        }
    }
}
