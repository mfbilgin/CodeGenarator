using Business.Abstract;

namespace Business.Concrete;

public class BusinessManager : IBusinessService
{
    private readonly string _concretePath;
    private readonly string _abstractPath;
    private readonly string _dependecyResolversPath;
    private readonly string _projectPath;
    private readonly List<string> _packages;
    private readonly List<string> _projectReference;
    public BusinessManager(string basePath)
    {
        basePath = Path.Combine(basePath, "Business");
        _concretePath = Path.Combine(basePath, "Concrete");
        _abstractPath = Path.Combine(basePath, "Abstract");
        _dependecyResolversPath = Path.Combine(basePath, "DependencyResolvers");
        _projectPath = Path.Combine(basePath,"Business.csproj");
        _packages = new List<string>()
        {   
            "Autofac",
            "Autofac.Extensions.DependencyInjection",
            "Autofac.Extras.DynamicProxy",
            "FluentValidation",
            "Microsoft.AspNetCore.Http.Abstractions",
            "Microsoft.AspNetCore.Http.Features",
            "Microsoft.Extensions.DependencyInjection"
        };
        _projectReference = new List<string>()
        {
            "DataAccess",
            "Entities",
        };
    }

    public void GenerateBusinessFolders()
    {
        if (!Directory.Exists(_concretePath))
        {
            Directory.CreateDirectory(_concretePath);
            Console.WriteLine($"\"{_concretePath}\" klasörü oluşturuldu.");
        }

        if (!Directory.Exists(_dependecyResolversPath))
        {
            Directory.CreateDirectory(_dependecyResolversPath);
            Console.WriteLine($"\"{_dependecyResolversPath}\" klasörü oluşturuldu.");
        }

        if (Directory.Exists(_abstractPath)) return;
        Directory.CreateDirectory(_abstractPath);
        Console.WriteLine($"\"{_abstractPath}\" folder created.");
    }

    public void CreateBaseService()
    {
        var baseServicePath = Path.Combine(_abstractPath, "IEntityService.cs");
        using var writer = new StreamWriter(baseServicePath);
        writer.WriteLine("namespace Business.Abstract");
        writer.WriteLine("{");
        writer.WriteLine("    public interface IEntityService<T> where T : class, new()");
        writer.WriteLine("    {");
        writer.WriteLine("        List<T> GetAll();");
        writer.WriteLine("        T GetById(int id);");
        writer.WriteLine("        void Add(T entity);");
        writer.WriteLine("        void Update(T entity);");
        writer.WriteLine("        void Delete(T entity);");
        writer.WriteLine("    }");
        writer.WriteLine("}");
        Console.WriteLine($"Base service file created to {baseServicePath} location.");
    }

    public void CreateEntityService(string className)
    {
        var entityServicePath = Path.Combine(_abstractPath, $"I{className}Service.cs");
        using var writer = new StreamWriter(entityServicePath);
        writer.WriteLine($"using Entities.Concrete;");
        writer.WriteLine();
        writer.WriteLine("namespace Business.Abstract");
        writer.WriteLine("{");
        writer.WriteLine($"    public interface I{className}Service : IEntityService<{className}>");
        writer.WriteLine("    {");
        writer.WriteLine("    }");
        writer.WriteLine("}");
    }

