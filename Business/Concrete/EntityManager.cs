using Business.Abstract;

namespace Business.Concrete;

public class EntityManager : IEntityService
{
    private readonly string _concretePath;
    private readonly string _dtoPath;
    private readonly string _projectPath;
    private readonly List<string> _packages;
    private readonly List<string> _projectReference;

    public EntityManager(string basePath)
    {
        var folderPath = Path.Combine(basePath, "Entities");
        _projectPath = Path.Combine(folderPath, "Entities.csproj");
        _concretePath = Path.Combine(folderPath, "Concrete");
        _dtoPath = Path.Combine(folderPath, "DTOs");
        _packages = new List<string>()
        {
            "Microsoft.AspNetCore.Hosting",
            "Microsoft.AspNetCore.Http"
        };
        _projectReference = new List<string>()
        {
        };
    }

    public void CreateEntityClass(string entityName, IEnumerable<string> fields)
    {
        var path = _concretePath + "\\" + entityName + ".cs";
        using (var writer = new StreamWriter(path))
        {
            writer.WriteLine("namespace Entities.Concrete");
            writer.WriteLine("{");
            writer.WriteLine($"    public class {entityName}");
            writer.WriteLine("    {");
            foreach (var fieldParts in fields.Select(field => field.Split(',')))
            {
                writer.WriteLine($"        public {fieldParts[0]} {fieldParts[1]} {{ get; set; }}");
            }
            writer.WriteLine("    }");
            writer.WriteLine("}");
        }

        Console.WriteLine($"{entityName}.cs class has been created successfully!");
    }

    public void GenerateEntityFolders()
    {
        // Eğer yoksa klasörleri oluşturalım.
        if (!Directory.Exists(_dtoPath))
        {
            Directory.CreateDirectory(_dtoPath);
            Console.WriteLine($"\"{_dtoPath}\" klasörü oluşturuldu.");
        }

        if (Directory.Exists(_concretePath)) return;
        Directory.CreateDirectory(_concretePath);
        Console.WriteLine($"\"{_concretePath}\" klasörü oluşturuldu.");
    }

    public string GetConcretePath()
    {
        return _concretePath;
    }

    public string GetDtoPath()
    {
        return _dtoPath;
    }

    public string GetProjectPath()
    {
        return _projectPath;
    }

    public List<string?> GetEntityNames()
    {
        return Directory.GetFiles(_concretePath, "*.cs").Select(Path.GetFileNameWithoutExtension).ToList();
    }

    public List<string> GetProjectPackages()
    {
        return _packages;
    }

    public List<string> GetProjectReference()
    {
        return _projectReference;
    }
}