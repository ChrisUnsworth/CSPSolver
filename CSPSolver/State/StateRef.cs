using CSPSolver.common;

namespace CSPSolver.State;

public readonly struct StateRef : IStateRef
{
    public StateRef(int idx, int offset, int id = -1) => (Idx, Offset, Id) = (idx, offset, id);

    public int Idx { get; }
    public int Offset { get; }
    public int Id { get; }
}