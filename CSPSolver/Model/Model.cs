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

    // Scratch space for Propagate, so a constraint is only queued once per round.
    private readonly bool[] _queued;

    public Model(IConstraint[] constraints, IVariable[] variables, IVariable[] nonDecisionVariables, IVariable objective = null, bool? maximise = null)
    {
        Maximise = maximise ?? true;
        Objective = objective;
        Variables = variables;
        NonDecisionVariables = nonDecisionVariables;
        Constraints = constraints;
        _queued = new bool[constraints.Length];

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
        var todo = Enumerable.Range(0, Constraints.Length).ToList();
        var next = new List<int>();

        while (todo.Count > 0)
        {
            foreach (var c in todo)
            {
                foreach (var v in Constraints[c].Propagate(state)) Enqueue(v, next);
            }

            foreach (var c in next) _queued[c] = false;
            (todo, next) = (next, todo);
            next.Clear();
        }
    }

    private void Enqueue(IVariable v, List<int> next)
    {
        if (v.Id >= 0)
        {
            if (v.Id >= _constraintsByVariable.Length) return;

            foreach (var c in _constraintsByVariable[v.Id])
            {
                if (!_queued[c])
                {
                    _queued[c] = true;
                    next.Add(c);
                }
            }
        }
        else if (v is ICompoundVariable cv)
        {
            foreach (var child in cv.GetChildren()) Enqueue(child, next);
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

    public bool IsSolved(IState state) => Variables.All(v => v.IsInstantiated(state));

    public bool HasEmptyDomain(IState state) => Variables.Concat(NonDecisionVariables).Any(v => v.IsEmpty(state));

    public void Initialise(IState state)
    {
        foreach (var v in Variables) v.Initialise(state);
        foreach (var v in NonDecisionVariables) v.Initialise(state);
    }
}