using CSPSolver.common;
using CSPSolver.common.variables;

namespace CSPSolver.Model
{
    public class ModelBoolVar : ModelVar<bool>
    {
        public IBoolVar Variable { get; set; }

        public override IVariable<bool> GetVariable() => (IVariable<bool>)Variable;
    }
}
