﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CSPSolver.common
{
    public interface IIntVar : IVariable<int>
    {
        int GetDomainMax(IState state);

        int GetDomainMin(IState state);

        bool SetMax(IState state, int max);

        bool SetMin(IState state, int min);

        int Min { get; }

        int Size { get; }

        int Max { get; }
    }
}
