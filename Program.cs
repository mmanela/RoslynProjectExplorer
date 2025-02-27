using Microsoft.CodeAnalysis.MSBuild; 
using Microsoft.Build.Locator; 
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
    
        if (args.Length < 1)
        {
            Console.WriteLine("Usage: RoslynProjectExplorer <path-to-csproj>");
            return;
        }

        var projectPath = args[0];

        // Register MSBuild
        MSBuildLocator.RegisterDefaults();

        // Create a workspace
        using (var workspace = MSBuildWorkspace.Create())
        { 
            // Add a trace listener to capture logs
            Trace.Listeners.RemoveAt(0);
            Trace.Listeners.Add(new ConsoleTraceListener());

            // Curious if this will reveal the issues on projects not being analyzed
            // workspace.SkipUnrecognizedProjects = false;

            Console.WriteLine("Loading project...");
            var project = workspace.OpenProjectAsync(projectPath).Result;
 
            // Log project information
            if (project != null)
            {
                Console.WriteLine("Project loaded: {0}", project.Name);

                Console.WriteLine("Listing documents...");
                foreach (var document in project.Documents)
                {
                    Console.WriteLine("Document found: {0}", document.FilePath);
                }
            }
            else
            {
                Console.WriteLine("Project not loaded successfully.");
            }
            // Enable advanced logging
            workspace.WorkspaceFailed += (sender, e) =>
            {
                Console.WriteLine($"Workspace failed: {e.Diagnostic.Kind} - {e.Diagnostic.Message}");
            };


            Console.WriteLine("Workspace diagnostics:");
            foreach (var diagnostic in workspace.Diagnostics)
            {
                Console.WriteLine($"{diagnostic.Kind}: {diagnostic.Message}");
            }
        }
    }
}
