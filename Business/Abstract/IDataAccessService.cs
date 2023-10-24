namespace Business.Abstract;

public interface IDataAccessService
{
    void AddNugetsToProject();
    void GenerateDataAccessFolders();
    void CreateContextFile();
    void CreateRepositoryInterface(string className);
    void CreateRepositoryClass(string className);
    void CreateBaseRepositoryInterface();
    void CreateBaseRepositoryClass();
    void AddEntityToContext(List<string?> entities);
    void AddMigration(string migrationName);
    void UpdateDatabase();
    string GetAbstractPath();
    string GetConcretePath();
    string GetMigrationPath();
}