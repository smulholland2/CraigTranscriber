using System.Text;
using CraigTranscriber.Options;
using Whisper.net;

namespace CraigTranscriber.Services;

public class TranscriptionService
{
    public async Task<string> TranscribeAsync(string wavFilePath, ConversionOptions options)
    {
        if (!File.Exists(wavFilePath))
        {
            throw new FileNotFoundException("The WAV file could not be found.", wavFilePath);
        }

        if (!File.Exists(options.ModelPath))
        {
            throw new FileNotFoundException("The Whisper model file could not be found.", options.ModelPath);
        }

        Directory.CreateDirectory(options.OutputPath);

        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(wavFilePath);
        string transcriptPath = Path.Combine(options.OutputPath, $"{fileNameWithoutExtension}.txt");

        var transcriptBuilder = new StringBuilder();

        using var whisperFactory = WhisperFactory.FromPath(options.ModelPath);

        using var processor = whisperFactory.CreateBuilder()
            .WithLanguage("auto")
            .Build();

        using var fileStream = File.OpenRead(wavFilePath);

        await foreach (var result in processor.ProcessAsync(fileStream))
        {
            string line = $"{result.Start} --> {result.End}: {result.Text}";
            transcriptBuilder.AppendLine(line);

            Console.WriteLine(line);
        }

        string transcript = transcriptBuilder.ToString();

        await File.WriteAllTextAsync(transcriptPath, transcript);

        Console.WriteLine();
        Console.WriteLine($"Transcript saved to: {transcriptPath}");

        return transcriptPath;
    }
}
