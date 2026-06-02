using CraigTranscriber.Options;
using CraigTranscriber.Services;

namespace CraigTranscriber;

public sealed class CraigTranscriberApp
{
    private readonly AudioConversionService _audioConversionService;

    public CraigTranscriberApp(AudioConversionService audioConversionService)
    {
        _audioConversionService = audioConversionService;
    }

    public async Task RunAsync(string[] args)
    {
        var options = ConversionOptions.FromArgs(args);

        await _audioConversionService.ConvertAsync(options);
    }
}