# Changelog

Notable changes to Bachs.Sdk are recorded here.

## [2.0.0] - 2026-11-13

### Breaking changes

- Dropped .NET 8 and .NET 9 targets. This release targets .NET 10 and .NET 11; consumers on .NET 8 or .NET 9 must remain on version 1.0.0 or migrate to a supported target framework.

### Added

- Added request-scoped API key environment resolution, routing live and sandbox scoped keys to their corresponding API base URLs.

### Changed

- Target .NET 10 and .NET 11.
- Improved response-processing performance with asynchronous stream-based JSON deserialization, including cancellation support.
- Added automatic idempotency keys for write requests when the caller does not provide one. Explicit keys continue to be honored.
- Validate configured client options at application startup and require a non-empty webhook secret when registering with explicit credentials.
- Use `Microsoft.Extensions.Http.Resilience` 11.0.0.
- Updated test dependencies and kept test-only package assets private to the test project.

### Fixed

- Improved handling of invalid scoped API keys and ensured request content is disposed if request URI construction fails.
