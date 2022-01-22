using CSPSolver.common;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using CSPSolver.State;
using CSPSolver.common.variables;

namespace CSPSolver.Search
{
    public class DebugSearch : IEnumerator<ISolution>, IEnumerable<ISolution>
    {

        public readonly IModel _model;
        public readonly Stack<SearchTree> _frontier;
        public readonly SearchConfig _searchConfig;
        public readonly StatePool _statePool;
        public readonly SearchTree Root;

        public DebugSearch(IModelBuilder mb, SearchConfig? searchConfig = null)
        {
            _model = mb.GetModel();
            _statePool = new StatePool(mb.GetStateSize());
            var initialState = _statePool.Empty();
            _model.Initialise(initialState);
            Root = new SearchTree(initialState, null);
            _frontier = new Stack<SearchTree>();
            _frontier.Push(Root);
            _searchConfig = searchConfig ?? SearchConfig.Default();
        }

        public ISolution Current { get; private set; }

        object IEnumerator.Current => Current;

        public void Dispose() { }

        public void Reset() => _frontier.Push(Root);

        public bool MoveNext() => Solve();

        private bool Solve()
        {
            while (_frontier.Any())
            {
                var node = _frontier.Pop();

                if (Current != null && _model.Objective != null)
                {
                    var objective = _model.Objective as IIntVar;
                    var best = Current.GetValue(objective);
                    if (_model.Maximise) objective.SetMin(node.Before, best + 1);
                    else objective.SetMax(node.Before, best - 1);

                    if (_model.Objective.IsEmpty(node.Before)) break;
                }

                if (node.After != null) _statePool.Return(node.After);
                node.After = _statePool.Copy(node.Before);

                _model.Propagate(node.After);
                if (_model.IsSolved(node.After))
                {
                    Current = new Solution(_statePool.Copy(node.After));
                    return true;
                }
                else if (!_model.HasEmptyDomain(node.After))
                {
                    foreach (var branch in _searchConfig.Branching.Branch(_model, node.After, _statePool).Reverse())
                    {
                        _frontier.Push(node.AddChild(branch));
                    }
                }
            }

            return false;
        }

        public IEnumerator<ISolution> GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => this;
    }
}
