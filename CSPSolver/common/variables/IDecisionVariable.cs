using System;
using System.Collections.Generic;

namespace CSPSolver.common.variables
{
    public interface IDecisionVariable<T> : IDecisionVariable
    {
        IEnumerable<T> Domain(in IState _);

        bool SetMax(IState state, T max);

        bool SetMin(IState state, T min);
    }

    public interface IDecisionVariable : IVariable, IEquatable<IDecisionVariable>
    {
        public int DomainSize(in IState _);

        public int DomainSize();
    }
}
