using System.Linq;
using System.Collections.Generic;
using System.Text;

using CSPSolver.common;
using CSPSolver.common.search;

namespace CSPSolver.Search.strategy
{
    public readonly struct  BinaryBranching : IBranchStrategy
    {
        private IVariableOrderingHeuristic _variableOrdering { get; }
        private IValueOrderingHeuristic _valueOrdering { get; }

        public BinaryBranching(IVariableOrderingHeuristic variableOrdering, IValueOrderingHeuristic valueOrdering) => 
            (_variableOrdering, _valueOrdering) = (variableOrdering, valueOrdering);

        public IEnumerable<IState> Branch(in IModel model, in IState state, IStatePool statePool)
        {
            var variable = _variableOrdering.Next(in model, state);
            var value = _valueOrdering.Order(model, state, variable).First();

            var without = statePool.Copy(state);
            var with = state;

            variable.RemoveValue(without, value);
            variable.SetValue(with, value);

            return new IState[] { with, without };
        }
    }
}
