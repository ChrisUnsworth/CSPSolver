using CSPSolver.common;
using CSPSolver.Constraint.Bool;
using CSPSolver.Constraint.Logic;

namespace CSPSolver.Model
{
    public class ModelConstraint
    {
        private readonly IConstraint _constraint;

        public ModelConstraint(IConstraint constraint) => _constraint = constraint;

        public static implicit operator ModelConstraint(ModelBoolVar boolVar) => new(new BoolConstraint(boolVar.Variable));

        public IConstraint Constraint => _constraint;

        public static ModelConstraint operator &(ModelConstraint c1, ModelConstraint c2) => new(new And(c1.Constraint, c2.Constraint));
        public static ModelConstraint operator &(ModelBoolVar boolVar, ModelConstraint c2) => new(new And(new BoolConstraint(boolVar.Variable), c2.Constraint));
        public static ModelConstraint operator &(ModelConstraint c1, ModelBoolVar boolVar) => new(new And(c1.Constraint, new BoolConstraint(boolVar.Variable)));

        public static ModelConstraint operator !(ModelConstraint c1) => new(new Not(c1.Constraint));

        public static ModelConstraint operator |(ModelConstraint c1, ModelConstraint c2) => new(new Or(c1.Constraint, c2.Constraint));

        public static ModelConstraint operator |(ModelBoolVar boolVar, ModelConstraint c2) => new(new Or(new BoolConstraint(boolVar.Variable), c2.Constraint));

        public static ModelConstraint operator |(ModelConstraint c1, ModelBoolVar boolVar) => new(new Or(c1.Constraint, new BoolConstraint(boolVar.Variable)));

        public static ModelConstraint IfThen(ModelConstraint c1, ModelConstraint c2) => new(new IfThen(c1.Constraint, c2.Constraint));

        public static ModelConstraint IfThen(ModelBoolVar boolVar, ModelConstraint c2) => new(new IfThen(new BoolConstraint(boolVar.Variable), c2.Constraint));

        public static ModelConstraint IfThen(ModelConstraint c1, ModelBoolVar boolVar) => new(new IfThen(c1.Constraint, new BoolConstraint(boolVar.Variable)));

        public static ModelConstraint IfThenElse(ModelConstraint c1, ModelConstraint c2, ModelConstraint c3) => new(new IfThenElse(c1.Constraint, c2.Constraint, c3.Constraint));

        public static ModelConstraint XOr(ModelConstraint c1, ModelConstraint c2) => new(new XOr(c1.Constraint, c2.Constraint));

        public static ModelConstraint Not(ModelConstraint c1) => new(new Not(c1.Constraint));

        public static ModelConstraint IfAndOnlyIf(ModelConstraint c1, ModelConstraint c2) => new(new IfAndOnlyIf(c1.Constraint, c2.Constraint));
    }
}
