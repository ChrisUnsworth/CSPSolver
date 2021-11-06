using CSPSolver.common;
using System.Collections.Generic;
using System.Linq;

namespace CSPSolver.Search
{
    public class SearchTree
    {
        public SearchTree(IState state, SearchTree parent)
        {
            Before = state;
            Parent = parent;
        }
        
        public SearchTree Parent { get; }

        public List<SearchTree> Children { get; } = new List<SearchTree>();

        public IState Before { get; }

        public IState After { get; set; }

        public SearchTree AddChild(IState state)
        {
            var child = Children.FirstOrDefault(c => c.Before.IsSameAs(state));

            if (child == null)
            {
                child = new SearchTree(state, this);
                Children.Add(child);
            }            

            return child;
        }
    }
}
