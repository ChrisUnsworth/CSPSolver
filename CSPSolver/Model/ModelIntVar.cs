using CSPSolver.common;
using CSPSolver.Constraint.Equal;
using CSPSolver.Constraint.Minus;
using CSPSolver.Constraint.Plus;
using CSPSolver.Constraint.Multiply;
using System;
using System.Collections.Generic;
using System.Text;
using CSPSolver.Variable;

namespace CSPSolver.Model
{
    public class ModelIntVar : ModelVar<int>
    {
        public IIntVar variable { get; set; }

        public override IVariable<int> GetVariable() => variable;

        public override bool Equals(object obj) => obj is ModelIntVar var && EqualityComparer<IIntVar>.Default.Equals(variable, var.variable);
        public override int GetHashCode() => HashCode.Combine(variable);

        public static implicit operator ModelIntVar(int i) => new() { variable = new IntConstVar(i) };

        public static ModelIntVar operator +(ModelIntVar v1, ModelIntVar v2) => new() { variable = new PlusIntDomain(v1.variable, v2.variable) };
        public static ModelIntVar operator -(ModelIntVar v1, ModelIntVar v2) => new() { variable = new MinusIntDomain(v1.variable, v2.variable) };
        public static ModelIntVar operator *(ModelIntVar v1, ModelIntVar v2)
        {
            if (v1.variable.Min >= 0 && v2.variable.Min >= 0) return new() { variable = new PositiveMultiplyIntVar(v1.variable, v2.variable) };
            if (v1.variable.Max < 0 && v2.variable.Max < 0) return new() { variable = new NegativeMultiplyIntVar(v1.variable, v2.variable) };
            return new ModelIntVar { variable = new MixedSignMultiplyIntVar(v1.variable, v2.variable) };
        }
        public static IConstraint operator ==(ModelIntVar v1, ModelIntVar v2) => new EqualIntVar(v1.variable, v2.variable);
        public static IConstraint operator !=(ModelIntVar v1, ModelIntVar v2) => new NotEqualIntVar(v1.variable, v2.variable);
        public static IConstraint operator ==(ModelIntVar v, int i) => new EqualIntDomainConst(v.variable, i);
        public static IConstraint operator !=(ModelIntVar v, int i) => new NotEqualIntDomainConst(v.variable, i);
        public static IConstraint operator ==(int i, ModelIntVar v) => new EqualIntDomainConst(v.variable, i);
        public static IConstraint operator !=(int i, ModelIntVar v) => new NotEqualIntDomainConst(v.variable, i);
    }
}
