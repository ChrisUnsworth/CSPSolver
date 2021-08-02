using System;
using System.Collections.Generic;
using System.Linq;
using CSPSolver.common;
using CSPSolver.common.variables;

namespace CSPSolver.Constraint.Divide
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

        private (int v1Min, int v1Max, int v2Min, int v2Max) GetVariableDomainExtremes(IState state)
            => (_v1.GetDomainMin(state), _v1.GetDomainMax(state), _v2.GetDomainMin(state), _v2.GetDomainMax(state));

        public int GetDomainMax(IState state)
            => GetMax(_v1.GetDomainMin(state), _v1.GetDomainMax(state), _v2.GetDomainMin(state), _v2.GetDomainMax(state));

        private static int GetMax(int v1Min, int v1Max, int v2Min, int v2Max)
        {
            int max = int.MinValue;

            if (v2Max > 0) max = v1Max / Math.Max(1, v2Min);
            if (v2Min < 0) max = Math.Max(max, v1Min / Math.Min(-1, v2Max));

            return max;
        }

        public int GetDomainMin(IState state)
            => GetMin(_v1.GetDomainMin(state), _v1.GetDomainMax(state), _v2.GetDomainMin(state), _v2.GetDomainMax(state));

        private static int GetMin(int v1Min, int v1Max, int v2Min, int v2Max)
        {
            int min = int.MaxValue;

            if (v2Max > 0)
            {
                min = v1Min / v2Max;
                min = Math.Min(min, v1Max / Math.Min(1, v2Min));
            }
            if (v2Min < 0)
            {
                min = Math.Min(min, v1Max / v2Min);
                min = Math.Min(min, v1Max / Math.Min(-1, v2Max));
            }

            return min;
        }

        public void initialise(IState state) { /* holds no state */ }

        public bool isEmpty(IState state) => _v1.isEmpty(state) | _v2.isEmpty(state);

        public bool isInstantiated(IState state) => _v1.isInstantiated(state) & _v2.isInstantiated(state);

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

            if (max >= 0)
            {
                if (v1Min >= 0 & v2Min > 0)
                {
                    result |= _v2.SetMin(state, (v1Min / (max + 1)) + 1);
                    result |= _v1.SetMax(state, ((max + 1) * v2Max) - 1);
                } else if (v1Max < 0 & v2Max < 0)
                {
                    result |= _v1.SetMin(state, ((max + 1) * v2Min) + 1);
                    result |= _v2.SetMax(state, (v1Max / (max + 1)) - 1);
                }
            } else
            {
                if (v1Min > 0)
                {
                    result |= _v2.SetMax(state, -1);
                    result |= _v2.SetMin(state, (v1Max / (max - 1)) + 1);
                } else if (v1Max <= 0)
                {
                    result |= _v1.SetMax(state, -1);
                    result |= _v2.SetMin(state, 1);
                    result |= _v2.SetMax(state, (v1Min / (max - 1)) + 1);
                }

                if (v2Min > 0)
                {
                    result |= _v1.SetMax(state, -1);
                    result |= _v1.SetMin(state, ((max - 1) * v2Max) + 1);
                } else if (v2Max < 0)
                {
                    result |= _v1.SetMin(state, ((max - 1) * v2Min) + 1);
                }
            }

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
                    result |= _v2.SetMin(state, (v1Max / (min - 1)) + 1);
                    result |= _v1.SetMin(state, ((min - 1) * v2Max) - 1);
                }  else if (v1Max < 0 && v2Min > 0)
                {

                }
            }
            else
            {
                if (v1Min > 0)
                {

                } else if (v1Max < 0)
                {

                }

                if (v2Min > 0)
                {

                } else if (v2Max < 0)
            }


            throw new NotImplementedException();

            if (v1Min > 0 && min != 0) result |= _v2.SetMax(state, (int)Math.Ceiling(_v1.GetDomainMax(state) / (double)min));
            if (v2Min > 0) result |= _v1.SetMin(state, _v2.GetDomainMin(state) * min);

            if (v1Max < 0 && min != 0) result |= _v2.SetMin(state, (int)Math.Ceiling(_v1.GetDomainMin(state) / (double)min));
            if (v2Max < 0) result |= _v1.SetMax(state, _v2.GetDomainMax(state) * min);

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
                value = v1 / v2;
                return true;
            }

            value = 0;
            return false;
        }

        public Type VariableType() => typeof(int);

        public string PrettyDomain(IState state)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IVariable> GetChildren() => new IVariable[] { _v1, _v2 };
    }
}
