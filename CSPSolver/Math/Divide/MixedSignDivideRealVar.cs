using System;
using System.Collections.Generic;

using static System.Math;

using CSPSolver.common;
using CSPSolver.common.variables;

namespace CSPSolver.Math.Divide
{
    public readonly struct MixedSignDivideRealVar : IRealVar, ICompoundVariable
    {
        private readonly IRealVar _v1;
        private readonly IRealVar _v2;

        public double Min { get; }

        public double Max { get; }

        public double Epsilon => Min(_v1.Epsilon, _v2.Epsilon);

        public MixedSignDivideRealVar(IRealVar v1, IRealVar v2)
        {
            _v1 = v1;
            _v2 = v2;
            Min = GetMin(v1.Min, v1.Max, v2.Min, v2.Max, Min(v1.Epsilon, v2.Epsilon));
            Max = GetMax(v1.Min, v1.Max, v2.Min, v2.Max, Min(v1.Epsilon, v2.Epsilon));
        }

        private (double v1Min, double v1Max, double v2Min, double v2Max) GetVariableDomainExtremes(IState state)
            => (_v1.GetDomainMin(state), _v1.GetDomainMax(state), _v2.GetDomainMin(state), _v2.GetDomainMax(state));

        public double GetDomainMax(in IState state)
            => GetMax(_v1.GetDomainMin(state), _v1.GetDomainMax(state), _v2.GetDomainMin(state), _v2.GetDomainMax(state), Epsilon);

        private static double GetMax(double v1Min, double v1Max, double v2Min, double v2Max, double epsilon)
        {
            double max = double.MinValue;

            if (v2Max > 0)
            {
                max = v1Max / Max(epsilon, v2Min);
                max = Max(max, v1Min / v2Max);
            }
            if (v2Min < 0) max = Max(max, v1Min / Min(-epsilon, v2Max));

            return max;
        }

        public double GetDomainMin(in IState state)
            => GetMin(_v1.GetDomainMin(state), _v1.GetDomainMax(state), _v2.GetDomainMin(state), _v2.GetDomainMax(state), Epsilon);

        private static double GetMin(double v1Min, double v1Max, double v2Min, double v2Max, double epsilon)
        {
            if (v2Max == 0 && v2Min == 0) return double.MaxValue;

            if (v2Min >= 0)
            {
                return v1Min >= 0
                    ? v1Min / v2Max
                    : v1Min / Max(1, v2Min);
            }

            if (v2Max <= 0) return v1Max < 0 ? v1Max / v2Min : v1Max / Min(-epsilon, v2Max);

            return Min(v1Max / -epsilon, v1Min / epsilon);
        }

        public void Initialise(IState state) { /* holds no state */ }

        public bool IsEmpty(in IState state) => _v1.IsEmpty(state) | _v2.IsEmpty(state);

        public bool IsInstantiated(in IState state) => _v1.IsInstantiated(state) & _v2.IsInstantiated(state);

        public bool RemoveValue(IState state, object value) =>
            (_v2.TryGetValue(state, out double v2) && _v1.RemoveValue(state, (double)value * v2))
          | (_v1.TryGetValue(state, out double v1) && (double)value != 0 && _v2.RemoveValue(state, v1 / (double)value));

        public bool SetMax(IState state, double max) =>
            SetMax(GetVariableDomainExtremes(state), state, max);

        private bool SetMax((double v1Min, double v1Max, double v2Min, double v2Max) e, IState state, double max)
        {
            var result = false;

            if (e.v2Min == 0)
            {
                result |= _v2.SetMin(state, Epsilon);
                e.v2Min = Epsilon;
            }

            if (e.v2Max == 0)
            {
                result |= _v2.SetMax(state, -Epsilon);
                e.v2Max = -Epsilon;
            }

            if (e.v2Min > 0) // Positive denominator
            {
                if (e.v1Min >= 0) // Positive numerator
                {
                    if (max <= 0) return _v1.SetMax(state, max) || result;
                    return _v1.SetMax(state, max * e.v2Max)
                         | (e.v1Min > 0 && _v2.SetMin(state, e.v1Min / max))
                         || result;
                }
                else if (e.v1Max < 0) // Negative numerator
                {
                    if (max >= 0) return result;
                    return _v1.SetMax(state, max * e.v2Max)
                         | _v2.SetMax(state, e.v1Min / max)
                         || result;
                }

                // Mixed sign numerator
                if (max > 0) return _v1.SetMax(state, max * e.v2Max) || result;
                if (max == 0) return _v1.SetMax(state, 0) || result;
                return _v1.SetMax(state, max * e.v2Max)
                     | _v2.SetMax(state, e.v1Min / max)
                     || result;
            }
            else if (e.v2Max < 0) // Negative denominator
            {
                if (e.v1Min >= 0) // Positive numerator
                {
                    if (max >= 0) return result;
                    return _v1.SetMax(state, max * e.v2Min)
                         | _v2.SetMin(state, e.v1Min / max)
                         || result;
                }
                else if (e.v1Max < 0) // Negative numerator
                {
                    return _v1.SetMin(state, max * e.v2Min)
                         | _v2.SetMax(state, e.v1Max / max)
                         || result;
                }

                // Mixed sign numerator

                if (max > 0) return _v1.SetMin(state, max * e.v2Max) || result;
                if (max == 0) return _v1.SetMin(state, 0) || result;
                return _v1.SetMin(state, max * e.v2Max)
                     | _v2.SetMin(state, e.v1Max / max)
                    || result;
            }

            // Mixed sign denominator

            if (max >= 0) return result;

            if (e.v1Min >= 0) // Positive numerator
            {
                return _v1.SetMin(state, max * -Epsilon)
                     | _v2.SetMax(state, e.v1Min / max)
                     || result;
            }
            else if (e.v1Max < 0) // Negative numerator
            {
                return _v1.SetMin(state, max * e.v2Min)
                     | _v2.SetMax(state, e.v1Max / max)
                     || result;
            }

            // Mixed sign numerator

            return result;
        }

        public bool SetMin(IState state, double min) =>
            SetMin(GetVariableDomainExtremes(state), state, min);

        private bool SetMin((double v1Min, double v1Max, double v2Min, double v2Max) e, IState state, double min)
        {
            var result = false;

            if (e.v2Min == 0)
            {
                result |= _v2.SetMin(state, Epsilon);
                e.v2Min = Epsilon;
            }

            if (e.v2Max == 0)
            {
                result |= _v2.SetMax(state, -Epsilon);
                e.v2Max = -Epsilon;
            }

            if (min < 0)
            {
                if (e.v1Min > 0 && e.v2Max < 0)
                {
                    result |= _v2.SetMin(state, e.v1Max / min);
                    result |= _v1.SetMin(state, min* e.v2Max);
                }
                else if (e.v1Max < 0 && e.v2Min > 0)
                {
                    if (e.v1Max < min) result |= _v2.SetMax(state, e.v1Max / min);
                    result |= _v1.SetMax(state, min * e.v2Max);
                }
            }
            else
            {
                if (e.v1Min > 0)
                {
                    result |= _v2.SetMin(state, 1);
                    result |= _v2.SetMax(state, Ceiling(e.v1Max / min));
                }
                else if (e.v1Max < 0)
                {
                    result |= _v2.SetMax(state, -1);
                    result |= _v2.SetMin(state, Floor(e.v1Min / min));
                }

                if (e.v2Min > 0)
                {
                    result |= _v1.SetMin(state, e.v2Min * min);
                }
                else if (e.v2Max < 0)
                {
                    result |= _v1.SetMax(state, e.v2Max * min);
                }
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
                value = v1 / v2;
                return true;
            }

            value = 0;
            return false;
        }

        public Type VariableType() => typeof(double);

        public string PrettyDomain(in IState state) => $"{_v1.PrettyDomain(state)} / {_v2.PrettyDomain(state)}";

        public IEnumerable<IVariable> GetChildren() => new IVariable[] { _v1, _v2 };
    }
}
