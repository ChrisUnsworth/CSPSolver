using CSPSolver.common;

namespace CSPSolver.State
{
    public readonly struct StateRef : IStateRef
    {
        public StateRef(int idx, int offset) => (Idx, Offset) = (idx, offset);

        public int Idx { get; }
        public int Offset { get;  }

        public int UniqueIdentifier => (Idx << 5) + Offset;
    }
}
