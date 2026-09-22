using System;
using System.Collections.Generic;
using System.Linq;

using CSPSolver.common;
using CSPSolver.common.variables;

namespace CSPSolver.Math.Sum;

/// <summary>
/// The sum of an arbitrary number of real vars, generalising PlusRealVar's
/// pairwise bounds consistency to N terms the same way SumOfIntVar does for ints.
/// </summary>
public readonly struct SumOfRealVar : IRealVar, ICompoundVariable
{
    private readonly IRealVar[] _vars;
    private readonly double[] _extremes;

    public double Min { get; }

    public double Max { get; }

    public double Epsilon => _vars.Min(v => v.Epsilon);

    public SumOfRealVar(IEnumerable<IRealVar> vars)
    {
        _vars = vars.ToArray();
        _extremes = new double[_vars.Length];
        Min = _vars.Sum(v => v.Min);
        Max = _vars.Sum(v => v.Max);
    }

    public double GetDomainMax(IState state) => _vars.Sum(v => v.GetDomainMax(state));

    public double GetDomainMin(IState state) => _vars.Sum(v => v.GetDomainMin(state));

    public void Initialise(IState state) { /* holds no state */ }

    public bool IsEmpty(IState state) => _vars.Any(v => v.IsEmpty(state));

    public bool IsInstantiated(IState state) => _vars.All(v => v.IsInstantiated(state));

    public bool RemoveValue(IState state, object value)
    {
        var undecided = -1;
        var sum = 0d;

        for (var i = 0; i < _vars.Length; i++)
        {
            if (_vars[i].TryGetValue(state, out double v))
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

        return undecided != -1 && _vars[undecided].RemoveValue(state, (double)value - sum);
    }

    public bool SetMax(IState state, double max)
    {
        var sumMin = 0d;

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

    public bool SetMin(IState state, double min)
    {
        var sumMax = 0d;

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

    public bool SetValue(IState state, object value) => SetMax(state, (double)value) | SetMin(state, (double)value);

    public bool TryGetValue(IState state, out double value)
    {
        var sum = 0d;

        foreach (var v in _vars)
        {
            if (!v.TryGetValue(state, out double val))
            {
                value = 0;
                return false;
            }

            sum += val;
        }

        value = sum;
        return true;
    }

    public Type VariableType() => typeof(double);

    public string PrettyDomain(IState state) => string.Join(" + ", _vars.Select(v => v.PrettyDomain(state)));

    public IEnumerable<IVariable> GetChildren() => _vars;
}
