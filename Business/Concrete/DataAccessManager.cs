using System.Diagnostics;
using Business.Abstract;

namespace Business.Concrete;

public class DataAccessManager : IDataAccessService
{
    private readonly string _concretePath;
    private readonly string _abstractPath;
    private readonly string _migrationPath;
    private readonly string _folderPath;
    private readonly string _projectPath;

    private readonly List<string> _packages;
    public DataAccessManager(string basePath)
    {
        _folderPath = Path.Combine(basePath, "DataAccess");
        _migrationPath = Path.Combine(_folderPath, "Migrations");
        _concretePath = Path.Combine(_folderPath, "Concrete");
        _abstractPath = Path.Combine(_folderPath, "Abstract");
        _projectPath = Path.Combine(_folderPath, "DataAccess.csproj");
        _packages = new List<string>()
        {
            "Microsoft.EntityFrameworkCore",
            "Microsoft.EntityFrameworkCore.SqlServer",
            "Microsoft.AspNetCore.Http",
            "Microsoft.AspNetCore.Hosting"
        };
    }
    public void AddNugetsToProject()
    {   
        if (!File.Exists(_projectPath))
            return;
  
        _packages.ForEach(AddNuget);

    }

    public void GenerateDataAccessFolders()
    {
        //Eğer yoksa klasörleri oluşturalım
        if (!Directory.Exists(_concretePath))
        {
            Directory.CreateDirectory(_concretePath);
            Console.WriteLine($"\"{_concretePath}\" klasörü oluşturuldu.");
        }

        if (Directory.Exists(_abstractPath)) return;
        Directory.CreateDirectory(_abstractPath);
        Console.WriteLine($"\"{_abstractPath}\" folder created.");
    }

    public void CreateContextFile()
    {
        var contextPath = Path.Combine(_concretePath, "Context");
        if (!Directory.Exists(contextPath))
        {
            Directory.CreateDirectory(contextPath);
            Console.WriteLine($"\"{contextPath}\" folder created.");
        }

        // Console.WriteLine("Server Name : ");
        // Console.WriteLine("Database Name : ");
        
        var connectionString = $@"Server=MFBILGIN\MFBILGIN;Database=TestDb;Trusted_Connection=true\";

        using var writer = new StreamWriter(contextPath + "\\DatabaseContext.cs");
        writer.WriteLine("using Microsoft.EntityFrameworkCore;");
        writer.WriteLine("using Entities.Concrete;");
        writer.WriteLine();
        writer.WriteLine("namespace DataAccess.Concrete.Context");
        writer.WriteLine("{");
        writer.WriteLine("    public class DatabaseContext : DbContext");
        writer.WriteLine("    {");
        writer.WriteLine("        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)");
        writer.WriteLine("        {");
        writer.WriteLine($"            optionsBuilder.UseSqlServer({connectionString});");
        writer.WriteLine("        }");
        writer.WriteLine();
        writer.WriteLine("    }");
        writer.WriteLine("}");
        Console.WriteLine($"Context file created to {contextPath} location.");
    }

    public void CreateRepositoryInterface(string className)
    {
        var interfacePath = Path.Combine(_abstractPath, "I" + className + "Repository.cs");
        using var writer = new StreamWriter(interfacePath);
        writer.WriteLine($"using Entities.Concrete;");
        writer.WriteLine();
        writer.WriteLine("namespace DataAccess.Abstract");
        writer.WriteLine("{");
        writer.WriteLine($"    public interface I{className}Repository : IEntityRepository<{className}>");
        writer.WriteLine("    {");
        writer.WriteLine("    }");
        writer.WriteLine("}");
        Console.Write($"I{className}Repository.cs file created.");
    }

    public void CreateRepositoryClass(string className)
    {
        var classPath = Path.Combine(_concretePath, $"Ef{className}Repository.cs");
        using var writer = new StreamWriter(classPath);
        writer.WriteLine("using DataAccess.Abstract;");
        writer.WriteLine("using DataAccess.Concrete.Context;");
        writer.WriteLine("using Entities.Concrete;");
        writer.WriteLine();
        writer.WriteLine("namespace DataAccess.Concrete");
        writer.WriteLine("{");
        writer.WriteLine(
            $"    public class Ef{className}Repository : EfEntityRepositoryBase<{className}, DatabaseContext>, I{className}Repository");
        writer.WriteLine("    {");
        writer.WriteLine("    }");
        writer.WriteLine("}");
        Console.WriteLine($"Ef{className}Repository.cs file created.");
    }

    public void CreateBaseRepositoryInterface()
    {
        var interfacePath = _abstractPath + "\\IEntityRepository.cs";
        using var writer = new StreamWriter(interfacePath);

        writer.WriteLine("using System.Linq.Expressions;");
        writer.WriteLine();
        writer.WriteLine("namespace DataAccess.Abstract");
        writer.WriteLine("{");
        writer.WriteLine("    public interface IEntityRepository<T> where T : class, new()");
        writer.WriteLine("    {");
        writer.WriteLine("        List<T> GetAll(Expression<Func<T, bool>> filter = null);");
        writer.WriteLine("        T Get(Expression<Func<T, bool>> filter);");
        writer.WriteLine("        void Add(T entity);");
        writer.WriteLine("        void Update(T entity);");
        writer.WriteLine("        void Delete(T entity);");
        writer.WriteLine("    }");
        writer.WriteLine("}");
        Console.WriteLine("IEntityRepository file created.");
    }

