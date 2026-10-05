using CSPSolver.common;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using CSPSolver.common.variables;

namespace CSPSolver.Model;

public readonly struct Model : IModel
{
    public bool Maximise { get; }

    public IVariable Objective { get; }

    private IVariable[] Variables { get; }

    private IVariable[] NonDecisionVariables { get; }

    private IConstraint[] Constraints { get; }

    IEnumerable<IVariable> IModel.Variables => Variables;

    IEnumerable<IConstraint> IModel.Constraints => Constraints;

    // Indexed by variable Id: the indices of the constraints that variable appears in.
    private readonly int[][] _constraintsByVariable;

    // Every constraint index, for the first round of Propagate.
    private readonly int[] _allConstraints;

    // Scratch space for Propagate, so it is neither thread-safe nor re-entrant; each Search gets its own Model.
    // _queued keeps a constraint to once per round, so a round never holds more than Constraints.Length entries.
    private readonly bool[] _queued;
    private readonly int[] _roundA;
    private readonly int[] _roundB;

    public Model(IConstraint[] constraints, IVariable[] variables, IVariable[] nonDecisionVariables, IVariable objective = null, bool? maximise = null)
    {
        Maximise = maximise ?? true;
        Objective = objective;
        Variables = variables;
        NonDecisionVariables = nonDecisionVariables;
        Constraints = constraints;
        _allConstraints = Enumerable.Range(0, constraints.Length).ToArray();
        _queued = new bool[constraints.Length];
        _roundA = new int[constraints.Length];
        _roundB = new int[constraints.Length];

        var ids = constraints
            .Select(c => c.Variables.SelectMany(GetStateIds).Where(id => id >= 0).Distinct().ToArray())
            .ToArray();
        var size = ids.SelectMany(i => i).DefaultIfEmpty(-1).Max() + 1;

        var byVariable = Enumerable.Range(0, size).Select(_ => new List<int>()).ToArray();
        for (int c = 0; c < constraints.Length; c++)
        {
            foreach (var id in ids[c]) byVariable[id].Add(c);
        }

        _constraintsByVariable = byVariable.Select(l => l.ToArray()).ToArray();
    }

    // Same rule as Enqueue: a variable's own Id first, otherwise the variables an expression is built from.
    private static IEnumerable<int> GetStateIds(IVariable v) =>
        v.Id >= 0 ? [v.Id]
        : v is ICompoundVariable cv ? cv.GetChildren().SelectMany(GetStateIds)
        : [];

    public void Propagate(IState state)
    {
        var todo = _allConstraints;
        var todoCount = todo.Length;
        var next = _roundA;

        while (todoCount > 0)
        {
            var nextCount = 0;

            for (int i = 0; i < todoCount; i++)
            {
                var changed = Constraints[todo[i]].Propagate(state);

                // Concrete types rather than IReadOnlyList: casting an array to a generic interface is slow.
                if (changed is IVariable[] array)
                {
                    for (int j = 0; j < array.Length; j++) Enqueue(array[j], next, ref nextCount);
                }
                else if (changed is List<IVariable> list)
                {
                    for (int j = 0; j < list.Count; j++) Enqueue(list[j], next, ref nextCount);
                }
                else
                {
                    foreach (var v in changed) Enqueue(v, next, ref nextCount);
                }
            }

            for (int i = 0; i < nextCount; i++) _queued[next[i]] = false;

            todo = next;
            todoCount = nextCount;
            next = next == _roundA ? _roundB : _roundA;
        }
    }

    private void Enqueue(IVariable v, int[] next, ref int nextCount)
    {
        if (v.Id >= 0)
        {
            if (v.Id >= _constraintsByVariable.Length) return;

            foreach (var c in _constraintsByVariable[v.Id])
            {
                if (!_queued[c])
                {
                    _queued[c] = true;
                    next[nextCount++] = c;
                }
            }
        }
        else if (v is ICompoundVariable cv)
        {
            foreach (var child in cv.GetChildren()) Enqueue(child, next, ref nextCount);
        }
    }

    public string PrettyDomains(IState state)
    {
        var sb = new StringBuilder();

        foreach (var v in Variables)
        {
            sb.AppendLine(v.PrettyDomain(state));
        }

        return sb.ToString();
    }

    public DomainState GetDomainState(IState state)
    {
        var result = DomainState.Instantiated;

        foreach (var v in Variables)
        {
            switch (v.GetDomainState(state))
            {
                case DomainState.Empty:
                    return DomainState.Empty;
                case DomainState.Undecided:
                    result = DomainState.Undecided;
                    break;
            }
        }

        // Non-decision variables need not be fixed for a solution, but a constraint can fail by emptying one.
        foreach (var v in NonDecisionVariables)
        {
            if (v.GetDomainState(state) == DomainState.Empty) return DomainState.Empty;
        }

        return result;
    }

    public void Initialise(IState state)
    {
        foreach (var v in Variables) v.Initialise(state);
        foreach (var v in NonDecisionVariables) v.Initialise(state);
    }
}