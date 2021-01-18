using CSPSolver.common;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSPSolver.State
{
    public readonly struct StateRef : IStateRef
    {
        public StateRef(int idx, int offset) => (Idx, Offset) = (idx, offset);

        public int Idx { get; }
        public int Offset { get;  }
    }
}
