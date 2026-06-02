using CraigTranscriber;
using CraigTranscriber.Services;

var app = new CraigTranscriberApp(
    new AudioConversionService()
);

await app.RunAsync(args);