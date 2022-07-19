using System.Linq;

using CSPSolver.common;
using CSPSolver.common.search;
using CSPSolver.common.variables;

namespace CSPSolver.Search.VariableOrdering
{
    public readonly struct Static : IVariableOrderingHeuristic
    {
        private readonly IDecisionVariable[] _variables;
        public Static(IDecisionVariable[] variables) => _variables = variables;

        public IDecisionVariable Next(in IModel model, in IState state)
        {
            var s = state;
            return _variables
                .Where(_ => !_.IsInstantiated(s))
                .FirstOrDefault();
        }
    }
}
