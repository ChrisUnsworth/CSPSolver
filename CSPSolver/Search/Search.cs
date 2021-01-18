using CSPSolver.common;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CSPSolver.Search
{
    public class Search : IEnumerator<ISolution>
    {
        private IModel _model;
        private Stack<IState> _frontier;
        private SearchConfig _searchConfig;

        public Search(IModel model, SearchConfig searchConfig = null)
        {
            _model = model;
            _frontier = new Stack<IState>();
            _frontier.Push(_model.State);
            _searchConfig = searchConfig ?? new SearchConfig();
        }

        public ISolution Current { get; private set; }

        object IEnumerator.Current => Current;

        public void Dispose() { }

        public void Reset() => throw new NotImplementedException();

        public bool MoveNext() => Solve();

        private bool Solve()
        {
            while (_frontier.Any())
            {
                _model.State = _frontier.Pop();
                _model.propagate();
                if (_model.IsSolved())
                {
                    Current = new Solution(_model.State.Copy());
                    return true;
                }

                if (!_model.HasEmptyDomain()) Branch();
            }
            
            return false;
        }

        private void Branch()
        {
            var variable = _searchConfig.VariableOrderingHeuristic.Invoke(_model);
            var value = _searchConfig.ValueOrderingHeuristic.Invoke(_model, variable);
        }
    }
}
