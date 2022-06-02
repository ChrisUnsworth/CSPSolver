using System.Linq;

using CSPSolver.common;
using CSPSolver.common.search;
using CSPSolver.common.variables;

namespace CSPSolver.Search.VariableOrdering
{
    public readonly struct LargestDomain : IVariableOrderingHeuristic
    {
        public IVariable Next(in IModel model, in IState state)
        {
            var s = state;
            return model.Variables
                .Where(_ => !_.IsInstantiated(s))
                .OrderByDescending(_ => _.DomainSize(s))
                .FirstOrDefault();
            
        }
    }
}
