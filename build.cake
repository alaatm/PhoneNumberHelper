//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var binDir = Directory("./src/bin") + Directory(configuration);
var packDir = Directory("./dist/");

Setup(context =>
{
});

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(binDir);
    CleanDirectory(packDir);
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetCoreRestore(".");
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
	DotNetCoreBuild(".", new DotNetCoreBuildSettings
	{
		Configuration = configuration,
	});
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
	DotNetCoreTest(".", new DotNetCoreTestSettings 
	{ 
		Configuration = configuration,
	});
});

Task("Pack")
	.IsDependentOn("Test")
	.Does(() =>
{
	DotNetCorePack(".", new DotNetCorePackSettings 
	{ 
		Configuration = configuration,
		OutputDirectory = packDir,
		NoBuild = true,
	});
});

Task("NugetPush")
    .IsDependentOn("Pack")
    .Does(() =>
{
    var nugetPath = GetFiles("./dist/PhoneNumberHelper*.nupkg").Single();

    var settings = new DotNetCoreNuGetPushSettings
    {
        Source = "https://api.nuget.org/v3/index.json",
        ApiKey = EnvironmentVariable("NUGET_API_KEY"),
    };
    
    DotNetCoreNuGetPush(nugetPath.ToString(), settings);
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Test");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);


//////////////////////////////////////////////////////////////////////
// HELPER METHODS
//////////////////////////////////////////////////////////////////////

string ReadFile(string path)
{
	return System.IO.File.ReadAllText(path);
}
