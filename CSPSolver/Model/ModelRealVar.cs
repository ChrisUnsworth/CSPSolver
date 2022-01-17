using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSPSolver.common;
using CSPSolver.common.variables;
using CSPSolver.Math.Divide;
using CSPSolver.Math.Round;
using CSPSolver.Variable;

namespace CSPSolver.Model
{
    public class ModelRealVar : ModelVar<double>
    {
        public IRealVar Variable { get; set; }

        public override IVariable<double> GetVariable() => Variable;

        public static implicit operator ModelRealVar(ModelIntVar i) => new() { Variable = new RealVarIntWrapper(i.Variable) };

        public static ModelRealVar operator /(ModelRealVar v1, ModelRealVar v2)
        {
            if (v1.Variable.Min >= 0 && v2.Variable.Min >= 0) return new() { Variable = new DividePositiveRealVar(v1.Variable, v2.Variable) };
            throw new NotImplementedException();
        }

        public static ModelIntVar Truncate(ModelRealVar v) => new() { Variable = new Truncate(v.Variable) };
    }
}
