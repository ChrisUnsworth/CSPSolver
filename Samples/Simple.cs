﻿using System;

using CSPSolver.Model;

namespace Samples
{
    public static class Simple
    {
        public static void Rune()
        {
            var mb = new ModelBuilder();

            var x = mb.AddIntDomainVar(1, 5);
            var y = mb.AddIntDomainVar(1, 5);

            mb.AddConstraint(x + y == 6);
            mb.AddConstraint(x < y);

            //mb.AddObjective(x * y, maximise: true);

            foreach (var solution in mb.Search())
            {
                Console.WriteLine($"{solution.GetValue(x)} + {solution.GetValue(y)} == 6");
            }
        }
    }
}
