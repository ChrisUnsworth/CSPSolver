namespace CSPSolver.common.variables;

/// <summary>
/// A snapshot of an int var's bounds and state, read in one pass so callers that need more than one of them
/// don't walk the variable (or a compound variable's children) repeatedly.
/// When State is Empty, Min and Max match what GetDomainMin and GetDomainMax return for that empty domain.
/// </summary>
public readonly record struct IntBounds(int Min, int Max, DomainState State);
