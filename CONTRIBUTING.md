# Contributing to Bachs SDK For .Net

Thank you for your interest in contributing to **Bachs.Net**, the community-maintained
.NET SDK for the Bachs API.

## Disclaimer

This project is independently developed and maintained by Amaechi Ugwu. It is not an
official Bachs product and is not supported by the Bachs team. Issues related to the
SDK should be reported in this repository, not to Bachs support.

## Code of Conduct

This project follows the [Contributor Covenant Code of Conduct](CODE_OF_CONDUCT.md).
By participating, you agree to uphold it.

## Getting started

1. Fork the repository and create a feature branch from `main`.
2. Ensure you have the .NET 8 SDK (and optionally .NET 10) installed.
3. Restore and build:

   ```bash
   dotnet build Bachs.Net.slnx
   ```

4. Run the tests:

   ```bash
   dotnet test test/Bachs.Test/Bachs.Test.csproj
   ```

## Development guidelines

- Follow the conventions in `CODE_DESIGN.md` (file-scoped namespaces, primary
  constructors, sealed records, `ConfigureAwait(false)`, result pattern).
- Public members need XML documentation (`GenerateDocumentationFile=true`).
- Add or update tests in `test/Bachs.Test/` for any behaviour change. HTTP is never
  hit for real; use the `MockHttpMessageHandler` / `CountingHttpMessageHandler`
  patterns.
- Keep changes focused; one concern per pull request.

## Adding an endpoint

See `CODE_DESIGN.md` §5.1 for the step-by-step recipe (models → interface → client →
tests). No changes to `BachsClient` are required.

## Pull requests

- Keep PRs small and well-described.
- Ensure `dotnet build` and `dotnet test` pass for both target frameworks.
- Update `README.md` / `docs/` when public behaviour changes.

## Reporting issues

Open an issue in this repository with reproduction steps, expected vs. actual
behaviour, and the SDK version. Do not contact Bachs support for SDK issues.
