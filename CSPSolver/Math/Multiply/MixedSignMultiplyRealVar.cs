using System;
using System.Collections.Generic;
using System.Linq;

using static System.Math;

using CSPSolver.common;
using CSPSolver.common.variables;

namespace CSPSolver.Math.Multiply
{
    public readonly struct MixedSignMultiplyRealVar: IRealVar, ICompoundVariable
    {
        private readonly IRealVar _v1;
        private readonly IRealVar _v2;

        public double Min { get; }

        public double Max { get; }

        public double Epsilon => Max(_v1.Epsilon, _v2.Epsilon);

        public MixedSignMultiplyRealVar(IRealVar v1, IRealVar v2)
        {
            _v1 = v1;
            _v2 = v2;
            var extremes = new double[] { v1.Max * v2.Max, v1.Max * v2.Min, v1.Min * v2.Max, v1.Min * v2.Min };
            Min = extremes.Min();
            Max = extremes.Max();
        }

        private double[] GetDomainExtremes(in IState state)
        {
            (double v1Min, double v1Max, double v2Min, double v2Max) = GetVariableDomainExtremes(state);
            return new double[] { v1Max * v2Max, v1Max * v2Min, v1Min * v2Max, v1Min * v2Min };
        }

        private (double v1Min, double v1Max, double v2Min, double v2Max) GetVariableDomainExtremes(in IState state)
            => (_v1.GetDomainMin(state), _v1.GetDomainMax(state), _v2.GetDomainMin(state), _v2.GetDomainMax(state));

        public double GetDomainMax(in IState state) => GetDomainExtremes(state).Max();

        public double GetDomainMin(in IState state) => GetDomainExtremes(state).Min();

        public void Initialise(IState state) { /* holds no state */ }

        public bool IsEmpty(in IState state) => _v1.IsEmpty(state) | _v2.IsEmpty(state);

        public bool IsInstantiated(in IState state) => _v1.IsInstantiated(state) & _v2.IsInstantiated(state);

        public bool RemoveValue(IState state, object value)
        {
            var result = false;
            var doubleVal = (double)value;
            if (_v2.TryGetValue(state, out double v2))
            {
                result = _v1.RemoveValue(state, doubleVal / v2);
            }

            if (_v1.TryGetValue(state, out double v1))
            {
                result |= _v2.RemoveValue(state, doubleVal / v1);
            }

            return result;
        }

        public bool SetMax(IState state, double max) =>
            SetMax(GetVariableDomainExtremes(state), state, max);

        private bool SetMax((double v1Min, double v1Max, double v2Min, double v2Max) e, IState state, double max)
        {
            var result = false;

            if (e.v1Min > 0) result = _v2.SetMax(state, max / e.v1Min);
            if (e.v2Min > 0) result |= _v1.SetMax(state, max / e.v2Min);

            if (e.v1Max < 0) result |= _v2.SetMin(state, max / e.v1Max);
            if (e.v2Max < 0) result |= _v1.SetMin(state, max / e.v2Max);

            if (max < 0)
            {
                if (e.v1Max == 0) result |= _v1.SetMax(state, -_v1.Epsilon);
                if (e.v2Max == 0) result |= _v2.SetMax(state, -_v2.Epsilon);
            }

            return result;
        }

        public bool SetMin(IState state, double min) =>
            SetMin(GetVariableDomainExtremes(state), state, min);

        private bool SetMin((double v1Min, double v1Max, double v2Min, double v2Max) e, IState state, double min)
        {
            var result = false;

            if (e.v1Min > 0) result = _v2.SetMin(state, min / e.v1Max);
            if (e.v2Min > 0) result |= _v1.SetMin(state, min / e.v2Max);

            if (e.v1Max < 0) result |= _v2.SetMax(state, min / e.v1Min);
            if (e.v2Max < 0) result |= _v1.SetMax(state, min / e.v2Min);

            if (min > 0)
            {
                if (e.v1Min == 0) result |= _v1.SetMin(state, _v1.Epsilon);
                if (e.v2Min == 0) result |= _v2.SetMin(state, _v2.Epsilon);
            }

            return result;
        }

        public bool SetValue(IState state, object value)
        {
            var extremes = GetVariableDomainExtremes(state);
            return SetMax(extremes, state, (double)value) | SetMin(extremes, state, (double)value);
        }

        public bool TryGetValue(in IState state, out double value)
        {
            if (_v1.TryGetValue(state, out double v1) & _v2.TryGetValue(state, out double v2))
            {
                value = v1 * v2;
                return true;
            }

            value = 0;
            return false;
        }

        public Type VariableType() => typeof(double);

        public string PrettyDomain(in IState state) => $"{_v1.PrettyDomain(state)} * {_v2.PrettyDomain(state)}";

        public IEnumerable<IVariable> GetChildren() => new IVariable[] { _v1, _v2 };
    }
}
