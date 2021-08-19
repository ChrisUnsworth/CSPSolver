using System;
using System.Collections.Generic;
using System.Text;

namespace CSPSolver.common
{
    public interface IStateBuilder
    {
        IStateRef AddDomain(int size);

        IStateRef AddInt();
        IStateRef AddDouble();
        IStateRef AddFloat();
        IStateRef AddLong();

        IState GetState();
        int GetSize();
    }
}
