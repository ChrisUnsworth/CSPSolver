using System.Collections.Generic;

namespace CSPSolver.common.variables
{
    public interface ILongDomainVar : IIntVar
    {
        (ulong domain, int min, int size) GetDomain(IState state);

        bool SetDomain(IState state, ulong domain);

        bool DomainMinus(IState state, ulong domain);

        IEnumerable<int> EnumerateDomain(IState state);
    }
}
