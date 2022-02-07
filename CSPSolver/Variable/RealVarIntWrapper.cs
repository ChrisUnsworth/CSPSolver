using System;

using static System.Math;

using CSPSolver.common;
using CSPSolver.common.variables;

namespace CSPSolver.Variable
{
    public readonly struct RealVarIntWrapper : IRealVar
    {
        private readonly IIntVar _intVar;

        public RealVarIntWrapper(IIntVar intVar) => _intVar = intVar;

        public double Min => _intVar.Min;

        public double Max => _intVar.Max;

        public double Epsilon => 1;

        public double GetDomainMax(IState state) => _intVar.GetDomainMax(state);

        public double GetDomainMin(IState state) => _intVar.GetDomainMin(state);

        public void Initialise(IState state) => _intVar.Initialise(state);

        public bool IsEmpty(IState state) => _intVar.IsEmpty(state);

        public bool IsInstantiated(IState state) => _intVar.IsInstantiated(state);

        public string PrettyDomain(IState state) => _intVar.PrettyDomain(state);

        public bool RemoveValue(IState state, object value) => _intVar.RemoveValue(state, (int)value);

        public bool SetMax(IState state, double max) => _intVar.SetMax(state, (int)Ceiling(max));

        public bool SetMin(IState state, double min) => _intVar.SetMin(state, (int)Floor(min));

        public bool SetValue(IState state, object value) => _intVar.SetValue(state, (int)Round((double)value));

        public bool TryGetValue(IState state, out double value)
        {
            var result = _intVar.TryGetValue(state, out int intVal);
            value = intVal;
            return result;
        }

        public Type VariableType() => typeof(double);
    }
}
