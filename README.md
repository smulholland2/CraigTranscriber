# CraigTranscriber

A Windows console app for locally transcribing Craig Discord bot FLAC recordings. Built for Game Masters who want detailed tabletop RPG session logs without relying on paid transcription websites.

## Current MVP

- Converts Craig `.flac` files to `.wav`
- Uses FFmpeg for audio conversion
- Runs locally on Windows
- Prepares audio for local transcription with whisper.cpp

## Planned Features

- Run whisper.cpp directly from the console app
- Generate `.txt` and `.srt` transcript files
- Remove `[BLANK_AUDIO]` entries
- Add timestamped transcript output
- Merge separate Craig speaker tracks into one chronological transcript

## Requirements

- Windows
- Visual Studio 2022
- .NET 8 SDK
- FFmpeg
- whisper.cpp
- A compatible Whisper model file, such as `ggml-base.en.bin`

## Ready-to-run download

This repository does not include FFmpeg, whisper.cpp, or Whisper model files. Users are responsible for downloading those separately and complying with their respective licenses.

For convenience, packaged Windows builds may be provided under GitHub Releases. Release packages may include the required runtime binaries and a small Whisper model.

## Folder Setup

Place Craig `.flac` files in:
