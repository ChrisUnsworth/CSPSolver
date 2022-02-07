using System;
using System.Collections.Generic;

using CSPSolver.common;
using CSPSolver.common.variables;

using static System.Math;

namespace CSPSolver.Math.Round
{
    public readonly struct Floor : IIntVar, ICompoundVariable
    {
        private readonly IRealVar _realVar;

        public Floor(IRealVar realVar) => _realVar = realVar;

        public int Min => (int)Floor(_realVar.Min);

        public int Size => Max - Min + 1;

        public int Max => (int)Floor(_realVar.Max);

        public IEnumerable<IVariable> GetChildren() { yield return _realVar; }

        public int GetDomainMax(IState state) => (int)Floor(_realVar.GetDomainMax(state));

        public int GetDomainMin(IState state) => (int)Floor(_realVar.GetDomainMin(state));

        public void Initialise(IState state) => _realVar.Initialise(state);

        public bool IsEmpty(IState state) => _realVar.IsEmpty(state);

        public bool IsInstantiated(IState state) => GetDomainMax(state) == GetDomainMin(state);

        public string PrettyDomain(IState state)
        {
            var min = GetDomainMin(state);
            var max = GetDomainMax(state);

            return min == max
                ? $"{{ {max} }}"
                : $"{{ {min} .. {max} }}";
        }

        public bool RemoveValue(IState state, object value) => _realVar.RemoveValue(state, value);

        public bool SetMax(IState state, int max) => _realVar.SetMax(state, max + 1 - _realVar.Epsilon);

        public bool SetMin(IState state, int min) => _realVar.SetMin(state, min);

        public bool SetValue(IState state, object value) =>
            SetMax(state, (int)value) |
            SetMin(state, (int)value);

        public bool TryGetValue(IState state, out int value)
        {
            value = GetDomainMin(state);
            return value == GetDomainMax(state);
        }

        public Type VariableType() => typeof(int);
    }
}
