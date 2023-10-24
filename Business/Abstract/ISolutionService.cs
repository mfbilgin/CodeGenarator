namespace Business.Abstract;

public interface ISolutionService
{
    void CreateSolution(string solutionName, string solutionVersion);
    void CreateSolutionProjects(List<string> projectNames, string solutionVersion);
    void AddNugetsToProject(List<string> packages, string projectPath);
    void AddReferencesToProject(List<string> references, string projectPath);
}