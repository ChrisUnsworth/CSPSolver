using System;
using System.Collections.Generic;
using System.Linq;

using CSPSolver.common;
using CSPSolver.common.variables;

namespace CSPSolver.Math.Sum;

/// <summary>
/// The sum of an arbitrary number of int vars, generalising PlusIntVar's pairwise
/// bounds consistency to N terms: each var's own bound is narrowed from the
/// target range with the other vars' contributions subtracted out.
/// </summary>
public readonly struct SumOfIntVar : IIntVar, ICompoundVariable
{
    private readonly IIntVar[] _vars;
    private readonly int[] _extremes;

    public int Min { get; }

    public int Size { get; }

    public int Max { get; }

    public SumOfIntVar(IEnumerable<IIntVar> vars)
    {
        _vars = vars.ToArray();
        _extremes = new int[_vars.Length];
        Min = _vars.Sum(v => v.Min);
        Max = _vars.Sum(v => v.Max);
        Size = Max - Min + 1;
    }

    public int GetDomainMax(IState state) => _vars.Sum(v => v.GetDomainMax(state));

    public int GetDomainMin(IState state) => _vars.Sum(v => v.GetDomainMin(state));

    public void Initialise(IState state) { /* holds no state */ }

    public bool IsEmpty(IState state) => _vars.Any(v => v.IsEmpty(state));

    public bool IsInstantiated(IState state) => _vars.All(v => v.IsInstantiated(state));

    public bool RemoveValue(IState state, object value)
    {
        var undecided = -1;
        var sum = 0;

        for (var i = 0; i < _vars.Length; i++)
        {
            if (_vars[i].TryGetValue(state, out int v))
            {
                sum += v;
            }
            else if (undecided == -1)
            {
                undecided = i;
            }
            else
            {
                return false;
            }
        }

        return undecided switch
        {
            -1 when sum != (int)value => false,
            -1 => _vars[0].MakeEmpty(state),
            _ => _vars[undecided].RemoveValue(state, (int)value - sum)
        };
    }

    public bool SetMax(IState state, int max)
    {
        var sumMin = 0;

        for (var i = 0; i < _vars.Length; i++)
        {
            _extremes[i] = _vars[i].GetDomainMin(state);
            sumMin += _extremes[i];
        }

        var changed = false;
        for (var i = 0; i < _vars.Length; i++)
        {
            changed |= _vars[i].SetMax(state, max - (sumMin - _extremes[i]));
        }

        return changed;
    }

    public bool SetMin(IState state, int min)
    {
        var sumMax = 0;

        for (var i = 0; i < _vars.Length; i++)
        {
            _extremes[i] = _vars[i].GetDomainMax(state);
            sumMax += _extremes[i];
        }

        var changed = false;
        for (var i = 0; i < _vars.Length; i++)
        {
            changed |= _vars[i].SetMin(state, min - (sumMax - _extremes[i]));
        }

        return changed;
    }

    public bool SetValue(IState state, object value) => SetMax(state, (int)value) | SetMin(state, (int)value);

    public bool TryGetValue(IState state, out int value)
    {
        var sum = 0;

        foreach (var v in _vars)
        {
            if (!v.TryGetValue(state, out int val))
            {
                value = 0;
                return false;
            }

            sum += val;
        }

        value = sum;
        return true;
    }

    public Type VariableType() => typeof(int);

    public string PrettyDomain(IState state) => string.Join(" + ", _vars.Select(v => v.PrettyDomain(state)));

    public IEnumerable<IVariable> GetChildren() => _vars;
}
