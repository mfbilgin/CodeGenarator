namespace Business.Abstract;

public interface ISolutionService
{
    void CreateSolution(string solutionName, string solutionVersion);
    void CreateSolutionProjects(List<string> projectNames, string solutionVersion);
}