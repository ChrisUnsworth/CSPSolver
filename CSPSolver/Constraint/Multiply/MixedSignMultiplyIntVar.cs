using System;
using System.Collections.Generic;
using System.Linq;

using CSPSolver.common;
using CSPSolver.common.variables;

namespace CSPSolver.Constraint.Multiply
{
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
            var extremes = new int[] { v1.Max * v2.Max, v1.Max * v2.Min, v1.Min * v2.Max, v1.Min * v2.Min };
            Min = extremes.Min();
            Max = extremes.Max();
            Size = Max - Min + 1;
        }

        private int[] GetDomainExtremes(IState state)
        {
            (int v1Min, int v1Max, int v2Min, int v2Max) = GetVariableDomainExtremes(state);
            return new int[] { v1Max * v2Max, v1Max * v2Min, v1Min * v2Max, v1Min * v2Min };
        }

        private (int v1Min, int v1Max, int v2Min, int v2Max) GetVariableDomainExtremes(IState state)
            => (_v1.GetDomainMin(state), _v1.GetDomainMax(state), _v2.GetDomainMin(state), _v2.GetDomainMax(state));

        public int GetDomainMax(IState state) => GetDomainExtremes(state).Max();

        public int GetDomainMin(IState state) => GetDomainExtremes(state).Min();

        public void initialise(IState state) { /* holds no state */ }

        public bool isEmpty(IState state) => _v1.isEmpty(state) | _v2.isEmpty(state);

        public bool isInstantiated(IState state) => _v1.isInstantiated(state) & _v2.isInstantiated(state);

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

            if (v1Min > 0) result = _v2.SetMin(state, (int)Math.Ceiling((double)min / v1Max));
            if (v2Min > 0) result |= _v1.SetMin(state, (int)Math.Ceiling((double)min / v2Max));

            if (v1Max < 0) result |= _v2.SetMax(state, (int)Math.Floor((double)min / v1Min));
            if (v2Max < 0) result |= _v1.SetMax(state, (int)Math.Floor((double)min / v2Min));

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

        public string PrettyDomain(IState state)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IVariable> GetChildren() => new IVariable[] { _v1, _v2 };
    }
}
