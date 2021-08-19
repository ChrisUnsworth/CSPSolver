using CSPSolver.common;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using CSPSolver.State;
using CSPSolver.common.variables;

namespace CSPSolver.Search
{
    public class Search : IEnumerator<ISolution>, IEnumerable<ISolution>
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

        public void Reset()
        {
            while (_frontier.Any())
            {
                _statePool.Return(_frontier.Pop());
            }

            var initialState = _statePool.Empty();
            _model.Initialise(initialState);
            _frontier.Push(initialState);
        }

        public bool MoveNext() => Solve();

        private bool Solve()
        {
            while (_frontier.Any())
            {
                var state = _frontier.Pop();

                if (Current != null && _model.Objective != null)
                {
                    var objective = _model.Objective as IIntVar;
                    var best = Current.GetValue(objective);
                    if (_model.Maximise) objective.SetMin(state, best + 1);
                    else objective.SetMax(state, best - 1);

                    if (_model.Objective.IsEmpty(state)) break;
                }

                _model.Propagate(state);
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

        public IEnumerator<ISolution> GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => this;
    }
}
