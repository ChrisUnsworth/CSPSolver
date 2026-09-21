using System;
using System.Collections.Generic;
using System.Linq;

using static System.Math;

using CSPSolver.common;
using CSPSolver.common.variables;

namespace CSPSolver.Math.Multiply;

public readonly struct MixedSignMultiplyIntVar : IIntVar, ICompoundVariable
{
    private readonly IIntVar _v1;
    private readonly IIntVar _v2;

    public int Min { get; }

    public int Size { get; }

    public int Max { get; }

    public MixedSignMultiplyIntVar(IIntVar v1, IIntVar v2)
    {
        _v1 = v1;
        _v2 = v2;
        (Min, Max) = Bounds(v1.Min, v1.Max, v2.Min, v2.Max);
        Size = Max - Min + 1;
    }

    private (int v1Min, int v1Max, int v2Min, int v2Max) GetVariableDomainExtremes(IState state)
        => (_v1.GetDomainMin(state), _v1.GetDomainMax(state), _v2.GetDomainMin(state), _v2.GetDomainMax(state));

    public int GetDomainMax(IState state)
        => Bounds(_v1.GetDomainMin(state), _v1.GetDomainMax(state), _v2.GetDomainMin(state), _v2.GetDomainMax(state)).max;

    public int GetDomainMin(IState state)
        => Bounds(_v1.GetDomainMin(state), _v1.GetDomainMax(state), _v2.GetDomainMin(state), _v2.GetDomainMax(state)).min;

    /// <summary>
    /// A product is monotonic in each operand, so over a pair of ranges its
    /// extremes sit on the corners.
    /// </summary>
    private static (int min, int max) Bounds(int v1Min, int v1Max, int v2Min, int v2Max)
    {
        var min = int.MaxValue;
        var max = int.MinValue;

        Visit(v1Max * v2Max, ref min, ref max);
        Visit(v1Max * v2Min, ref min, ref max);
        Visit(v1Min * v2Max, ref min, ref max);
        Visit(v1Min * v2Min, ref min, ref max);

        return (min, max);
    }

    private static void Visit(int product, ref int min, ref int max)
    {
        if (product < min) min = product;
        if (product > max) max = product;
    }

    public void Initialise(IState state) { /* holds no state */ }

    public bool IsEmpty(IState state) => _v1.IsEmpty(state) | _v2.IsEmpty(state);

    public bool IsInstantiated(IState state) => _v1.IsInstantiated(state) & _v2.IsInstantiated(state);

    public bool RemoveValue(IState state, object value)
    {
        var result = false;
        var intVal = (int)value;
        if (_v2.TryGetValue(state, out int v2))
        {
            result = _v1.RemoveValue(state, intVal / v2);
        }

        if (_v1.TryGetValue(state, out int v1))
        {
            result |= _v2.RemoveValue(state, intVal / v1);
        }

        return result;
    }

    public bool SetMax(IState state, int max) =>
        SetMax(GetVariableDomainExtremes(state), state, max);

    private bool SetMax((int v1Min, int v1Max, int v2Min, int v2Max) extremes, IState state, int max)
    {
        var result = false;
        (int v1Min, int v1Max, int v2Min, int v2Max) = extremes;

        if (v1Min > 0) result = _v2.SetMax(state, max / v1Min);
        if (v2Min > 0) result |= _v1.SetMax(state, max / v2Min);

        if (v1Max < 0) result |= _v2.SetMin(state, max / v1Max);
        if (v2Max < 0) result |= _v1.SetMin(state, max / v2Max);

        if (max < 0)
        {
            if (v1Max == 0) result |= _v1.SetMax(state, -1);
            if (v2Max == 0) result |= _v2.SetMax(state, -1);
        }

        return result;
    }

    public bool SetMin(IState state, int min) =>
        SetMin(GetVariableDomainExtremes(state), state, min);

    private bool SetMin((int v1Min, int v1Max, int v2Min, int v2Max) extremes, IState state, int min)
    {
        var result = false;
        (int v1Min, int v1Max, int v2Min, int v2Max) = extremes;

        if (v1Min > 0) result = _v2.SetMin(state, (int)Ceiling((double)min / v1Max));
        if (v2Min > 0) result |= _v1.SetMin(state, (int)Ceiling((double)min / v2Max));

        if (v1Max < 0) result |= _v2.SetMax(state, (int)Floor((double)min / v1Min));
        if (v2Max < 0) result |= _v1.SetMax(state, (int)Floor((double)min / v2Min));

        if (min > 0)
        {
            if (v1Min == 0) result |= _v1.SetMin(state, 1);
            if (v2Min == 0) result |= _v2.SetMin(state, 1);
        }

        return result;
    }

    public bool SetValue(IState state, object value)
    {
        var extremes = GetVariableDomainExtremes(state);
        return SetMax(extremes, state, (int)value) | SetMin(extremes, state, (int)value);
    }

    public bool TryGetValue(IState state, out int value)
    {
        if (_v1.TryGetValue(state, out int v1) & _v2.TryGetValue(state, out int v2))
        {
            value = v1 * v2;
            return true;
        }

        value = 0;
        return false;
    }

    public Type VariableType() => typeof(int);

    public string PrettyDomain(IState state) => $"{_v1.PrettyDomain(state)} * {_v2.PrettyDomain(state)}";

    public IEnumerable<IVariable> GetChildren() => new IVariable[] { _v1, _v2 };
}