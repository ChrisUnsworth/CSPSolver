using System;
using System.Collections.Generic;

using CSPSolver.common;
using CSPSolver.Constraint.Equal;
using CSPSolver.Math.Minus;
using CSPSolver.Math.Plus;
using CSPSolver.Math.Multiply;
using CSPSolver.Math.Divide;
using CSPSolver.Variable;
using CSPSolver.Constraint.Inequality;
using CSPSolver.common.variables;

namespace CSPSolver.Model
{
    public class ModelIntVar : ModelVar<int>
    {
        public IIntVar Variable { get; set; }

        public override IVariable<int> GetVariable() => Variable;

        public override bool Equals(object obj) => obj is ModelIntVar var && EqualityComparer<IIntVar>.Default.Equals(Variable, var.Variable);
        public override int GetHashCode() => HashCode.Combine(Variable);

        public static implicit operator ModelIntVar(int i) => new() { Variable = new IntConstVar(i) };

        public static ModelIntVar operator +(ModelIntVar v1, ModelIntVar v2) => new() { Variable = new PlusIntDomain(v1.Variable, v2.Variable) };
        public static ModelIntVar operator -(ModelIntVar v1, ModelIntVar v2) => new() { Variable = new MinusIntDomain(v1.Variable, v2.Variable) };
        public static ModelIntVar operator *(ModelIntVar v1, ModelIntVar v2)
        {
            if (v1.Variable.Min >= 0 && v2.Variable.Min >= 0) return new() { Variable = new PositiveMultiplyIntVar(v1.Variable, v2.Variable) };
            if (v1.Variable.Max < 0 && v2.Variable.Max < 0) return new() { Variable = new NegativeMultiplyIntVar(v1.Variable, v2.Variable) };
            throw new NotImplementedException(); //new ModelIntVar { variable = new MixedSignMultiplyIntVar(v1.variable, v2.variable) };
        }
        public static ModelIntVar operator /(ModelIntVar v1, ModelIntVar v2)
        {
            if (v1.Variable.Min >= 0 && v2.Variable.Min >= 0) return new() { Variable = new PositiveDivideIntVar(v1.Variable, v2.Variable) };
            if (v1.Variable.Max < 0 && v2.Variable.Max < 0) return new() { Variable = new NegativeDivideIntVar(v1.Variable, v2.Variable) };
            return new ModelIntVar { Variable = new MixedSignDivideIntVar(v1.Variable, v2.Variable) };
        }
        public static ModelConstraint operator ==(ModelIntVar v1, ModelIntVar v2) => new(new EqualIntVar(v1.Variable, v2.Variable));
        public static ModelConstraint operator !=(ModelIntVar v1, ModelIntVar v2) => new(new NotEqualIntVar(v1.Variable, v2.Variable));
        public static ModelConstraint operator ==(ModelIntVar v, int i) => new(new EqualIntDomainConst(v.Variable, i));
        public static ModelConstraint operator !=(ModelIntVar v, int i) => new(new NotEqualIntDomainConst(v.Variable, i));
        public static ModelConstraint operator ==(int i, ModelIntVar v) => new(new EqualIntDomainConst(v.Variable, i));
        public static ModelConstraint operator !=(int i, ModelIntVar v) => new(new NotEqualIntDomainConst(v.Variable, i));
        public static ModelConstraint operator >(ModelIntVar v1, ModelIntVar v2) => new(new GreaterThanIntVar(v1.Variable, v2.Variable));
        public static ModelConstraint operator <(ModelIntVar v1, ModelIntVar v2) => new(new GreaterThanIntVar(v2.Variable, v1.Variable));
        public static ModelConstraint operator >=(ModelIntVar v1, ModelIntVar v2) => new(new GreaterEqualIntVar(v1.Variable, v2.Variable));
        public static ModelConstraint operator <=(ModelIntVar v1, ModelIntVar v2) => new(new GreaterEqualIntVar(v2.Variable, v1.Variable));
    }
}
