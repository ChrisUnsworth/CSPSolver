using System;
using System.Collections.Generic;
using System.Linq;

using CSPSolver.common;
using CSPSolver.common.variables;
using CSPSolver.Constraint.AllDiff;
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

        public static ModelConstraint AllDiff(IEnumerable<ModelIntVar> vars) => AllDiff(vars.ToArray());

        public static ModelConstraint AllDiff(params ModelIntVar[] vars)
        {
            if (vars.All(m => m.GetVariable() is ISmallIntDomainVar))
            {
                var args = vars.Select(m => m.GetVariable() as ISmallIntDomainVar);
                return args.Select(v => v.Min).Distinct().Count() == 1
                    ? new(new AllDiffSameIntDomain(args))
                    : new(new AllDiffMixedSmallIntDomain(args));
            }

            if (vars.All(m => m.GetVariable() is ILongDomainVar)) return new(new AllDiffLongDomain(vars.Select(m => m.GetVariable() as ILongDomainVar)));

            if (vars.All(m => m.GetVariable() is IIntVar)) return new(new ForwardCheckingAllDiff(vars.Select(m => m.GetVariable() as IIntVar)));

            throw new NotSupportedException();
        }
    }
}
