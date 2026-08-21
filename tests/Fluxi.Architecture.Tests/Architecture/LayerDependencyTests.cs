using System.Xml.Linq;

namespace Fluxi.Architecture.Tests.Architecture;

[Trait(TestTraits.Category, TestTraits.ArchitectureCategory)]
[Trait(TestTraits.Layer, TestTraits.ArchitectureLayer)]
[Trait(TestTraits.Feature, TestTraits.DependenciesFeature)]
public sealed class LayerDependencyTests
{
    #region Tests

    [Theory]
    [InlineData("Fluxi.SharedKernel")]
    [InlineData("Fluxi.Domain")]
    [InlineData("Fluxi.Application")]
    [InlineData("Fluxi.Infrastructure")]
    [InlineData("Fluxi.Api")]
    public void Layer_ShouldReferenceOnlyAllowedFluxiProjects(string layerName)
    {
        // Arrange
        string projectFile = GetProjectFile(layerName);
        IReadOnlySet<string> allowedDependencies = GetAllowedDependencies(layerName);

        // Act
        string[] projectDependencies = GetProjectDependencies(projectFile);
        string[] forbiddenDependencies = projectDependencies
            .Where(dependency => !allowedDependencies.Contains(dependency))
            .ToArray();

        // Assert
        Assert.True(
            forbiddenDependencies.Length == 0,
            $"Layer '{layerName}' has forbidden Fluxi dependencies: "
                + string.Join(", ", forbiddenDependencies));
    }

    [Fact]
    public void Architecture_ShouldNotContainDependencyCycles()
    {
        // Arrange
        Dictionary<string, string[]> dependencyGraph = GetDependencyGraph();

        // Act
        string[] cyclicProjects = dependencyGraph.Keys
            .Where(project => HasDependencyCycle(project, dependencyGraph, [], []))
            .ToArray();

        // Assert
        Assert.Empty(cyclicProjects);
    }

    #endregion

    #region Helpers

    private static string[] GetProjectDependencies(string projectFile)
    {
        XDocument project = XDocument.Load(projectFile);

        return project
            .Descendants("ProjectReference")
            .Select(reference => reference.Attribute("Include")?.Value)
            .Where(path => path is not null)
            .Select(path => Path.GetFileNameWithoutExtension(path!.Replace('\\', Path.DirectorySeparatorChar)))
            .Where(name => name.StartsWith("Fluxi.", StringComparison.Ordinal))
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();
    }

    private static string GetProjectFile(string layerName)
    {
        string projectFile = Path.Combine(GetRepositoryRoot(), "src", layerName, $"{layerName}.csproj");

        Assert.True(File.Exists(projectFile), $"Project file was not found: {projectFile}");
        return projectFile;
    }

    private static string GetRepositoryRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);

        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Fluxi.slnx")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName
            ?? throw new DirectoryNotFoundException("Could not locate the repository root.");
    }

    private static Dictionary<string, string[]> GetDependencyGraph()
    {
        string[] layers =
        [
            "Fluxi.SharedKernel",
            "Fluxi.Domain",
            "Fluxi.Application",
            "Fluxi.Infrastructure",
            "Fluxi.Api"
        ];

        return layers.ToDictionary(
            layer => layer,
            layer => GetProjectDependencies(GetProjectFile(layer)),
            StringComparer.Ordinal);
    }

    private static bool HasDependencyCycle(
        string project,
        IReadOnlyDictionary<string, string[]> dependencyGraph,
        HashSet<string> visited,
        HashSet<string> currentPath)
    {
        if (currentPath.Contains(project))
        {
            return true;
        }

        if (!visited.Add(project))
        {
            return false;
        }

        currentPath.Add(project);

        bool hasCycle = dependencyGraph[project]
            .Where(dependencyGraph.ContainsKey)
            .Any(dependency => HasDependencyCycle(dependency, dependencyGraph, visited, currentPath));

        currentPath.Remove(project);
        return hasCycle;
    }

    private static IReadOnlySet<string> GetAllowedDependencies(string layerName)
    {
        return layerName switch
        {
            "Fluxi.SharedKernel" => new HashSet<string>(StringComparer.Ordinal),
            "Fluxi.Domain" =>
                new HashSet<string>(["Fluxi.SharedKernel"], StringComparer.Ordinal),
            "Fluxi.Application" =>
                new HashSet<string>(["Fluxi.Domain", "Fluxi.SharedKernel"], StringComparer.Ordinal),
            "Fluxi.Infrastructure" =>
                new HashSet<string>(
                    ["Fluxi.Domain", "Fluxi.Application", "Fluxi.SharedKernel"],
                    StringComparer.Ordinal),
            "Fluxi.Api" =>
                new HashSet<string>(["Fluxi.Application", "Fluxi.Infrastructure"], StringComparer.Ordinal),
            _ => throw new ArgumentOutOfRangeException(nameof(layerName), layerName, null)
        };
    }

    #endregion
}