    public void CreateBaseRepositoryClass()
    {
        var classPath = _concretePath + "\\EfEntityRepositoryBase.cs";
        using var writer = new StreamWriter(classPath);
        writer.WriteLine("using System.Linq.Expressions;");
        writer.WriteLine("using DataAccess.Abstract;");
        writer.WriteLine("using Microsoft.EntityFrameworkCore;");
        writer.WriteLine();
        writer.WriteLine("namespace DataAccess.Concrete");
        writer.WriteLine("{");
        writer.WriteLine("    public class EfEntityRepositoryBase<T, TContext> : IEntityRepository<T>");
        writer.WriteLine("        where T : class, new()");
        writer.WriteLine("        where TContext : DbContext, new()");
        writer.WriteLine("    {");
        writer.WriteLine("        public List<T> GetAll(Expression<Func<T, bool>> filter = null)");
        writer.WriteLine("        {");
        writer.WriteLine("            using var context = new TContext();");
        writer.WriteLine("            return filter == null");
        writer.WriteLine("                ? context.Set<T>().ToList()");
        writer.WriteLine("                : context.Set<T>().Where(filter).ToList();");
        writer.WriteLine("        }");
        writer.WriteLine();
        writer.WriteLine("        public T Get(Expression<Func<T, bool>> filter)");
        writer.WriteLine("        {");
        writer.WriteLine("            using var context = new TContext();");
        writer.WriteLine("            return context.Set<T>().SingleOrDefault(filter);");
        writer.WriteLine("        }");
        writer.WriteLine();
        writer.WriteLine("        public void Add(T entity)");
        writer.WriteLine("        {");
        writer.WriteLine("            using var context = new TContext();");
        writer.WriteLine("            var addedEntity = context.Entry(entity);");
        writer.WriteLine("            addedEntity.State = EntityState.Added;");
        writer.WriteLine("            context.SaveChanges();");
        writer.WriteLine("        }");
        writer.WriteLine();
        writer.WriteLine("        public void Update(T entity)");
        writer.WriteLine("        {");
        writer.WriteLine("            using var context = new TContext();");
        writer.WriteLine("            var updatedEntity = context.Entry(entity);");
        writer.WriteLine("            updatedEntity.State = EntityState.Modified;");
        writer.WriteLine("            context.SaveChanges();");
        writer.WriteLine("        }");
        writer.WriteLine();
        writer.WriteLine("        public void Delete(T entity)");
        writer.WriteLine("        {");
        writer.WriteLine("            using var context = new TContext();");
        writer.WriteLine("            var deletedEntity = context.Entry(entity);");
        writer.WriteLine("            deletedEntity.State = EntityState.Deleted;");
        writer.WriteLine("            context.SaveChanges();");
        writer.WriteLine("        }");
        writer.WriteLine("    }");
        writer.WriteLine("}");
        Console.WriteLine("EfEntityRepositoryBase file created.");
    }

    public void AddEntityToContext(List<string?> entites)
    {
        var contextPath = Path.Combine(GetConcretePath(), "Context\\DatabaseContext.cs");

        var lines = File.ReadAllLines(contextPath);
        File.WriteAllLines(contextPath, lines.Take(11).ToArray());

        var newLines = entites.Select(entite => $"        public DbSet<{entite}> {entite}s {{ get; set; }}").ToList();
        newLines.Add("    }");
        newLines.Add("}");
        File.AppendAllLines(contextPath, newLines);
        Console.WriteLine("Entites added to context file.");
    }

    public void AddMigration(string migrationName)
    {
        var process = new Process();

        process.StartInfo.FileName = "dotnet";
        process.StartInfo.Arguments = $"ef migrations add {migrationName}";
        process.StartInfo.WorkingDirectory = _folderPath;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.Start();
        process.WaitForExit();
        Console.WriteLine(process.StandardOutput.ReadToEnd());
    }

    public void UpdateDatabase()
    {
        var process = new Process();
        process.StartInfo.FileName = "dotnet";
        process.StartInfo.Arguments = "ef database update";
        process.StartInfo.WorkingDirectory = _folderPath;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.Start();
        process.WaitForExit();
        Console.WriteLine(process.StandardOutput.ReadToEnd());
    }

    public string GetAbstractPath()
    {
        return _abstractPath;
    }

    public string GetConcretePath()
    {
        return _concretePath;
    }

    public string GetMigrationPath()
    {
        return _migrationPath;
    }

    private void AddNuget(string packageName)
    {
        var process = new Process();
        process.StartInfo.FileName = "dotnet";
        process.StartInfo.Arguments = $"add {_projectPath} package {packageName}"; 
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.Start();
        Console.WriteLine(process.StandardOutput.ReadToEnd());
    }
}