using System.Linq;
using System.Collections.Generic;

using CSPSolver.common;
using CSPSolver.common.search;

namespace CSPSolver.Search.strategy
{
    public readonly struct  BinaryBranching : IBranchStrategy
    {
        private readonly IVariableOrderingHeuristic _variableOrdering;
        private readonly IValueOrderingHeuristic _valueOrdering;

        public BinaryBranching(IVariableOrderingHeuristic variableOrdering, IValueOrderingHeuristic valueOrdering) => 
            (_variableOrdering, _valueOrdering) = (variableOrdering, valueOrdering);

        public IEnumerable<IState> Branch(in IModel model, in IState state, IStatePool statePool)
        {
            var variable = _variableOrdering.Next(in model, state);
            var (option, inverse) = _valueOrdering.Order(model, state, variable).First();

            var second = statePool.Copy(state);
            var first = state;

            option.Invoke(first);
            inverse.Invoke(second);

            return new IState[] { first, second };
        }
    }
}
