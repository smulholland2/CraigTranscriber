namespace CraigTranscriber.Options;

public sealed class ConversionOptions
{
    public required string InputPath { get; init; }
    public required string OutputPath { get; init; }
    public required string WorkingPath { get; init; }
    public required string ModelPath { get; init; }
    public required string FFmpegPath { get; init; }

    public static ConversionOptions CreateDefault()
    {
        var basePath = GetDefaultBasePath();

        return new ConversionOptions
        {
            InputPath = Path.Combine(basePath, "input"),
            OutputPath = Path.Combine(basePath, "output"),
            WorkingPath = Path.Combine(basePath, "working"),
            ModelPath = Path.Combine(basePath, "models", "ggml-base.en.bin"),
            FFmpegPath = Path.Combine(basePath, "tools", "ffmpeg.exe")
        };
    }

    public static ConversionOptions FromArgs(string[] args)
    {
        var commandLineValues = ParseCommandLineValues(args);
        var initialBasePath = GetInitialBasePath(commandLineValues);
        var configValues = ParseConfigValues(commandLineValues, initialBasePath);
        var basePath = GetBasePath(commandLineValues, configValues, initialBasePath);

        return new ConversionOptions
        {
            InputPath = GetPathValue(commandLineValues, configValues, basePath, "input", "input-path", "input"),
            OutputPath = GetPathValue(commandLineValues, configValues, basePath, "output", "output-path", "output"),
            WorkingPath = GetPathValue(commandLineValues, configValues, basePath, "working", "working-path", "working"),
            ModelPath = GetPathValue(commandLineValues, configValues, basePath, Path.Combine("models", "ggml-base.en.bin"), "model-path", "model"),
            FFmpegPath = GetPathValue(commandLineValues, configValues, basePath, Path.Combine("tools", "ffmpeg.exe"), "ffmpeg-path", "ffmpeg")
        };
    }

    private static string GetInitialBasePath(Dictionary<string, string> commandLineValues)
    {
        var basePath = GetConfiguredValue(commandLineValues, "base-path", "base");

        if (!string.IsNullOrWhiteSpace(basePath))
        {
            return Path.GetFullPath(basePath);
        }

        return GetDefaultBasePath();
    }

    private static string GetDefaultBasePath()
    {
        var currentDirectory = Directory.GetCurrentDirectory();

        if (LooksLikeAppFolder(currentDirectory))
        {
            return currentDirectory;
        }

        var appDirectory = AppContext.BaseDirectory;

        if (LooksLikeAppFolder(appDirectory))
        {
            return appDirectory;
        }

        var discoveredFolder = FindAppFolder(appDirectory);

        if (discoveredFolder is not null)
        {
            return discoveredFolder;
        }

        return currentDirectory;
    }

    private static Dictionary<string, string> ParseConfigValues(
        Dictionary<string, string> commandLineValues,
        string basePath)
    {
        var configPath = GetConfiguredValue(commandLineValues, "config-path", "config");

        if (string.IsNullOrWhiteSpace(configPath))
        {
            configPath = Path.Combine(basePath, "craigtranscriber.config");
        }
        else
        {
            configPath = GetFullPath(configPath, basePath);
        }

        if (!File.Exists(configPath))
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        return ParseKeyValueLines(File.ReadAllLines(configPath));
    }

    private static string GetBasePath(
        Dictionary<string, string> commandLineValues,
        Dictionary<string, string> configValues,
        string initialBasePath)
    {
        var basePath = GetConfiguredValue(commandLineValues, "base-path", "base")
            ?? GetConfiguredValue(configValues, "base-path", "base");

        if (string.IsNullOrWhiteSpace(basePath))
        {
            return initialBasePath;
        }

        return GetFullPath(basePath, initialBasePath);
    }

    private static string GetPathValue(
        Dictionary<string, string> commandLineValues,
        Dictionary<string, string> configValues,
        string basePath,
        string defaultPath,
        params string[] names)
    {
        var path = GetConfiguredValue(commandLineValues, names)
            ?? GetConfiguredValue(configValues, names)
            ?? defaultPath;

        return GetFullPath(path, basePath);
    }

    private static string? GetConfiguredValue(Dictionary<string, string> values, params string[] names)
    {
        foreach (var name in names)
        {
            if (values.TryGetValue(name, out var value))
            {
                return value;
            }
        }

        return null;
    }

    private static Dictionary<string, string> ParseCommandLineValues(string[] args)
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];

            if (!arg.StartsWith("--", StringComparison.Ordinal))
            {
                continue;
            }

            var key = arg[2..];
            string? value = null;

            var equalsIndex = key.IndexOf('=');

            if (equalsIndex >= 0)
            {
                value = key[(equalsIndex + 1)..];
                key = key[..equalsIndex];
            }
            else if (i + 1 < args.Length && !args[i + 1].StartsWith("--", StringComparison.Ordinal))
            {
                value = args[i + 1];
                i++;
            }

            if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
            {
                values[key] = value;
            }
        }

        return values;
    }

    private static Dictionary<string, string> ParseKeyValueLines(string[] lines)
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith("#", StringComparison.Ordinal))
            {
                continue;
            }

            var equalsIndex = trimmedLine.IndexOf('=');

            if (equalsIndex < 0)
            {
                continue;
            }

            var key = trimmedLine[..equalsIndex].Trim();
            var value = trimmedLine[(equalsIndex + 1)..].Trim();

            if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
            {
                values[key] = value;
            }
        }

        return values;
    }

    private static bool LooksLikeAppFolder(string folderPath)
    {
        return Directory.Exists(Path.Combine(folderPath, "input"))
            || File.Exists(Path.Combine(folderPath, "CraigTranscriber.csproj"));
    }

    private static string? FindAppFolder(string startFolder)
    {
        var directory = new DirectoryInfo(startFolder);

        while (directory is not null)
        {
            if (LooksLikeAppFolder(directory.FullName))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return null;
    }

    private static string GetFullPath(string path, string basePath)
    {
        if (Path.IsPathRooted(path))
        {
            return Path.GetFullPath(path);
        }

        return Path.GetFullPath(Path.Combine(basePath, path));
    }
}
