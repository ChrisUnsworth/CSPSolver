using CSPSolver.common.search;
using CSPSolver.Search.strategy;

namespace CSPSolver.Search
{
    public readonly struct SearchConfig
    {
        public IBranchStrategy Branching { get; }

        public SearchConfig(IBranchStrategy branching) => Branching = branching;

        public static SearchConfig Default() => 
            new SearchConfig(new BinaryBranching(new ArbitraryVariableOrdering(), new MinValueOrdering()));
    }
}
