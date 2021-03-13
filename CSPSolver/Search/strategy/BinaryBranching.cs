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

        public IEnumerable<IState> Branch(IModel model)
        {
            var variable = _variableOrdering.Next(model);
            var value = _valueOrdering.Order(model, variable).First();

            var without = model.State.Copy();
            var with = model.State;

            variable.RemoveValue(without, value);
            variable.SetValue(with, value);

            yield return with;
            yield return without;
        }
    }
}
