using System.Linq;

using CSPSolver.common;
using CSPSolver.common.search;
using CSPSolver.Search.VariableOrdering;

namespace CSPSolver.Model
{
    public static class VariableOrdering
    {
        public static IVariableOrderingHeuristic Default(IModel _) => new Arbitrary();

        public static IVariableOrderingHeuristic LargestDomain(IModel _) => new LargestDomain();

        public static IVariableOrderingHeuristic SmallestDomain(IModel _) => new SmallestDomain();

        public static IVariableOrderingHeuristic StaticLargestDomain(IModel m) =>
            new Static(m.Variables.OrderByDescending(_ => _.DomainSize()).ToArray());

        public static IVariableOrderingHeuristic StaticSmallestDomain(IModel m) =>
            new Static(m.Variables.OrderBy(_ => _.DomainSize()).ToArray());

        public static IVariableOrderingHeuristic MostConstrained(IModel m) =>
            new Static(m.Variables.OrderByDescending(_ => m.IsConstrainedBy(_).Count()).ToArray());

        public static IVariableOrderingHeuristic LeastConstrained(IModel m) =>
            new Static(m.Variables.OrderByDescending(_ => m.IsConstrainedBy(_).Count()).ToArray());

        public static IVariableOrderingHeuristic MostConnected(IModel m) =>
            new Static(m.Variables.OrderByDescending(_ => m.IsLinkedTo(_).Count()).ToArray());

        public static IVariableOrderingHeuristic LeastConnected(IModel m) =>
            new Static(m.Variables.OrderByDescending(_ => m.IsLinkedTo(_).Count()).ToArray());
    }
}
