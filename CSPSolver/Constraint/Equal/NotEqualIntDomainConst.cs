﻿using System.Linq;
using System.Collections.Generic;

using CSPSolver.common;

namespace CSPSolver.Constraint.Equal
{
    public readonly struct NotEqualIntDomainConst : IConstraint
    {
        private readonly IIntVar _var;
        private readonly int _con;

        public NotEqualIntDomainConst(IIntVar var, int con) => (_var, _con) = (var, con);

        public IEnumerable<IVariable> Variables => new List<IVariable>() { _var };

        public IEnumerable<IVariable> Propagate(IState state) => _var.RemoveValue(state, _con) ? new IVariable[] { _var } : Enumerable.Empty<IVariable>();
    }
}
