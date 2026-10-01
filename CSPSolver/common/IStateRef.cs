namespace CSPSolver.common;

public interface IStateRef
{
    /// <summary>Unique per StateBuilder, so variables can be indexed by it; -1 if not issued by one.</summary>
    int Id { get; }
}