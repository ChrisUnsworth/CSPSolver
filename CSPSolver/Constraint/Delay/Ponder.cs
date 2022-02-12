using CSPSolver.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSPSolver.Constraint.Delay
{
    public readonly struct Ponder: IConstraint
    {
        private readonly int _delay;

        public Ponder(int delay) => _delay = delay;

        public IEnumerable<IVariable> Variables => Enumerable.Empty<IVariable>();

        public bool CanBeMet(IState state) => true;

        public bool IsMet(IState state) => true;

        public IEnumerable<IVariable> NegativePropagate(IState state)
        {
            Thread.Sleep(_delay);
            return Enumerable.Empty<IVariable>();
        }

        public IEnumerable<IVariable> Propagate(IState state)
        {
            Thread.Sleep(_delay);
            return Enumerable.Empty<IVariable>();
        }
    }
}
