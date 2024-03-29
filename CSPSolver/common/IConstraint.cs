﻿using System.Collections.Generic;

namespace CSPSolver.common
{
    public interface IConstraint
    {
        IEnumerable<IVariable> Variables { get; }

        IEnumerable<IVariable> Propagate(IState state);

        IEnumerable<IVariable> NegativePropagate(IState state);


        bool IsMet(IState state);

        bool CanBeMet(IState state);
    }
}
