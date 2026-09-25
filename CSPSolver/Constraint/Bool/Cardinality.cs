using System.Collections.Generic;

using CSPSolver.common;
using CSPSolver.common.variables;

namespace CSPSolver.Constraint.Bool;

/// <summary>
/// Exactly `count` of the given bool vars are true. Propagates in a single
/// O(N) pass over the group: once enough are already true to reach the count,
/// every remaining undecided var is forced false; once there aren't enough
/// undecided vars left to reach the count, every remaining undecided var is
/// forced true.
/// </summary>
public readonly struct Cardinality : IConstraint
{
    private readonly IBoolVar[] _vars;
    private readonly int _invCount;
    private readonly int _count;

    /// <summary>
    /// Exactly `count` of the given bool vars are true. Propagates in a single
    /// O(N) pass over the group: once enough are already true to reach the count,
    /// every remaining undecided var is forced false; once there aren't enough
    /// undecided vars left to reach the count, every remaining undecided var is
    /// forced true.
    /// </summary>
    public Cardinality(IEnumerable<IBoolVar> vars, int count)
    {
        _count = count;
        _vars = [.. vars];
        _invCount = _vars.Length - count;
    }

    public IEnumerable<IVariable> Variables => _vars;

    public IEnumerable<IVariable> Propagate(IState state)
    {
        var areTrue = 0;
        var canBeTrue = 0;

        for (int i = 0; i < _vars.Length; i++)
        {
            switch (_vars[i].GetState(state))
            {
                case BoolState.True:
                    areTrue++;
                    canBeTrue++;
                    break;
                case BoolState.Undecided:
                    canBeTrue++;
                    break;
            }
        }

        if (canBeTrue < _count || areTrue > _count)
        {
            return _vars[0].MakeEmpty(state)
                ? [_vars[0]]
                : [];
        }

        if (canBeTrue == _count && areTrue < _count)
        {
            var changed = new IVariable[canBeTrue - areTrue];
            var j = 0;
            for (int i = 0; i < _vars.Length; i++)
            {
                if (_vars[i].CanBeTrue(state) && _vars[i].SetValue(state, true))
                {
                    changed[j++] = _vars[i];
                }
            }

            return changed;
        }

        if (areTrue == _count && canBeTrue > _count)
        {
            var changed = new IVariable[canBeTrue - areTrue];
            var j = 0;
            for (int i = 0; i < _vars.Length; i++)
            {
                if (_vars[i].CanBeFalse(state) && _vars[i].SetValue(state, false))
                {
                    changed[j++] = _vars[i];
                }
            }

            return changed;
        }

        return [];
    }

    public IEnumerable<IVariable> NegativePropagate(IState state)
    {
        var trueCount = 0;
        IBoolVar undecided = null;
        for (int i = 0; i < _vars.Length; i++)
        {
            switch (_vars[i].GetState(state))
            {
                case BoolState.True:
                    trueCount++;
                    break;
                case BoolState.Undecided:
                    if (undecided != null)
                    {
                        return [];
                    }

                    undecided = _vars[i];
                    break;
            }
        }

        if (undecided == null)
        {
            return trueCount == _count && _vars[0].MakeEmpty(state)
                ? [_vars[0]]
                : [];
        }

        var forceTo = trueCount switch
        {
            int i when i == _count => true,
            int i when i == _count - 1 => false,
            _ => (bool?)null
        };

        return forceTo.HasValue && undecided.SetValue(state, forceTo.Value)
            ? [undecided]
            : [];
    }

    public bool IsMet(IState state)
    {
        var trueCount = 0;
        for (int i = 0; i < _vars.Length; i++)
        {
            switch (_vars[i].GetState(state))
            {
                case BoolState.True:
                    trueCount++;
                    break;
                case BoolState.Undecided:
                    return false;
            }
        }

        return trueCount == _count;
    }

    public bool CanBeMet(IState state)
    {
        var trueCount = 0;
        var falseCount = 0;
        for (int i = 0; i < _vars.Length; i++)
        {
            switch (_vars[i].GetState(state))
            {
                case BoolState.True:
                    trueCount++;
                    break;
                case BoolState.False:
                    falseCount++;
                    break;
                case BoolState.Undecided:
                    trueCount++;
                    falseCount++;
                    break;
            }
        }

        return trueCount >= _count && falseCount >= _invCount;
    }
}