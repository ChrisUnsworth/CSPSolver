using System.Collections.Generic;
using CSPSolver.common;
using CSPSolver.common.variables;

namespace CSPSolver.Constraint.Bool;

/// <summary>
/// Exactly `count` of the given bool vars are true, where `count` is itself an
/// int var rather than a fixed constant. Narrows in both directions: the
/// true/undecided split over the bools narrows count's own bounds, and count's
/// bounds narrow which bools must still be forced.
/// </summary>
public readonly struct CardinalityIntVar : IConstraint
{
    private readonly IBoolVar[] _vars;
    private readonly IIntVar _count;

    public CardinalityIntVar(IEnumerable<IBoolVar> vars, IIntVar count)
    {
        _vars = [.. vars];
        _count = count;
    }

    public IEnumerable<IVariable> Variables
    {
        get
        {
            foreach (var v in _vars) yield return v;
            yield return _count;
        }
    }

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

        var countChanged = _count.SetMin(state, areTrue);
        countChanged |= _count.SetMax(state, canBeTrue);

        if (_count.IsEmpty(state))
        {
            return countChanged
                ? [_count]
                : [];
        }

        var countMin = _count.GetDomainMin(state);
        var countMax = _count.GetDomainMax(state);

        if (canBeTrue != areTrue && canBeTrue == countMin)
        {
            return ForceTo(state, true, canBeTrue - areTrue, countChanged);
        }

        if (areTrue != canBeTrue && areTrue == countMax)
        {
            return ForceTo(state, false, canBeTrue - areTrue, countChanged);
        }

        return countChanged
            ? [_count]
            : [];
    }

    // maxForced is the number of undecided vars (canBeTrue - areTrue), the
    // most that could possibly change either way -- already-decided vars
    // that reach the matching SetValue call below are always a no-op.
    private IVariable[] ForceTo(IState state, bool value, int maxForced, bool countChanged)
    {
        var changed = new IVariable[maxForced + (countChanged ? 1 : 0)];
        var j = 0;

        for (int i = 0; i < _vars.Length; i++)
        {
            var canBe = value ? _vars[i].CanBeTrue(state) : _vars[i].CanBeFalse(state);
            if (canBe && _vars[i].SetValue(state, value))
            {
                changed[j++] = _vars[i];
            }
        }

        if (countChanged)
        {
            changed[j] = _count;
        }

        return changed;
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
            if (_count.TryGetValue(state, out int countVal))
            {
                return countVal == trueCount && _vars[0].MakeEmpty(state)
                    ? [_vars[0]]
                    : [];
            }

            return _count.RemoveValue(state, trueCount) ? [_count] : [];
        }

        if (!_count.TryGetValue(state, out int fixedCount))
        {
            return [];
        }

        var forceTo = trueCount switch
        {
            int i when i == fixedCount => true,
            int i when i == fixedCount - 1 => false,
            _ => (bool?)null
        };

        return forceTo.HasValue && undecided.SetValue(state, forceTo.Value)
            ? [undecided]
            : [];
    }

    public bool IsMet(IState state)
    {
        if (!_count.TryGetValue(state, out int countVal))
        {
            return false;
        }

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

        return trueCount == countVal;
    }

    public bool CanBeMet(IState state)
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

        return areTrue <= _count.GetDomainMax(state) && canBeTrue >= _count.GetDomainMin(state);
    }
}
