using System.Collections.Generic;

namespace CSPSolver.common.variables
{
    public interface IIntDomainVar : IIntVar
    {
        (uint[] domain, int min, int size) GetDomain(IState state);

        bool SetDomain(IState state, uint[] domain);

        bool DomainMinus(IState state, uint[] domain);

        IEnumerable<int> EnumerateDomain(IState state);
    }
}
