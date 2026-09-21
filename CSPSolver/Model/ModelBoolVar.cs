using System;
using System.Collections.Generic;

using CSPSolver.common;
using CSPSolver.common.variables;
using CSPSolver.Constraint.Bool;
using CSPSolver.Constraint.Logic;

namespace CSPSolver.Model;

public class ModelBoolVar : ModelVar<bool>
{
    public IBoolVar Variable { get; set; }

    public override IVariable<bool> GetVariable() => Variable;

    // == and != build constraints rather than comparing, so they deliberately
    // do not agree with Equals. Equals answers whether two model vars stand for
    // the same underlying variable; == asks the solver to make them equal.
    public override bool Equals(object obj) => obj is ModelBoolVar var && EqualityComparer<IBoolVar>.Default.Equals(Variable, var.Variable);
    public override int GetHashCode() => HashCode.Combine(Variable);

    public static ModelConstraint operator |(ModelBoolVar v1, ModelBoolVar v2) => new(new Or(new BoolConstraint(v1.Variable), new BoolConstraint(v2.Variable)));

    public static ModelConstraint operator &(ModelBoolVar v1, ModelBoolVar v2) => new(new And(new BoolConstraint(v1.Variable), new BoolConstraint(v2.Variable)));

    public static ModelConstraint operator ==(ModelBoolVar v1, ModelBoolVar v2) => new(new IfAndOnlyIf(new BoolConstraint(v1.Variable), new BoolConstraint(v2.Variable)));

    public static ModelConstraint operator !=(ModelBoolVar v1, ModelBoolVar v2) => new(new XOr(new BoolConstraint(v1.Variable), new BoolConstraint(v2.Variable)));
}
