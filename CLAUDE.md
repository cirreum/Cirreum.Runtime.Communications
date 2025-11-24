# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Repository Overview

This is the Cirreum.Runtime.Communications library - a runtime extension that provides Email and SMS communication services integration for .NET applications. It's part of the Cirreum Foundation Framework and currently supports SendGrid for email and Twilio for SMS.

## Build Commands

```bash
# Build the solution
dotnet build

# Build in Release mode
dotnet build -c Release

# Create NuGet package
dotnet pack -c Release

# Clean build artifacts
dotnet clean
```

## Architecture

### Core Components

1. **Service Registration** - The library provides extension methods for IHostApplicationBuilder:
   - `AddEmailServices()` - Registers email providers (SendGrid)
   - `AddSmsServices()` - Registers SMS providers (Twilio)

2. **Configuration Structure** - All configuration lives under `Cirreum:Communications:Providers:{Provider}:Instances:{InstanceName}`
   - Supports multiple named instances per provider
   - Health check configuration per instance
   - Bulk operation settings

3. **Key Integration Points**:
   - Depends on `Cirreum.Runtime.ServiceProvider` for service registration patterns
   - Integrates with Azure Key Vault for secure credential storage
   - Uses marker services to prevent duplicate registrations

### Project Structure

- `src/Cirreum.Runtime.Communications/` - Main library project
  - `Extensions/Hosting/` - IHostApplicationBuilder extension methods
  - `CONFIGURATION_GUIDE.md` - Comprehensive configuration documentation
- `build/` - MSBuild property files for package metadata and versioning

## Development Guidelines

- Target framework: .NET 10.0
- Follow existing patterns from Microsoft.Extensions.* libraries
- Configuration uses the standard IConfiguration pattern
- Services are registered as keyed services for multi-instance support
- Health checks are integrated with the standard .NET health check infrastructure

## Configuration

See `src/Cirreum.Runtime.Communications/CONFIGURATION_GUIDE.md` for detailed configuration options. Key points:
- Provider instances are configured under `Cirreum:Communications:Providers`
- Credentials can come from Key Vault (production) or connection strings (development)
- Each instance supports health checks, bulk operations, and provider-specific settings