namespace Business.Abstract;

public interface IEntityService
{
    void CreateEntityClass(string entityName, IEnumerable<string> fields);
    void GenerateEntityFolders();
    string GetConcretePath();
    string GetDtoPath();
    string GetProjectPath();
    List<string?> GetEntityNames();
    List<string> GetProjectPackages();
    List<string> GetProjectReference();
}