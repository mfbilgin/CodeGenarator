namespace Business.Abstract;

public interface IApiService
{
    void CreateApiProject(string solutionVersion);
    void DeleteDefaultClasses();
    void CreateController(string className);
    void CreateProgramCsFile(List<string?> entities);
    string GetControllerPath();
    string GetProgramCsPath();
}