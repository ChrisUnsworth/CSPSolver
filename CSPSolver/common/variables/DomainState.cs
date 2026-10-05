namespace CSPSolver.common;

public enum DomainState
{
    Empty,
    Instantiated,
    Undecided,
}

public static class DomainStates
{
    public static DomainState Combine(IVariable v1, IVariable v2, IState state) =>
        Combine(v1.GetDomainState(state), v2.GetDomainState(state));

    public static DomainState Combine(DomainState s1, DomainState s2) =>
        (s1, s2) switch
        {
            (DomainState.Empty, _) => DomainState.Empty,
            (_, DomainState.Empty) => DomainState.Empty,
            (DomainState.Instantiated, DomainState.Instantiated) => DomainState.Instantiated,
            _ => DomainState.Undecided
        };

    public static DomainState Combine(IVariable[] vars, IState state)
    {
        var result = DomainState.Instantiated;

        for (var i = 0; i < vars.Length; i++)
        {
            switch (vars[i].GetDomainState(state))
            {
                case DomainState.Empty:
                    return DomainState.Empty;
                case DomainState.Undecided:
                    result = DomainState.Undecided;
                    break;
            }
        }

        return result;
    }
}
