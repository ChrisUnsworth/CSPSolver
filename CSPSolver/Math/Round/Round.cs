using System;
using System.Collections.Generic;

using CSPSolver.common;
using CSPSolver.common.variables;

using static System.Math;

namespace CSPSolver.Math.Round
{
    public readonly struct Round : IIntVar, ICompoundVariable
    {
        private readonly IRealVar _realVar;

        public Round(IRealVar realVar) => _realVar = realVar;

        public int Min => (int)System.Math.Round(_realVar.Min);

        public int Size => Max - Min + 1;

        public int Max => (int)System.Math.Round(_realVar.Max);

        public IEnumerable<IVariable> GetChildren() { yield return _realVar; }

        public int GetDomainMax(IState state) => (int)System.Math.Round(_realVar.GetDomainMax(state));

        public int GetDomainMin(IState state) => (int)System.Math.Round(_realVar.GetDomainMin(state));

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

        public bool SetMax(IState state, int max) => _realVar.SetMax(state, max + 0.5 - _realVar.Epsilon);

        public bool SetMin(IState state, int min) => _realVar.SetMin(state, min - 0.5 + _realVar.Epsilon);

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
