using PagePlay.Site.Infrastructure.UI.Rendering;

/// <summary>
/// Simple CLI entry point for theme compilation.
/// Usage: dotnet run compile-theme [theme-file] [output-file]
/// </summary>
public class ThemeCompilerCli
{
    public static void CompileThemeFromArgs(string[] args)
    {
        if (args.Length < 3 || args[0] != "compile-theme")
        {
            Console.WriteLine("Theme compiler not invoked (normal app startup)");
            return;
        }

        var themeFile = args[1];
        var outputFile = args[2];

        Console.WriteLine($"Compiling theme: {themeFile} -> {outputFile}");

        try
        {
            var css = ThemeCompiler.CompileTheme(themeFile);
            File.WriteAllText(outputFile, css);
            Console.WriteLine($"✓ Theme compiled successfully!");
            Environment.Exit(0);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"✗ Theme compilation failed: {ex.Message}");
            Environment.Exit(1);
        }
    }
}
