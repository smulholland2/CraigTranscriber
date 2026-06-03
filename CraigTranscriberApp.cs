using CraigTranscriber.Options;
using CraigTranscriber.Services;

namespace CraigTranscriber;

public sealed class CraigTranscriberApp
{
    private readonly AudioConversionService _audioConversionService;
    private readonly FileDiscoveryService _fileDiscoveryService;
    private readonly TranscriptionService _transcriptionService;

    public CraigTranscriberApp(
        FileDiscoveryService fileDiscoveryService,
        AudioConversionService audioConversionService,
        TranscriptionService transcriptionService)
    {
        _fileDiscoveryService = fileDiscoveryService;
        _audioConversionService = audioConversionService;
        _transcriptionService = transcriptionService;
    }

    public async Task RunAsync(string[] args)
    {
        var options = ConversionOptions.FromArgs(args);

        Console.WriteLine("CraigTranscriber started.");
        Console.WriteLine($"Input path:   {options.InputPath}");
        Console.WriteLine($"Output path:  {options.OutputPath}");
        Console.WriteLine($"Working path: {options.WorkingPath}");
        Console.WriteLine($"Model path:   {options.ModelPath}");
        Console.WriteLine($"FFmpeg path:  {options.FFmpegPath}");
        Console.WriteLine();

        Directory.CreateDirectory(options.WorkingPath);

        await ConvertAudioAsync(options);

        Console.WriteLine();
        Console.WriteLine("Would you like to transcribe this file to text? (y/n): ");
        var makeText = Console.ReadLine();

        if (string.Equals(makeText, "y", StringComparison.OrdinalIgnoreCase))
        {
            await MakeTextAsync(options);
        }
        
        Console.WriteLine();
        Console.WriteLine("Audio conversion complete.");
    }    

    private async Task ConvertAudioAsync(ConversionOptions options)
    {
        if (!File.Exists(options.FFmpegPath))
        {
            Console.WriteLine($"FFmpeg not found at: {options.FFmpegPath}");
            return;
        }

        while (true)
        {
            var selectedFile = _fileDiscoveryService.ChooseFlacFile(options.InputPath);

            if (selectedFile is null)
            {
                return;
            }

            await _audioConversionService.ConvertFileAsync(selectedFile, options);

            Console.Write("Would you like to convert another file? (y/n): ");
            var newConvert = Console.ReadLine();

            if (!string.Equals(newConvert, "y", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            Console.WriteLine();
        }
    }

    private async Task MakeTextAsync(ConversionOptions options)
    {
        if (!File.Exists(options.ModelPath))
        {
            Console.WriteLine($"GGML not found at: {options.ModelPath}");
            return;
        }
        while (true)
        {
            var wavFile = _fileDiscoveryService.ChooseWavFile(options.WorkingPath);

            if (wavFile is null)
            {
                return;
            }

            await _transcriptionService.TranscribeAsync(wavFile, options);

            Console.Write("Would you like to transcribe another file? (y/n): ");
            var newConvert = Console.ReadLine();

            if (!string.Equals(newConvert, "y", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            Console.WriteLine();
        }
    }
}
