using System.Collections.Generic;

namespace CSPSolver.common.variables
{
    public interface IIntDomainVar : IIntVar
    {
        (uint[] domain, int min, int size) GetDomain(in IState state);

        bool SetDomain(IState state, in uint[] domain);

        bool DomainMinus(IState state, in uint[] domain);

        IEnumerable<int> EnumerateDomain(IState state);
    }
}
