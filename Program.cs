using CraigTranscriber;
using CraigTranscriber.Services;

var app = new CraigTranscriberApp(
    new FileDiscoveryService(),
    new AudioConversionService(),
    new TranscriptionService()
);

await app.RunAsync(args);
