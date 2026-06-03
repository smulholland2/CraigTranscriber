namespace CraigTranscriber.Services;

public sealed class FileDiscoveryService
{
    private enum FileType
    {
        Flac,
        Wav
    }
    private enum Action
    {
        Convert,
        Transcribe
    }

    public string? ChooseFlacFile(string inputPath)
    {
        if (!Directory.Exists(inputPath))
        {
            Console.WriteLine($"Input folder not found: {inputPath}");
            return null;
        }

        var audioFiles = Directory
            .GetFiles(inputPath, "*.flac")
            .OrderBy(file => file)
            .ToArray();

        if (audioFiles.Length == 0)
        {
            Console.WriteLine($"No .flac files found in: {inputPath}");
            return null;
        }

        return PromptForAudioFile(audioFiles, FileType.Flac, Action.Convert);
    }

    private static string? PromptForAudioFile(string[] audioFiles, FileType type, Action action)
    {
        while (true)
        {
            Console.WriteLine($"Available .{type.ToString().ToLowerInvariant()} files:");
            Console.WriteLine();

            for (var i = 0; i < audioFiles.Length; i++)
            {
                var fileName = Path.GetFileName(audioFiles[i]);
                Console.WriteLine($"{i + 1}. {fileName}");
            }

            Console.WriteLine();
            Console.Write($"Which file would you like to {action.ToString().ToLowerInvariant()}? ");

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

    public string? ChooseWavFile(string workingPath)
    {
        if (!Directory.Exists(workingPath))
        {
            Console.WriteLine($"Working folder not found: {workingPath}");
            return null;
        }

        var audioFiles = Directory
            .GetFiles(workingPath, "*.wav")
            .OrderBy(file => file)
            .ToArray();

        if (audioFiles.Length == 0)
        {
            Console.WriteLine($"No .wav files found in: {workingPath}");
            return null;
        }

        return PromptForAudioFile(audioFiles, FileType.Wav, Action.Transcribe);
    }
}
