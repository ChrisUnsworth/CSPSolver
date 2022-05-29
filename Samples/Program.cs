using System;

using CSPSolver.Model;
using Samples.Async;
using Samples.Misc;

namespace Samples
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Action[] samples  = new Action[]
            {
                () => Test1.Run(),
                () => PonderTest.Run()
            };

            foreach (var sample in samples) sample.Invoke();
        }
    }
}
