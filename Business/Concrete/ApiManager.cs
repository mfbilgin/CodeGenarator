using System.Diagnostics;
using Business.Abstract;

namespace Business.Concrete;

public class ApiManager : IApiService
{
    private readonly string _controllersPath;
    private readonly string _basePath;
    private readonly string _folderPath;
    private readonly string _projectPath;
    private readonly List<string> _packages;
    private readonly List<string> _projectReference;

    public ApiManager(string basePath)
    {
        _basePath = basePath;
        _folderPath = Path.Combine(_basePath, "WebAPI");
        _controllersPath = Path.Combine(_folderPath, "Controllers");
        _projectPath = Path.Combine(_folderPath, "WebAPI.csproj");
        _packages = new List<string>()
        {
            "Autofac.Extensions.DependencyInjection",
            "Microsoft.AspNetCore.Authentication.JwtBearer",
            "Microsoft.EntityFrameworkCore.Tools",
            "Swashbuckle.AspNetCore",
        };
        _projectReference = new List<string>()
        {
            "Business",
            "DataAccess",
            "Entities",
        };
    }

    public void CreateApiProject(string solutionVersion)
    {
        if (!Directory.Exists(_folderPath)) Directory.CreateDirectory(_folderPath);
        var baseSolutionPath = Directory.GetFiles(_basePath, "*.sln").FirstOrDefault();
        var projectPath = Directory.GetFiles(_folderPath, "*.csproj").FirstOrDefault();
        if (File.Exists(projectPath))
        {
            Console.WriteLine("WebAPI project already exists!");
            return;
        }


        Process.Start("dotnet", $"new webapi  -n WebAPI -o {_folderPath}");
        Thread.Sleep(3000);
        if (projectPath == null)
        {
            Thread.Sleep(500);
            projectPath = Directory.GetFiles(_folderPath, "*.csproj").FirstOrDefault();
        }

        Process.Start("dotnet", $"sln {baseSolutionPath} add {projectPath}");
        Thread.Sleep(1000);
        DeleteDefaultClasses();
    }

    public void DeleteDefaultClasses()
    {
        var defaultClassPath = _folderPath + @"\WeatherForecast.cs";
        if (File.Exists(defaultClassPath)) File.Delete(defaultClassPath);
        //Console.WriteLine(_controllersPath);
        var defaultControllerClassPath = _controllersPath + @"\WeatherForecastController.cs";
        File.Delete(defaultControllerClassPath);
        GenerateControllersFolder();
    }

    private void GenerateControllersFolder()
    {
        var controllersPath = _controllersPath;
        if (Directory.Exists(controllersPath)) return;
        Directory.CreateDirectory(controllersPath);
        Console.WriteLine($"{_controllersPath} folder created.");
    }

