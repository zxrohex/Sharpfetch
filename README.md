# Sharpfetch

Sharpfetch is an remake/clone/whatever you wanna call it of Neofetch in C# using dotnet/.NET (Core)
as the framework, which allows Sharpfetch to be cross-platform, with some caveats which are explained
in-depth below.

Right now, Sharpfetch uses and relies on Hardware.Info for fetching system and hardware information across platforms,
but falls back or uses custom code/implementations if needed or required, but this is limited to the abilities of
how and where I can run and debug Sharpfetch by myself (I cannot test Sharpfetch on macOS, and testing on Linux is limited to
WSL which additionally has it's own quirks). The plan though is to rely less on Hardware.Info in the future,
and use/implement my own code.

---

## Current status / Stability

- Extremely pre-alpha: this project is under heavy development and is not recommended for regular or public use.
- Expect instability, crashes, incomplete features, and frequent API/behavior changes.
- Use at your own risk; this is an experimental, developer-focused build intended for testing and early feedback.

## Features and support (current and planned)

### Current feature set
Refer to the implementation for exact, up-to-date behavior:
- See `Sharpfetch/CLI/Commands/BaseCommand.cs` — command entrypoint and platform dispatch.
- See `Sharpfetch/Core/SystemInfo.cs` — the core information collector and platform-specific renderers.

Current runtime behavior (Windows & Linux):
- Platform detection and platform-specific print routines.
- OS / distro logo ASCII art rendering (limited selection of Linux distros, macOS, Windows 11 and 10 for <10).
- Printed information includes:
  - `UserName@MachineName`
  - OS and OS version
  - Architecture
  - Uptime
  - Shell and Terminal environment info
  - CPU description, reported clock speed and core count
  - GPU description and screen resolution (when available)
  - RAM usage (used / total)
  - Disk infos
- Multi-color blocks / 8 color bar x2 (normal and dimmed) rendering
- Uses `Hardware.Info` for hardware queries.
- Uses `Spectre.Console` for terminal UI and `SixLabors.ImageSharp` for logo rendering.
- Windows-specific accent color integration and logo handling when running on Windows.

### Future plans
Planned improvements and additions include:
- macOS support and implementation (currently untested and not implemented).
- More reliable, stable, and broader information for:
  - Terminal / shell detection
  - Window manager / desktop environment
  - GUI vs headless environments
  - Disks and partitions, especially on Linux
  - Linux WM/DE infos and accent color fetching
- Better and more complete logo support for Linux distros
- Better ASCII art generation and rendering/output
- Improved and broader fallbacks for special situations and environments (WSL, minimal containers, VMs).
- More detailed information output (package managers, installed packages, sensors like temperatures/fans, battery health).
- Performance improvements, reduced startup latency, and more robust refresh/update strategies.
- Expanded test coverage and CI for cross-platform validation.

## Build, framework, and installation

- This project targets .NET 10 (preview / RC2). The author prefers to track an upcoming release; expect the project to require the preview SDK.
- No official release binaries are provided due to the project's pre-alpha state.
- To build locally (requires .NET 10 RC2 installed):
  - Restore and build:
    - `dotnet restore`
    - `dotnet build`
  - Run:
    - `dotnet run --project Sharpfetch`
- If you do not run the latest .NET 10 (RC) SDK, builds will probably fail. Use the official .NET SDK installer or `dotnet-install` script to obtain the preview SDK.

## Contributing and helping

- Testers and contributors are welcome — especially those on non-Windows platforms (Linux distros, macOS) because development/testing resources are limited.
- Useful contributions:
  - Cross-platform testing and bug reports (include logs and environment details).
  - Implementations for macOS and additional Linux distro logos.
  - Better detection/fallback code for terminals, shells, and desktop environments.
  - Sensor/package-manager integration and reliability improvements.
  - Performance and startup-time improvements.
- How to help:
  - Open issues with clear reproduction steps and environment details.
  - Fork, implement small focused changes, and open pull requests.
  - Include tests where applicable and keep changes small and documented.
- Project maintainer notes: active maintenance windows may be limited; merge/response times may vary.

## License and resources / Attribution

- License: none specified by the author. Stated project intent: "no license, do whatever the fuck you want with it, it's free real estate."
  - I reject any form of copyright law and assert that this project is in the public domain. R.I.P. Aaron Swartz

- Resources / logos:
  - Windows 11 logo: used from Wikipedia / public images; copyright Microsoft Corporation.
  - All other logos: [ngeenx/operating-system-logos](https://github.com/ngeenx/operating-system-logos)
    - Licensed under the MIT License. See [LICENSE](https://github.com/ngeenx/operating-system-logos/blob/master/LICENSE) or LICENSE.txt in Sharpfetch\Resources for details.

## Notices regarding Generative AI
I am using / utilizing Generative AI in the development process, but this is limited to:
- Auto-Completion
- Using / writing my own code before using Generative AI
- Parts/areas outside of my current knowledge and skillset, which are not security-critical or relevant

As an experiment and test of GitHub Copilot's Agent feature (with GPT-5 as the Agent model), I've let the Agent rebase/refactor/improve the entire codebase,
which worked well enough to use it for further development, but still made the decision to create an new branch with the AI-enhanced/improved codebase, which is for now
the default and main working branch.

Still, my guideline and personal belief is that Generative AI should be mostly and primarily used with one's self-made work.

Currently, the solely AI-generated contents in this project are:
- Sharpfetch\Core\Helpers\ASCIIArtGenerator.cs (ASCII art generation)
- Sharpfetch\Core\Helpers\WindowsInteropHelpers.cs (P/Invoke)
- parts of this README