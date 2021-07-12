using CSPSolver.common;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CSPSolver.State;

namespace CSPSolver.Search
{
    public class Search : IEnumerator<ISolution>
    {
        private readonly IModel _model;
        private readonly Stack<IState> _frontier;
        private readonly SearchConfig _searchConfig;
        private readonly StatePool _statePool;

        public Search(IModel model, SearchConfig? searchConfig = null)
        {
            _model = model;
            _frontier = new Stack<IState>();
            _frontier.Push(_model.State);
            _searchConfig = searchConfig ?? SearchConfig.Default();
            _statePool = new StatePool(_model.State);
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
                var before = _model.PrettyDomains();
                _model.propagate();
                var after = _model.PrettyDomains();
                if (_model.IsSolved())
                {
                    Current = new Solution(_statePool.Copy(_model.State));
                    return true;
                }
                else if (!_model.HasEmptyDomain())
                {
                    foreach (var state in _searchConfig.Branching.Branch(_model, _statePool).Reverse())
                    {
                        _frontier.Push(state);
                    }
                } else
                {
                    _statePool.Return(_model.State);
                }
            }
            
            return false;
        }
    }
}
