namespace CSPSolver.common.variables;

public interface IBoolVar : ISmallIntDomainVar, IVariable<bool>
{
    public bool IsTrue(IState state);

    public bool CanBeTrue(IState state);

    public bool IsFalse(IState state);

    public bool CanBeFalse(IState state);

    // Each implementation packs its domain differently (BoolVar uses 2 bits,
    // TrueVar/FalseVar use 1), so this reads state once and resolves the
    // three observable states directly, rather than deriving it generically
    // from IsTrue/CanBeTrue/IsFalse/CanBeFalse, which would cost the same
    // repeated reads this exists to avoid.
    public BoolState GetState(IState state);
}