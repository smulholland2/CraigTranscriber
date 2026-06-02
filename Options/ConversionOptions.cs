using System;
using System.Collections.Generic;
using System.Text;

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
        return new ConversionOptions
        {
            InputPath = "input",
            OutputPath = "output",
            WorkingPath = "working",
            ModelPath = Path.Combine("models", "ggml-base.en.bin"),
            FFmpegPath = Path.Combine("tools", "ffmpeg.exe")
        };
    }

    public static ConversionOptions FromArgs(string[] args)
    {
        // We are not parsing command-line arguments yet.
        // This gives the app a clean expansion point for later.
        return CreateDefault();
    }
}
