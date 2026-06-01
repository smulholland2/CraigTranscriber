using System.Diagnostics;

var projectRoot = Directory.GetParent(AppContext.BaseDirectory)!
    .Parent!
    .Parent!
    .Parent!
    .FullName;

var ffmpegPath = Path.Combine(projectRoot, "tools", "ffmpeg.exe");
var inputDir = Path.Combine(projectRoot, "input");
var workingDir = Path.Combine(projectRoot, "working");
var outputDir = Path.Combine(projectRoot, "output");

Directory.CreateDirectory(inputDir);
Directory.CreateDirectory(workingDir);
Directory.CreateDirectory(outputDir);

Console.WriteLine($"Project root: {projectRoot}");
Console.WriteLine($"FFmpeg path:  {ffmpegPath}");
Console.WriteLine($"Input dir:    {inputDir}");
Console.WriteLine($"Working dir:  {workingDir}");
Console.WriteLine($"Output dir:   {outputDir}");
Console.WriteLine();

if (!File.Exists(ffmpegPath))
{
    Console.WriteLine($"FFmpeg not found at: {ffmpegPath}");
    return;
}

var audioFiles = Directory.GetFiles(inputDir, "*.flac");

if (audioFiles.Length == 0)
{
    Console.WriteLine($"No .flac files found in: {inputDir}");
    return;
}

foreach (var inputFile in audioFiles)
{
    var speakerName = Path.GetFileNameWithoutExtension(inputFile);
    var wavOutputPath = Path.Combine(workingDir, $"{speakerName}.wav");

    Console.WriteLine($"Converting {Path.GetFileName(inputFile)} -> {Path.GetFileName(wavOutputPath)}");

    var arguments =
        $"-y -i \"{inputFile}\" -ar 16000 -ac 1 -c:a pcm_s16le \"{wavOutputPath}\"";

    var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = ffmpegPath,
            Arguments = arguments,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        }
    };

    process.Start();

    var stdoutTask = process.StandardOutput.ReadToEndAsync();
    var stderrTask = process.StandardError.ReadToEndAsync();

    await process.WaitForExitAsync();

    var stdout = await stdoutTask;
    var stderr = await stderrTask;

    if (process.ExitCode != 0)
    {
        Console.WriteLine($"FFmpeg failed for {inputFile}");
        Console.WriteLine(stderr);
        continue;
    }

    Console.WriteLine($"Created: {wavOutputPath}");
    Console.WriteLine();
}

Console.WriteLine("Done.");