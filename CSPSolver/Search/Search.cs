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

        public Search(IModelBuilder mb, SearchConfig? searchConfig = null)
        {
            _model = mb.GetModel();
            _statePool = new StatePool(mb.GetStateSize());
            var initialState = _statePool.Empty();
            _model.Initialise(initialState);
            _frontier = new Stack<IState>();
            _frontier.Push(initialState);
            _searchConfig = searchConfig ?? SearchConfig.Default();
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
                var state = _frontier.Pop();
                _model.propagate(state);
                if (_model.IsSolved(state))
                {
                    Current = new Solution(_statePool.Copy(state));
                    return true;
                }
                else if (!_model.HasEmptyDomain(state))
                {
                    foreach (var branch in _searchConfig.Branching.Branch(_model, state, _statePool).Reverse())
                    {
                        _frontier.Push(branch);
                    }
                } else
                {
                    _statePool.Return(state);
                }
            }
            
            return false;
        }
    }
}
