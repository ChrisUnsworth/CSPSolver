[![.NET](https://github.com/ChrisUnsworth/CSPSolver/actions/workflows/dotnet.yml/badge.svg)](https://github.com/ChrisUnsworth/CSPSolver/actions/workflows/dotnet.yml)

# CSPSolver
A simple Constraint Satisfaction Problem Solver (https://en.wikipedia.org/wiki/Constraint_satisfaction_problem)

Example usage:
  ```c#
  using CSPSolver.Model;
  
  ...
  
  var mb = new ModelBuilder();
  
  var x = mb.AddIntDomainVar(1, 5);
  var y = mb.AddIntDomainVar(1, 5);

  mb.AddConstraint(x + y == 6);
  mb.AddConstraint(x < y);

  foreach (var solution in mb.Search())
  {
      Console.WriteLine($"{solution.GetValue(x)} + {solution.GetValue(y)} == 6");
  }
  ```
  
This example first creates a modelbuilder:
  ```c#
  var mb = new ModelBuilder();
  ```    
Then defines two variables with domains of { 1, 2, 3, 4, 5}:
  ```c#
  var x = mb.AddIntDomainVar(1, 5);
  var y = mb.AddIntDomainVar(1, 5);
  ```  
Adds a some simple constraints:
  ```c#
  mb.AddConstraint(x + y == 6);
  mb.AddConstraint(x < y);
  ```  
Then enumerates all the solutions:
  ```c#
  foreach (var solution in mb.Search())
  {
      Console.WriteLine($"{solution.GetValue(x)} + {solution.GetValue(y)} == 6");
  }
  ```  
  
  This is a decision problem (Are there any solutions that respect all the constraints)
  
  To convert this to an optimisaion problem, we add an objective. For example:
  ```c#
  using CSPSolver.Model;
  
  ...
  
  var mb = new ModelBuilder();
  
  mb.AddConstraint(x + y == 6);

  mb.AddObjective(x * y, maximise: true);

  foreach (var solution in mb.Search())
  {
      Console.WriteLine($"{solution.GetValue(x)} + {solution.GetValue(y)} == 6");
  }
  ```
  
