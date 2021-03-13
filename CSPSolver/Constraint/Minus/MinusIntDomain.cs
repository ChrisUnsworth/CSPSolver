using CSPSolver.common;
using CSPSolver.common.variables;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSPSolver.Constraint.Minus
{
    public readonly struct MinusIntDomain : IIntVar, ICompoundVariable
    {
        private readonly IIntVar _v1;
        private readonly IIntVar _v2;

        public int Min { get; }

        public int Size { get; }

        public int Max { get; }

        public MinusIntDomain(IIntVar v1, IIntVar v2)
        {
            _v1 = v1;
            _v2 = v2;
            Min = v1.Min - v2.Max;
            Max = v1.Max - v2.Min;
            Size = Max - Min + 1;
        }

        public int GetDomainMax(IState state) => _v1.GetDomainMax(state) - _v2.GetDomainMin(state);

        public int GetDomainMin(IState state) => _v1.GetDomainMin(state) - _v2.GetDomainMax(state);

        public void initialise(IState state) { /* holds no state */ }

        public bool isEmpty(IState state) => _v1.isEmpty(state) | _v2.isEmpty(state);

        public bool isInstantiated(IState state) => _v1.isInstantiated(state) & _v2.isInstantiated(state);

        public bool RemoveValue(IState state, object value)
        {
            var result = false;
            var intVal = (int)value;
            if (_v2.TryGetValue(state, out int v2))
            {
                result = _v1.RemoveValue(state, v2 + intVal);
            }

            if (_v1.TryGetValue(state, out int v1))
            {
                result |= _v2.RemoveValue(state, v1 - intVal);
            }

            return result;
        }

        public bool SetMax(IState state, int max) =>
            _v1.SetMax(state, max + _v2.GetDomainMax(state))
          | _v2.SetMin(state, _v1.GetDomainMin(state) - max);

        public bool SetMin(IState state, int min) => 
            _v1.SetMin(state, min + _v2.GetDomainMin(state))
          | _v2.SetMax(state, _v1.GetDomainMax(state) - min);

        public bool SetValue(IState state, object value) => SetMax(state, (int)value) | SetMin(state, (int)value);

        public bool TryGetValue(IState state, out int value)
        {
            if (_v1.TryGetValue(state, out int v1) & _v2.TryGetValue(state, out int v2))
            {
                value = v1 - v2;
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
