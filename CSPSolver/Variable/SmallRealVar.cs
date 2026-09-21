using System;

using CSPSolver.common;
using CSPSolver.common.variables;

using static System.Math;

namespace CSPSolver.Variable;

public readonly struct SmallRealVar : IRealVar
{
    public double Min { get; }

    public double Max { get; }

    public double Epsilon { get; }

    public IStateRef MinStateRef { get; }
    public IStateRef MaxStateRef { get; }

    public SmallRealVar(double min, IStateRef minStateRef, double max, IStateRef maxStateRef, int decimalPlaces)
    {
        Min = min;
        MinStateRef = minStateRef;
        Max = max;
        MaxStateRef = maxStateRef;
        Epsilon = Pow(10, -decimalPlaces); ;
    }

    public double GetDomainMax(IState state) => Round(state.GetInt(MaxStateRef) * Epsilon, (int)Abs(Log10(Epsilon)));

    public double GetDomainMin(IState state) => Round(state.GetInt(MinStateRef) * Epsilon, (int)Abs(Log10(Epsilon)));

    public void Initialise(IState state)
    {
        state.SetInt(MaxStateRef, AsFloorInt(Max));
        state.SetInt(MinStateRef, AsCeilInt(Min));
    }

    public bool IsEmpty(IState state) => state.GetInt(MaxStateRef) < state.GetInt(MinStateRef);

    public bool IsInstantiated(IState state) => state.GetInt(MaxStateRef) == state.GetInt(MinStateRef);

    public string PrettyDomain(IState state) => $"{GetDomainMin(state)} ... {GetDomainMax(state)}";

    public bool RemoveValue(IState state, object value)
    {
        int val = AsInt((double)value);
        var result = false;
        var max = state.GetInt(MaxStateRef);
        var min = state.GetInt(MinStateRef);

        if (val == max)
        {
            state.SetInt(MaxStateRef, max - 1);
            result = true;
        }

        if (val == min)
        {
            state.SetInt(MinStateRef, min + 1);
            result = true;
        }

        return result;
    }

    public bool SetMax(IState state, double max)
    {
        // Floor, not nearest: rounding a shrinking max up onto the grid could
        // keep a point that's actually above the real bound just narrowed to.
        var maxInt = AsFloorInt(max);

        if (maxInt < state.GetInt(MaxStateRef))
        {
            state.SetInt(MaxStateRef, maxInt);
            return true;
        }

        return false;
    }

    public bool SetMin(IState state, double min)
    {
        // Ceiling, not nearest: rounding a growing min down onto the grid could
        // keep a point that's actually below the real bound just narrowed to.
        var minInt = AsCeilInt(min);

        if (minInt > state.GetInt(MinStateRef))
        {
            state.SetInt(MinStateRef, minInt);
            return true;
        }

        return false;
    }

    public bool SetValue(IState state, object value) => 
        SetMin(state, (double)value) | SetMax(state, (double)value);

    public bool TryGetValue(IState state, out double value)
    {
        value = GetDomainMax(state);
        return value == GetDomainMin(state);
    }

    public Type VariableType() => typeof(double);

    private int AsInt(double val) => (int)Round((double)val / Epsilon);

    // val / Epsilon lands a tick off the intended integer for values that are
    // exact grid points (e.g. 6.6 / 0.1 == 65.99999999999999), so floor/ceiling
    // alone would knock a real grid point off the domain. Nudging by a tolerance
    // tiny next to a single grid step corrects that without blunting the
    // directional rounding true non-grid values still need.
    private int AsFloorInt(double val) => (int)Floor(val / Epsilon + 1e-9);

    private int AsCeilInt(double val) => (int)Ceiling(val / Epsilon - 1e-9);
}
