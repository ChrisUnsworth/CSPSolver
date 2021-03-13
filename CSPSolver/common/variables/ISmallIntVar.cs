using System.Collections.Generic;

namespace CSPSolver.common.variables
{
    public interface ISmallIntVar: IIntVar
    {
        (int domain, int min, int size) GetDomain(IState state);

        void SetDomain(IState state, int domain);

        IEnumerable<int> EnumerateDomain(IState state);
    }
}
