using System.Diagnostics;
using CraigTranscriber.Options;

namespace CraigTranscriber.Services;

public sealed class AudioConversionService
{
    public async Task<string?> ConvertFileAsync(string inputFile, ConversionOptions options)
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

        try
        {
            process.Start();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FFmpeg could not be started: {options.FFmpegPath}");
            Console.WriteLine(ex.Message);
            return null;
        }

        var stdoutTask = process.StandardOutput.ReadToEndAsync();
        var stderrTask = process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        var stdout = await stdoutTask;
        var stderr = await stderrTask;

        if (process.ExitCode != 0)
        {
            Console.WriteLine($"FFmpeg failed for {inputFile}");
            Console.WriteLine(stderr);
            return null;
        }

        Console.WriteLine($"Created: {wavOutputPath}");
        Console.WriteLine();

        return wavOutputPath;
    }
}
