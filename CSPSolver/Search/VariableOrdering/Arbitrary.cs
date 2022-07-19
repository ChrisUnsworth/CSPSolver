using System.Linq;

using CSPSolver.common;
using CSPSolver.common.search;
using CSPSolver.common.variables;

namespace CSPSolver.Search.VariableOrdering
{
    public readonly struct Arbitrary : IVariableOrderingHeuristic
    {
        public IVariable Next(in IModel model, in IState state)
        {
            foreach (var v in model.Variables)
            {
                if (!v.IsInstantiated(state)) return v;
            }

            return null;
        }

        IDecisionVariable IVariableOrderingHeuristic.Next(in IModel model, in IState state)
        {
            var s = state;
            return model.Variables.First(v => !v.IsInstantiated(s));;
        }
    }
}
