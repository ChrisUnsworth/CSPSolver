using System;
using System.Collections.Generic;

using CSPSolver.common;
using CSPSolver.common.variables;
using CSPSolver.Constraint.Equal;
using CSPSolver.Constraint.Inequality;
using CSPSolver.Math.Divide;
using CSPSolver.Math.Minus;
using CSPSolver.Math.Multiply;
using CSPSolver.Math.Plus;
using CSPSolver.Math.Round;
using CSPSolver.Variable;

namespace CSPSolver.Model;

public class ModelRealVar : ModelVar<double>
{
    public IRealVar Variable { get; set; }

    public override IVariable<double> GetVariable() => Variable;

    // == and != build constraints rather than comparing, so they deliberately
    // do not agree with Equals. Equals answers whether two model vars stand for
    // the same underlying variable; == asks the solver to make them equal.
    public override bool Equals(object obj) => obj is ModelRealVar var && EqualityComparer<IRealVar>.Default.Equals(Variable, var.Variable);

    public override int GetHashCode() => HashCode.Combine(Variable);

    public static implicit operator ModelRealVar(ModelIntVar i) => new() { Variable = new RealVarIntWrapper(i.Variable) };

    public static ModelRealVar operator /(ModelRealVar v1, ModelRealVar v2)
    {
        // DividePositiveRealVar has no zero-exclusion of its own, unlike its int
        // counterpart -- the denominator must already be strictly positive.
        if (v1.Variable.Min >= 0 && v2.Variable.Min > 0) return new() { Variable = new DividePositiveRealVar(v1.Variable, v2.Variable) };
        return new() { Variable = new MixedSignDivideRealVar(v1.Variable, v2.Variable) };
    }

    public static ModelRealVar operator *(ModelRealVar v1, ModelRealVar v2) =>  new() { Variable = new MixedSignMultiplyRealVar(v1.Variable, v2.Variable) };

    public static ModelRealVar operator +(ModelRealVar v1, ModelRealVar v2) => new() { Variable = new PlusRealVar(v1.Variable, v2.Variable) };

    public static ModelRealVar operator -(ModelRealVar v1, ModelRealVar v2) => new() { Variable = new MinusRealVar(v1.Variable, v2.Variable) };

    public static ModelConstraint operator ==(ModelRealVar v1, ModelRealVar v2) => new(new EqualRealVar(v1.Variable, v2.Variable));

    public static ModelConstraint operator !=(ModelRealVar v1, ModelRealVar v2) => new(new NotEqualRealVar(v1.Variable, v2.Variable));
    public static ModelConstraint operator >(ModelRealVar v1, ModelRealVar v2) => new(new GreaterThanRealVar(v1.Variable, v2.Variable));
    public static ModelConstraint operator <(ModelRealVar v1, ModelRealVar v2) => new(new GreaterThanRealVar(v2.Variable, v1.Variable));
    public static ModelConstraint operator >=(ModelRealVar v1, ModelRealVar v2) => new(new GreaterEqualRealVar(v1.Variable, v2.Variable));
    public static ModelConstraint operator <=(ModelRealVar v1, ModelRealVar v2) => new(new GreaterEqualRealVar(v2.Variable, v1.Variable));

    public static ModelIntVar Truncate(ModelRealVar v) => new() { Variable = new Truncate(v.Variable) };

    public static ModelIntVar Floor(ModelRealVar v) => new() { Variable = new Floor(v.Variable) };

    public static ModelIntVar Ceiling(ModelRealVar v) => new() { Variable = new Ceil(v.Variable) };
}
