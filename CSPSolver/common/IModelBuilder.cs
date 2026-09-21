namespace CSPSolver.common;

public interface IModelBuilder
{
    IModel GetModel();

    int GetStateSize();
}