    public void CreateEntityManager(string className)
    {
        var entityManagerPath = Path.Combine(_concretePath, $"{className}Manager.cs");
        using var writer = new StreamWriter(entityManagerPath);
        writer.WriteLine($"using Business.Abstract;");
        writer.WriteLine($"using DataAccess.Abstract;");
        writer.WriteLine($"using Entities.Concrete;");
        writer.WriteLine();
        writer.WriteLine("namespace Business.Concrete");
        writer.WriteLine("{");
        writer.WriteLine($"    public class {className}Manager : I{className}Service");
        writer.WriteLine("    {");
        writer.WriteLine($"        private readonly I{className}Repository _{className.ToLower()}Repository;");
        writer.WriteLine();
        writer.WriteLine($"        public {className}Manager(I{className}Repository {className.ToLower()}Repository)");
        writer.WriteLine("        {");
        writer.WriteLine($"            _{className.ToLower()}Repository = {className.ToLower()}Repository;");
        writer.WriteLine("        }");
        writer.WriteLine();
        writer.WriteLine("        public List<" + className + "> GetAll()");
        writer.WriteLine("        {");
        writer.WriteLine("            return _" + className.ToLower() + "Repository.GetAll();");
        writer.WriteLine("        }");
        writer.WriteLine();
        writer.WriteLine("        public " + className + " GetById(int id)");
        writer.WriteLine("        {");
        writer.WriteLine("            return _" + className.ToLower() + $"Repository.Get(x=> x.{className}Id == id);");
        writer.WriteLine("        }");
        writer.WriteLine();
        writer.WriteLine("        public void Add(" + className + " entity)");
        writer.WriteLine("        {");
        writer.WriteLine("            _" + className.ToLower() + "Repository.Add(entity);");
        writer.WriteLine("        }");
        writer.WriteLine();
        writer.WriteLine("        public void Update(" + className + " entity)");
        writer.WriteLine("        {");
        writer.WriteLine("            _" + className.ToLower() + "Repository.Update(entity);");
        writer.WriteLine("        }");
        writer.WriteLine();
        writer.WriteLine("        public void Delete(" + className + " entity)");
        writer.WriteLine("        {");
        writer.WriteLine("            _" + className.ToLower() + "Repository.Delete(entity);");
        writer.WriteLine("        }");
        writer.WriteLine("    }");
        writer.WriteLine("}");
        Console.WriteLine($"Entity manager file created to {entityManagerPath} location.");
    }

    public void CreateBusinessModule()
    {
        using var writer = new StreamWriter(_dependecyResolversPath + "\\AutofacBusinessModule.cs");
        writer.WriteLine("using Autofac;");
        writer.WriteLine("using Autofac.Extras.DynamicProxy;");
        writer.WriteLine("using Business.Abstract;");
        writer.WriteLine("using Business.Concrete;");
        writer.WriteLine("using Core.Utilities.Interceptors;");
        writer.WriteLine("using DataAccess.Abstract;");
        writer.WriteLine("using DataAccess.Concrete;");
        writer.WriteLine();
        writer.WriteLine("namespace Business.DependencyResolvers.Autofac");
        writer.WriteLine("{");
        writer.WriteLine("    public class AutofacBusinessModule : Module");
        writer.WriteLine("    {");
        writer.WriteLine("        protected override void Load(ContainerBuilder builder)");
        writer.WriteLine("        {");
        writer.WriteLine();
        writer.WriteLine("        }");
        writer.WriteLine("    }");
        writer.WriteLine("}");
        Console.WriteLine($"Business module file created to {_dependecyResolversPath} location.");
    }

    public void AddEntityToBusinessModule(List<string?> entities)
    {
        var modulePath = Path.Combine(_dependecyResolversPath, "AutofacBusinessModule.cs");

        var lines = File.ReadAllLines(modulePath);
        File.WriteAllLines(modulePath, lines.Take(14).ToArray());

        var newLines = new List<string>();
        foreach (var entity in entities)
        {
            newLines.Add($"            builder.RegisterType<{entity}Manager>().As<I{entity}Service>();");
            newLines.Add($"            builder.RegisterType<Ef{entity}Repository>().As<I{entity}Repository>();");
        }

        newLines.Add("            var assembly = System.Reflection.Assembly.GetExecutingAssembly();");
        newLines.Add("            builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces()");
        newLines.Add("                .EnableInterfaceInterceptors(new Castle.DynamicProxy.ProxyGenerationOptions()");
        newLines.Add("                {");
        newLines.Add("                    Selector = new AspectInterceptorSelector()");
        newLines.Add("                }).SingleInstance();");
        newLines.Add("        }");
        newLines.Add("    }");
        newLines.Add("}");
        File.AppendAllLines(modulePath, newLines);
        Console.WriteLine("Entites added to module file.");
    }

    public string GetAbstractPath()
    {
        return _abstractPath;
    }

    public string GetConcretePath()
    {
        return _concretePath;
    }

    public string GetProjectPath()
    {
        return _projectPath;
    }

    public string GetDependencyResolversPath()
    {
        return _dependecyResolversPath;
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