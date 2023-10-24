namespace Business.Abstract;

public interface IEntityService
{
    void CreateEntityClass(string entityName, IEnumerable<string> fields);
    void GenerateEntityFolders();
    string GetConcretePath();
    string GetDtoPath();
    List<string?> GetEntityNames();
}