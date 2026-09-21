using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSPSolver.common;
using CSPSolver.Math.Divide;
using CSPSolver.State;
using CSPSolver.Variable;

namespace CSPSolverTests.Variables;

/// <summary>
/// Checks the quotient bounds a divide variable reports against the true
/// extremes of its domains. Bounds are what every propagator downstream reads,
/// so a loose bound weakens the whole model and a tight but wrong one loses
/// solutions.
/// </summary>
[TestClass]
public class DivideBoundsTests
{
    [TestMethod]
    public void MixedSignDivideReportsExactBounds()
    {
        var faults = new List<string>();
        var checkedPairs = 0;

        for (var xlo = -6; xlo <= 5; xlo++)
            for (var xhi = xlo; xhi <= Math.Min(xlo + 6, 6); xhi++)
                for (var ylo = -5; ylo <= 4; ylo++)
                    for (var yhi = ylo; yhi <= Math.Min(ylo + 5, 5); yhi++)
                    {
                        if (ylo == 0 && yhi == 0) continue;

                        var quotients = Quotients(xlo, xhi, ylo, yhi);
                        if (quotients.Count == 0) continue;

                        var (state, divide) = Divide(xlo, xhi, ylo, yhi);
                        checkedPairs++;

                        if (divide.GetDomainMin(state) != quotients.Min() ||
                            divide.GetDomainMax(state) != quotients.Max())
                        {
                            faults.Add(
                                $"x[{xlo},{xhi}] y[{ylo},{yhi}] reported " +
                                $"[{divide.GetDomainMin(state)},{divide.GetDomainMax(state)}] " +
                                $"but the true range is [{quotients.Min()},{quotients.Max()}]");
                        }
                    }

        Assert.AreNotEqual(0, checkedPairs);
        Assert.AreEqual(
            0,
            faults.Count,
            $"{faults.Count} of {checkedPairs} domain pairs report the wrong bounds:{Environment.NewLine}" +
            string.Join(Environment.NewLine, faults.Take(10)));
    }

    private static List<int> Quotients(int xlo, int xhi, int ylo, int yhi)
    {
        var quotients = new List<int>();

        for (var x = xlo; x <= xhi; x++)
            for (var y = ylo; y <= yhi; y++)
                if (y != 0) quotients.Add(x / y);

        return quotients;
    }

    private static (IState state, MixedSignDivideIntVar divide) Divide(int xlo, int xhi, int ylo, int yhi)
    {
        var sb = new StateBuilder();
        var v1 = new IntSmallDomainVar(xlo, xhi - xlo + 1, sb.AddDomain(xhi - xlo + 1));
        var v2 = new IntSmallDomainVar(ylo, yhi - ylo + 1, sb.AddDomain(yhi - ylo + 1));
        var state = sb.GetState();
        v1.Initialise(state);
        v2.Initialise(state);

        return (state, new MixedSignDivideIntVar(v1, v2));
    }
}