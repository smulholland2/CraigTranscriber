using System.Diagnostics;
using CraigTranscriber.Options;

namespace CraigTranscriber.Services;

public sealed class AudioConversionService
{
    public async Task ConvertAsync(ConversionOptions options)
    {
        Console.WriteLine("Audio conversion service started.");
        Console.WriteLine($"Input path:   {options.InputPath}");
        Console.WriteLine($"Output path:  {options.OutputPath}");
        Console.WriteLine($"Working path: {options.WorkingPath}");
        Console.WriteLine($"Model path:   {options.ModelPath}");
        Console.WriteLine($"FFmpeg path:  {options.FFmpegPath}");
        Console.WriteLine();

        if (!Directory.Exists(options.InputPath))
        {
            Console.WriteLine($"Input folder not found: {options.InputPath}");
            return;
        }

        Directory.CreateDirectory(options.WorkingPath);

        if (!File.Exists(options.FFmpegPath))
        {
            Console.WriteLine($"FFmpeg not found at: {options.FFmpegPath}");
            return;
        }

        var audioFiles = Directory
            .GetFiles(options.InputPath, "*.flac")
            .OrderBy(file => file)
            .ToArray();

        if (audioFiles.Length == 0)
        {
            Console.WriteLine($"No .flac files found in: {options.InputPath}");
            return;
        }

        while (true)
        {
            var selectedFile = PromptForAudioFile(audioFiles);

            if (selectedFile is null)
            {
                Console.WriteLine("No file selected. Exiting.");
                return;
            }

            await ConvertFileAsync(selectedFile, options);

            Console.Write("Would you like to convert another file? (y/n): ");
            var newConvert = Console.ReadLine();

            if (!string.Equals(newConvert, "y", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            Console.WriteLine();
        }

        Console.WriteLine();
        Console.WriteLine("Audio conversion complete.");
    }

    private static string? PromptForAudioFile(string[] audioFiles)
    {
        while (true)
        {
            Console.WriteLine("Available .flac files:");
            Console.WriteLine();

            for (var i = 0; i < audioFiles.Length; i++)
            {
                var fileName = Path.GetFileName(audioFiles[i]);
                Console.WriteLine($"{i + 1}. {fileName}");
            }

            Console.WriteLine();
            Console.Write("Which file would you like to convert? ");

            var input = Console.ReadLine();

            if (!int.TryParse(input, out var selectedNumber))
            {
                Console.WriteLine("Please enter a valid number.");
                Console.WriteLine();
                continue;
            }

            if (selectedNumber < 1 || selectedNumber > audioFiles.Length)
            {
                Console.WriteLine($"Please enter a number between 1 and {audioFiles.Length}.");
                Console.WriteLine();
                continue;
            }

            var selectedFile = audioFiles[selectedNumber - 1];
            var selectedFileName = Path.GetFileName(selectedFile);

            Console.WriteLine();
            Console.WriteLine($"You chose number {selectedNumber}: {selectedFileName}");
            Console.Write("Are you sure? (y/n): ");

            var confirmation = Console.ReadLine();

            if (string.Equals(confirmation, "y", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine();
                return selectedFile;
            }

            if (string.Equals(confirmation, "n", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine();
                continue;
            }

            Console.WriteLine("Selection cancelled. Please choose again.");
            Console.WriteLine();
        }
    }

    private static async Task ConvertFileAsync(string inputFile, ConversionOptions options)
    {
        var speakerName = Path.GetFileNameWithoutExtension(inputFile);
        var wavOutputPath = Path.Combine(options.WorkingPath, $"{speakerName}.wav");

        Console.WriteLine($"Converting {Path.GetFileName(inputFile)} -> {Path.GetFileName(wavOutputPath)}");

        var arguments =
            $"-y -i \"{inputFile}\" -ar 16000 -ac 1 -c:a pcm_s16le \"{wavOutputPath}\"";

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = options.FFmpegPath,
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
            return;
        }

        Console.WriteLine($"Created: {wavOutputPath}");
        Console.WriteLine();
    }
}