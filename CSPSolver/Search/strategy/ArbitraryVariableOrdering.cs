using CSPSolver.common;
using CSPSolver.common.search;

namespace CSPSolver.Search.strategy;

public readonly struct ArbitraryVariableOrdering : IVariableOrderingHeuristic
{
    public IVariable Next(in IModel model, in IState state)
    {
        foreach (var v in model.Variables)
        {
            if (v.GetDomainState(state) != DomainState.Instantiated) return v;
        }

        return null;
    }
}