    public void CreateController(string className)
    {
        var controllerPath = Path.Combine(_controllersPath, $"{className}sController.cs");
        using var writer = new StreamWriter(controllerPath);
        writer.WriteLine($"using Business.Abstract;");
        writer.WriteLine($"using Entities.Concrete;");
        writer.WriteLine($"using Microsoft.AspNetCore.Mvc;");
        writer.WriteLine();
        writer.WriteLine("namespace WebAPI.Controllers");
        writer.WriteLine("{");
        writer.WriteLine($"    [ApiController]");
        writer.WriteLine($"    [Route(\"api/[controller]\")]");
        writer.WriteLine($"    public class {className}sController : ControllerBase");
        writer.WriteLine("    {");
        writer.WriteLine($"        private readonly I{className}Service _{className.ToLower()}Service;");
        writer.WriteLine();
        writer.WriteLine($"        public {className}sController(I{className}Service {className.ToLower()}Service)");
        writer.WriteLine("        {");
        writer.WriteLine($"            _{className.ToLower()}Service = {className.ToLower()}Service;");
        writer.WriteLine("        }");
        writer.WriteLine();
        writer.WriteLine($"        [HttpGet]");
        writer.WriteLine($"        public IActionResult GetAll()");
        writer.WriteLine("        {");
        writer.WriteLine($"            var result = _{className.ToLower()}Service.GetAll();");
        writer.WriteLine($"            return Ok(result);");
        writer.WriteLine("        }");
        writer.WriteLine();
        writer.WriteLine($"        [HttpGet(\"getbyid\")]");
        writer.WriteLine($"        public IActionResult GetById([FromQuery]int {className.ToLower()}Id)");
        writer.WriteLine("        {");
        writer.WriteLine($"            var result = _{className.ToLower()}Service.GetById({className.ToLower()}Id);");
        writer.WriteLine($"            return Ok(result);");
        writer.WriteLine("        }");
        writer.WriteLine();
        writer.WriteLine($"        [HttpPost]");
        writer.WriteLine($"        public IActionResult Add([FromBody]{className} {className.ToLower()})");
        writer.WriteLine("        {");
        writer.WriteLine($"            _{className.ToLower()}Service.Add({className.ToLower()});");
        writer.WriteLine($"            return Ok();");
        writer.WriteLine("        }");
        writer.WriteLine();
        writer.WriteLine($"        [HttpPut]");
        writer.WriteLine($"        public IActionResult Update([FromBody]{className} {className.ToLower()})");
        writer.WriteLine("        {");
        writer.WriteLine($"            _{className.ToLower()}Service.Update({className.ToLower()});");
        writer.WriteLine($"            return Ok();");
        writer.WriteLine("        }");
        writer.WriteLine();
        writer.WriteLine($"        [HttpDelete]");
        writer.WriteLine($"        public IActionResult Delete([FromBody]{className} {className.ToLower()})");
        writer.WriteLine("        {");
        writer.WriteLine($"            _{className.ToLower()}Service.Delete({className.ToLower()});");
        writer.WriteLine($"            return Ok();");
        writer.WriteLine("        }");
        writer.WriteLine("    }");
        writer.WriteLine("}");
        Console.WriteLine($"{className}sController.cs class has been created successfully!");
    }

    public void CreateProgramCsFile(List<string?> entities)
    {
        // Dosya yolunu ve ismini belirle
        var filePath = Path.Combine(_folderPath, "Program.cs");
        using var writer = new StreamWriter(filePath);
        writer.WriteLine("using Business.Abstract;");
        writer.WriteLine("using Business.Concrete;");
        writer.WriteLine("using DataAccess.Abstract;");
        writer.WriteLine("using DataAccess.Concrete;");
        writer.WriteLine();
        writer.WriteLine("var builder = WebApplication.CreateBuilder(args);");
        writer.WriteLine();
        writer.WriteLine("builder.Services.AddControllers();");
        writer.WriteLine("builder.Services.AddEndpointsApiExplorer();");
        writer.WriteLine("builder.Services.AddSwaggerGen();");
        writer.WriteLine();
        foreach (var entity in entities)
        {
            writer.WriteLine($"builder.Services.AddScoped<I{entity}Service, {entity}Manager>();");
            writer.WriteLine($"builder.Services.AddScoped<I{entity}Repository, Ef{entity}Repository>();");
        }

        writer.WriteLine();
        writer.WriteLine("var app = builder.Build();");
        writer.WriteLine();
        writer.WriteLine("if (app.Environment.IsDevelopment())");
        writer.WriteLine("{");
        writer.WriteLine("    app.UseSwagger();");
        writer.WriteLine("    app.UseSwaggerUI();");
        writer.WriteLine("}");
        writer.WriteLine("app.UseHttpsRedirection();");
        writer.WriteLine();
        writer.WriteLine("app.UseRouting();");
        writer.WriteLine();
        writer.WriteLine("app.UseStaticFiles();");
        writer.WriteLine();
        writer.WriteLine("app.UseAuthentication();");
        writer.WriteLine();
        writer.WriteLine("app.UseAuthorization();");
        writer.WriteLine();
        writer.WriteLine("app.UseEndpoints(endpoints =>");
        writer.WriteLine("{");
        writer.WriteLine("    endpoints.MapControllers();");
        writer.WriteLine("});");
        writer.WriteLine();
        writer.WriteLine("app.Run();");
    }

    public string GetControllerPath()
    {
        return _controllersPath;
    }

    public string GetProgramCsPath()
    {
        return Path.Combine(_folderPath, "Program.cs");
    }

    public string GetProjectPath()
    {
        return _projectPath;
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