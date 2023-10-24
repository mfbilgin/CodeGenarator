namespace Business.Abstract;

public interface IBusinessService
{
    void AddNugetToProject();
    void GenerateBusinessFolders();
    void CreateBaseService();
    void CreateEntityService(string className);
    void CreateEntityManager(string className);
    void CreateBusinessModule();
    void AddEntityToBusinessModule(List<string?> entities);
    string GetAbstractPath();
    string GetConcretePath();
    string GetDependencyResolversPath();
}