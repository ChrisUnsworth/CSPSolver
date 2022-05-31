using CSPSolver.common.search;
using CSPSolver.Search.strategy;
using CSPSolver.Search.VariableOrdering;

namespace CSPSolver.Search
{
    public readonly struct SearchConfig
    {
        public IBranchStrategy Branching { get; }

        public SearchConfig(IBranchStrategy branching) => Branching = branching;

        public static SearchConfig Default() => 
            new(new BinaryBranching(new Arbitrary(), new MinValueOrdering()));
    }
}
