using CSPSolver.common;
using CSPSolver.Constraint.Equal;
using CSPSolver.Constraint.Minus;
using CSPSolver.Constraint.Plus;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSPSolver.Model
{
    public class ModelIntVar : ModelVar<int>
    {
        public IIntVar variable { get; set; }

        public override IVariable<int> GetVariable() => variable;

        public static ModelIntVar operator +(ModelIntVar v1, ModelIntVar v2) => new ModelIntVar { variable = new PlusIntDomain(v1.variable, v2.variable) };
        public static ModelIntVar operator -(ModelIntVar v1, ModelIntVar v2) => new ModelIntVar { variable = new MinusIntDomain(v1.variable, v2.variable) };
        public static IConstraint operator ==(ModelIntVar v1, ModelIntVar v2) => new EqualIntVar(v1.variable, v2.variable);
        public static IConstraint operator !=(ModelIntVar v1, ModelIntVar v2) => new NotEqualIntVar(v1.variable, v2.variable);
    }
}
