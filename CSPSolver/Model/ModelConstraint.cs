using CSPSolver.common;
using CSPSolver.Constraint.Logic;

namespace CSPSolver.Model
{
    public class ModelConstraint
    {
        private IConstraint _constraint;

        public ModelConstraint(IConstraint constraint) => _constraint = constraint;

        public IConstraint Constraint => _constraint;

        public static ModelConstraint operator &(ModelConstraint c1, ModelConstraint c2) => new(new And(c1.Constraint, c2.Constraint));
        public static ModelConstraint operator |(ModelConstraint c1, ModelConstraint c2) => new(new Or(c1.Constraint, c2.Constraint));

        public static ModelConstraint IfThen(ModelConstraint c1, ModelConstraint c2) => new(new IfThen(c1.Constraint, c2.Constraint));

        public static ModelConstraint IfThenElse(ModelConstraint c1, ModelConstraint c2, ModelConstraint c3) => new(new IfThenElse(c1.Constraint, c2.Constraint, c3.Constraint));
    }
}
