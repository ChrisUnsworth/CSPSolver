using System;
using System.Collections.Generic;

using static System.Math;

using CSPSolver.common;
using CSPSolver.common.variables;

namespace CSPSolver.Math.Divide
{
    public readonly struct MixedSignDivideIntVar : IIntVar, ICompoundVariable
    {
        private readonly IIntVar _v1;
        private readonly IIntVar _v2;

        public int Min { get; }

        public int Size { get; }

        public int Max { get; }

        public MixedSignDivideIntVar(IIntVar v1, IIntVar v2)
        {
            _v1 = v1;
            _v2 = v2;
            Min = GetMin(v1.Min, v1.Max, v2.Min, v2.Max);
            Max = GetMax(v1.Min, v1.Max, v2.Min, v2.Max);
            Size = Max - Min + 1;
        }

        private (int v1Min, int v1Max, int v2Min, int v2Max) GetVariableDomainExtremes(in IState state)
            => (_v1.GetDomainMin(state), _v1.GetDomainMax(state), _v2.GetDomainMin(state), _v2.GetDomainMax(state));

        public int GetDomainMax(in IState state)
            => GetMax(_v1.GetDomainMin(state), _v1.GetDomainMax(state), _v2.GetDomainMin(state), _v2.GetDomainMax(state));

        private static int GetMax(int v1Min, int v1Max, int v2Min, int v2Max)
        {
            int max = int.MinValue;

            if (v2Max > 0)
            {
                max = v1Max / Max(1, v2Min);
                max = Max(max, v1Min / v2Max);
            }
            if (v2Min < 0) max = Max(max, v1Min / Min(-1, v2Max));

            return max;
        }

        public int GetDomainMin(in IState state)
            => GetMin(_v1.GetDomainMin(state), _v1.GetDomainMax(state), _v2.GetDomainMin(state), _v2.GetDomainMax(state));

        private static int GetMin(int v1Min, int v1Max, int v2Min, int v2Max)
        {
            if (v2Max == 0 && v2Min == 0) return int.MaxValue;

            if (v2Min >= 0)
            {
                return v1Min >= 0
                    ? v1Min / v2Max
                    : v1Min / Max(1, v2Min);
            }

            if (v2Max <= 0) return v1Max < 0 ? v1Max / v2Min : v1Max / Min(-1, v2Max);

            return Min(v1Max / -1, v1Min / 1);
        }

        public void Initialise(IState state) { /* holds no state */ }

        public bool IsEmpty(in IState state) => _v1.IsEmpty(state) | _v2.IsEmpty(state);

        public bool IsInstantiated(in IState state) => _v1.IsInstantiated(state) & _v2.IsInstantiated(state);

        public bool RemoveValue(IState state, object value) =>
            (_v2.TryGetValue(state, out int v2) && _v1.RemoveValue(state, (int)value * v2))
          | (_v1.TryGetValue(state, out int v1) && (int)value != 0 && _v2.RemoveValue(state, v1 / (int)value));

        public bool SetMax(IState state, int max) =>
            SetMax(GetVariableDomainExtremes(state), state, max);

        private bool SetMax((int v1Min, int v1Max, int v2Min, int v2Max) extremes, IState state, int max)
        {
            var result = false;
            (int v1Min, int v1Max, int v2Min, int v2Max) = extremes;

            if (v2Min == 0)
            {
                result |= _v2.SetMin(state, 1);
                v2Min++;
            }

            if (v2Max == 0)
            {
                result |= _v2.SetMax(state, -1);
                v2Max--;
            }

            if (v2Min > 0) // Positive denominator
            {
                if (v1Min >= 0) // Positive numerator
                {
                    if (max <= 0) return _v1.SetMax(state, max) || result;
                    return _v1.SetMax(state, (max + 1) * v2Max - 1)
                         | (v1Min > 0 && _v2.SetMin(state, v1Min / (max + 1) + 1))
                         || result;
                }
                else if (v1Max < 0) // Negative numerator
                {
                    if (max >= 0) return result;
                    return _v1.SetMax(state, (max + 1) * v2Max - 1)
                         | _v2.SetMax(state, v1Min / Min(max + 1, -1) + Max(-1, max + 1))
                         || result;
                }

                // Mixed sign numerator
                if (max > 0) return _v1.SetMax(state, (max + 1) * v2Max - 1) || result;
                if (max == 0) return _v1.SetMax(state, 0) || result;
                return _v1.SetMax(state, (max + 1) * v2Max - 1)
                     | _v2.SetMax(state, v1Min / (max + 1) - 1)
                     || result;
            }
            else if (v2Max < 0) // Negative denominator
            {
                if (v1Min >= 0) // Positive numerator
                {
                    if (max >= 0) return result;
                    return _v1.SetMax(state, (max + 1) * v2Min - 1)
                         | _v2.SetMin(state, v1Min / (max + 1) + 1)
                         || result;
                }
                else if (v1Max < 0) // Negative numerator
                {
                    return _v1.SetMin(state, (max + 1) * v2Min + 1)
                         | _v2.SetMax(state, v1Max / (max + 1) - 1)
                         || result;
                }

                // Mixed sign numerator

                if (max > 0) return _v1.SetMin(state, (max + 1) * v2Max + 1) || result;
                if (max == 0) return _v1.SetMin(state, 0) || result;
                return _v1.SetMin(state, (max + 1) * v2Max + 1)
                     | _v2.SetMin(state, v1Max / (max + 1) + 1)
                    || result;
            }

            // Mixed sign denominator

            if (max >= 0) return result;

            if (v1Min >= 0) // Positive numerator
            {
                return _v1.SetMin(state, (max + 1) * -1 - 1)
                     | _v2.SetMax(state, v1Min / (max + 1) + 1)
                     || result;
            }
            else if (v1Max < 0) // Negative numerator
            {
                return _v1.SetMin(state, (max + 1) * v2Min + 1)
                     | _v2.SetMax(state, v1Max / (max + 1) - 1)
                     || result;
            }

            // Mixed sign numerator

            return result;
        }

        public bool SetMin(IState state, int min) =>
            SetMin(GetVariableDomainExtremes(state), state, min);

        private bool SetMin((int v1Min, int v1Max, int v2Min, int v2Max) extremes, IState state, int min)
        {
            var result = false;
            (int v1Min, int v1Max, int v2Min, int v2Max) = extremes;

            if (v2Min == 0)
            {
                result |= _v2.SetMin(state, 1);
                v2Min++;
            }

            if (v2Max == 0)
            {
                result |= _v2.SetMax(state, -1);
                v2Max--;
            }

            if (min < 0)
            {
                if (v1Min > 0 && v2Max < 0)
                {
                    result |= _v2.SetMin(state, v1Max / (min - 1) + 1);
                    result |= _v1.SetMin(state, (min - 1) * v2Max - 1);
                }
                else if (v1Max < 0 && v2Min > 0)
                {
                    if (v1Max < min) result |= _v2.SetMax(state, v1Max / (min - 1) - 1);
                    //result |= _v1.SetMax(state, ((min + 1) * v2Max) + 1);
                }
            }
            else
            {
                if (v1Min > 0)
                {
                    result |= _v2.SetMin(state, 1);
                    result |= _v2.SetMax(state, (int)Ceiling(v1Max / (double)min));
                }
                else if (v1Max < 0)
                {
                    result |= _v2.SetMax(state, -1);
                    result |= _v2.SetMin(state, (int)Floor(v1Min / (double)min));
                }

                if (v2Min > 0)
                {
                    result |= _v1.SetMin(state, v2Min * min);
                }
                else if (v2Max < 0)
                {
                    result |= _v1.SetMax(state, v2Max * min);
                }
            }

            return result;
        }

        public bool SetValue(IState state, object value)
        {
            var extremes = GetVariableDomainExtremes(state);
            return SetMax(extremes, state, (int)value) | SetMin(extremes, state, (int)value);
        }

        public bool TryGetValue(in IState state, out int value)
        {
            if (_v1.TryGetValue(state, out int v1) & _v2.TryGetValue(state, out int v2))
            {
                value = v1 / v2;
                return true;
            }

            value = 0;
            return false;
        }

        public Type VariableType() => typeof(int);

        public string PrettyDomain(in IState state) => $"{_v1.PrettyDomain(state)} / {_v2.PrettyDomain(state)}";

        public IEnumerable<IVariable> GetChildren() => new IVariable[] { _v1, _v2 };
    }
}
