using CSPSolver.common;
using CSPSolver.common.variables;
using CSPSolver.Constraint.Bool;
using CSPSolver.Constraint.Logic;

namespace CSPSolver.Model
{
    public class ModelBoolVar : ModelVar<bool>
    {
        public IBoolVar Variable { get; set; }

        public override IVariable<bool> GetVariable() => (IVariable<bool>)Variable;

        public static ModelConstraint operator |(ModelBoolVar v1, ModelBoolVar v2) => new(new Or(new BoolConstraint(v1.Variable), new BoolConstraint(v2.Variable)));

        public static ModelConstraint operator &(ModelBoolVar v1, ModelBoolVar v2) => new(new And(new BoolConstraint(v1.Variable), new BoolConstraint(v2.Variable)));
    }
}
