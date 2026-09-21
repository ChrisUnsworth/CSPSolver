using CSPSolver.common;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using CSPSolver.State;
using CSPSolver.common.variables;

namespace CSPSolver.Search;

public class Search : IEnumerator<ISolution>, IEnumerable<ISolution>
{
    private readonly IModel _model;
    private readonly Stack<IState> _frontier;
    private readonly SearchConfig _searchConfig;
    private readonly StatePool _statePool;
    private bool _enumerated;

    public Search(IModelBuilder mb, SearchConfig? searchConfig = null)
    {
        _model = mb.GetModel();
        _statePool = new StatePool(mb.GetStateSize());
        var initialState = _statePool.Empty();
        _model.Initialise(initialState);
        _frontier = new Stack<IState>();
        PushIfFeasible(initialState);
        _searchConfig = searchConfig ?? SearchConfig.Default();
    }

    // A constraint built entirely from constants (e.g. TrueVar == FalseVar) can
    // never be pruned by propagation -- constants never change, so MakeEmpty on
    // them is a no-op, and there are no decision variables for HasEmptyDomain to
    // catch either. CanBeMet only needs checking once: if it's false on the
    // initial state, it stays false for the rest of the run.
    private void PushIfFeasible(IState state)
    {
        if (_model.Constraints.All(c => c.CanBeMet(state)))
        {
            _frontier.Push(state);
        }
        else
        {
            _statePool.Return(state);
        }
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
        PushIfFeasible(initialState);

        // Current bounds the objective, so leaving it set would start the next
        // run already limited by the previous run's best.
        Current = null;
        _enumerated = false;
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

                if (_model.Objective.IsEmpty(state))
                {
                    _statePool.Return(state);
                    break;
                }
            }

            _model.Propagate(state);
            if (_model.IsSolved(state))
            {
                Current = new Solution(_statePool.Copy(state));
                _statePool.Return(state);
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

        // Reaching here with a non-empty frontier means the objective bound broke
        // out above; whatever was left unexplored needs returning too.
        while (_frontier.Any())
        {
            _statePool.Return(_frontier.Pop());
        }

        return false;
    }

    // A search is expensive, so enumerating one twice is a mistake worth hearing
    // about rather than silently resuming from where the last pass stopped.
    public IEnumerator<ISolution> GetEnumerator()
    {
        if (_enumerated) throw new InvalidOperationException(SearchedAlready);

        _enumerated = true;
        return this;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    internal const string SearchedAlready =
        "A Search can only be enumerated once. Cache the solutions, or call Reset to search again.";
}