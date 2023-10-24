using System.Diagnostics;
using Business.Abstract;

namespace Business.Concrete;

public class SolutionManager : ISolutionService
{
    private readonly string _basePath;

    public SolutionManager(string basePath)
    {
        _basePath = basePath;
    }

    public void CreateSolution(string solutionName, string solutionVersion)
    {
        if (File.Exists(_basePath + "\\" + solutionName + ".sln"))
        {
            Console.WriteLine("Solution already exists!");
            return;
        }
        var thread = new Thread(() =>
        {
            Process.Start("dotnet", $"new sln -n {solutionName} -o {_basePath}");
        });
        thread.Start();
        thread.Join();
        Console.WriteLine($"{solutionName}.sln has been created successfully!");
        Console.WriteLine(_basePath);
    }

    public void CreateSolutionProjects(List<string> projectNames, string solutionVersion)
    {
        projectNames.ForEach(projectName => CreateProject(solutionVersion, projectName));
    }

    private void CreateProject(string solutionVersion, string projectName)
    {
        var baseSolutionPath = Directory.GetFiles(_basePath, "*.sln").FirstOrDefault();
        var projectFolderPath = Path.Combine(_basePath, projectName);
        var projectPath = Path.Combine(projectFolderPath, $"{projectName}.csproj");
        if (File.Exists(projectPath))
        {
            Console.WriteLine($"{projectName} project already exists!");
            return;
        }
        
        Process.Start("dotnet", $"new classlib -o {projectFolderPath} -n {projectName} -f {solutionVersion}");
        Thread.Sleep(750);
        
        Process.Start("dotnet", $"sln {baseSolutionPath} add {projectPath}");
        Thread.Sleep(750);
        
        var class1Path = Path.Combine(projectFolderPath, "Class1.cs");
        File.Delete(class1Path);
    }
}