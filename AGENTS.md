# CraigTranscriber Agent Instructions

This is a .NET 10 console application for processing Discord Craig audio exports.

## Current goal

The app currently converts `.flac` files to `.wav` using FFmpeg. The next major feature is Whisper transcription.

## Architecture preferences

- Keep `Program.cs` minimal.
- Use dependency injection.
- Keep orchestration in `CraigTranscriberApp`.
- Keep audio conversion details in `AudioConversionService`.
- Keep command-line/configuration parsing in `ConversionOptions`.
- Avoid moving user-prompting logic into low-level conversion code unless explicitly requested.

## Current intended flow

1. Load `ConversionOptions`.
2. Display available `.flac` files from the input folder.
3. Prompt the user to choose a file by number.
4. Convert the selected `.flac` file to `.wav`.
5. Ask whether the user wants to convert another file.
6. Repeat until the user declines.

## Style

- Make small, reviewable changes.
- Explain why each change is needed before editing.
- Preserve existing names, especially `FFmpegPath`.
- Do not rewrite the project structure unless asked.
- Prefer simple beginner-readable C# over clever abstractions.