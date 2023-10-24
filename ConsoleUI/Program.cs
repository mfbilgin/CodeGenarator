using Business.Concrete;

{
    //Console.WriteLine("Projeyi oluşturmak istediğiniz dizini giriniz: ");
    var basePath = @"C:\Users\mfbil\OneDrive\Desktop\Test";
    //Console.WriteLine("Proje ismini giriniz: ");
    var solutionName = "TestProj";
    //Console.WriteLine("Proje versiyonunu giriniz: ");
    var solutionVersion = "net7.0";
    //
    // var projectNames = new List<string>()
    // {
    //     "Entities",
    //     "DataAccess",
    //     "Business",
    // };
    //
    var solutionManager = new SolutionManager(basePath);
    var apiManager = new ApiManager(basePath);
    var dataAccessManager = new DataAccessManager(basePath);
    var businessManager = new BusinessManager(basePath);
    var entityManager = new EntityManager(basePath);
    // solutionManager.CreateSolution(solutionName, solutionVersion);
    // Thread.Sleep(1000);
    // solutionManager.CreateSolutionProjects(projectNames, solutionVersion);
    // Thread.Sleep(1000);
    // apiManager.CreateApiProject(solutionVersion);
    //
    //
    // entityManager.GenerateEntityFolders();
    //
    // dataAccessManager.GenerateDataAccessFolders();
    // dataAccessManager.CreateContextFile();
    // dataAccessManager.CreateBaseRepositoryInterface();
    // dataAccessManager.CreateBaseRepositoryClass();
    //
    // businessManager.GenerateBusinessFolders();
    // businessManager.CreateBaseService();
    // businessManager.CreateBusinessModule();
    //
    // Console.WriteLine("How many entities do you want to create?");
    //
    // int entityCount = int.TryParse(Console.ReadLine(), out entityCount) ? entityCount : 0;
    //
    // for (var index = 0; index < entityCount; index++)
    // {
    //     var fields = new List<string>();
    //     Console.WriteLine("Enter the entity name:");
    //     var entityName = Console.ReadLine()!;
    //
    //     while (true)
    //     {
    //         Console.WriteLine("Enter the field tpye and name (ex. int,id)(press enter to exit):");
    //         var field = Console.ReadLine();
    //         if (field!.Equals(string.Empty))
    //             break;
    //         fields.Add(field);
    //     }
    //
    //     entityManager.CreateEntityClass(entityName, fields);
    //
    //     dataAccessManager.CreateRepositoryInterface(entityName);
    //     dataAccessManager.CreateRepositoryClass(entityName);
    //
    //     businessManager.CreateEntityService(entityName);
    //     businessManager.CreateEntityManager(entityName);
    //
    //     apiManager.CreateController(entityName);
    // }
    //
    // dataAccessManager.AddEntityToContext(entityManager.GetEntityNames());
    // businessManager.AddEntityToBusinessModule(entityManager.GetEntityNames());
    // apiManager.CreateProgramCsFile(entityManager.GetEntityNames());
    // //For Entity
    // solutionManager.AddNugetsToProject(entityManager.GetProjectPackages(),entityManager.GetProjectPath());
    // //For DataAccess
    // solutionManager.AddNugetsToProject(dataAccessManager.GetProjectPackages(),dataAccessManager.GetProjectPath());
    // //For Business
    // solutionManager.AddNugetsToProject(businessManager.GetProjectPackages(),businessManager.GetProjectPath());
    // //For API
    // solutionManager.AddNugetsToProject(apiManager.GetProjectPackages(),apiManager.GetProjectPath());
    
    //For Entity
    solutionManager.AddReferencesToProject(entityManager.GetProjectReference(),entityManager.GetProjectPath());
    //For DataAccess
    solutionManager.AddReferencesToProject(dataAccessManager.GetProjectReference(),dataAccessManager.GetProjectPath());
    //For Business
    solutionManager.AddReferencesToProject(businessManager.GetProjectReference(),businessManager.GetProjectPath());
    //For API
    solutionManager.AddReferencesToProject(apiManager.GetProjectReference(),apiManager.GetProjectPath());
    
    dataAccessManager.AddMigration("InitialCreate");
    dataAccessManager.UpdateDatabase();
}