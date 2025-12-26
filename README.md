# Cirreum.Runtime.Communications

[![NuGet Version](https://img.shields.io/nuget/v/Cirreum.Runtime.Communications.svg?style=flat-square&labelColor=1F1F1F&color=003D8F)](https://www.nuget.org/packages/Cirreum.Runtime.Communications/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Cirreum.Runtime.Communications.svg?style=flat-square&labelColor=1F1F1F&color=003D8F)](https://www.nuget.org/packages/Cirreum.Runtime.Communications/)
[![GitHub Release](https://img.shields.io/github/v/release/cirreum/Cirreum.Runtime.Communications?style=flat-square&labelColor=1F1F1F&color=FF3B2E)](https://github.com/cirreum/Cirreum.Runtime.Communications/releases)
[![License](https://img.shields.io/github/license/cirreum/Cirreum.Runtime.Communications?style=flat-square&labelColor=1F1F1F&color=F2F2F2)](https://github.com/cirreum/Cirreum.Runtime.Communications/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-003D8F?style=flat-square&labelColor=1F1F1F)](https://dotnet.microsoft.com/)

**Unified communication services for modern .NET applications**

## Overview

**Cirreum.Runtime.Communications** provides a standardized approach to integrating email and SMS services into .NET applications. It offers provider-agnostic abstractions with built-in support for multiple providers: SendGrid and Azure Email Service for email, and Twilio and Azure SMS Service for SMS, including health checks, bulk operations, and Azure Key Vault integration.

## Features

- **Multi-provider support** - SendGrid and Azure Email Service for email, Twilio and Azure SMS Service for SMS
- **Named instances** - Configure multiple instances of the same provider
- **Health checks** - Built-in health check support with caching
- **Bulk operations** - Efficient batch processing for mass communications
- **Azure Key Vault integration** - Secure credential management
- **Sandbox mode** - Safe testing in development environments
- **Retry policies** - Configurable retry logic for resilience

## Choosing a Provider

### Email Providers
- **SendGrid** - Best for high-volume transactional emails, marketing campaigns, and detailed analytics
- **Email.Azure** - Ideal when already using Azure services, provides native Azure integration and compliance

### SMS Providers
- **Twilio** - Industry leader with global reach, programmable messaging, and extensive features
- **Sms.Azure** - Perfect for Azure-centric applications, integrated billing, and compliance requirements

## Prerequisites

The package includes the following provider SDKs:
- SendGrid SDK (for SendGrid email)
- Azure Communication Services SDK (for Email.Azure and Sms.Azure)
- Twilio SDK (for Twilio SMS)

## Getting Started

### Installation

```bash
dotnet add package Cirreum.Runtime.Communications
```

### Basic Usage

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add email and SMS services
builder.AddEmailServices()
       .AddSmsServices();

// Add health checks
builder.Services.AddHealthChecks()
    .AddCheck<EmailHealthCheck>("email-health")
    .AddCheck<SmsHealthCheck>("sms-health");

var app = builder.Build();

// Map health check endpoints
app.MapHealthChecks("/health");

// Inject and use default services
app.MapPost("/notify", async (IEmailService email, ISmsService sms) =>
{
    await email.SendAsync(new EmailMessage
    {
        To = "user@example.com",
        Subject = "Welcome!",
        Body = "Thanks for signing up."
    });
    
    await sms.SendAsync(new SmsMessage
    {
        To = "+1234567890",
        Body = "Your code is: 123456"
    });
});

// Use named instances (when multiple instances are configured)
app.MapPost("/marketing", async (IServiceProvider sp) =>
{
    var marketingEmail = sp.GetRequiredKeyedService<IEmailService>("marketing");
    var alertsSms = sp.GetRequiredKeyedService<ISmsService>("alerts");
    
    // Use specific instances for different purposes
});
```

### Configuration

Add to your `appsettings.json`:

```json
{
  "Cirreum": {
    "Communications": {
      "Providers": {
        "SendGrid": {
          "Instances": {
            "default": {
              "Name": "sendgrid-api-key",
              "MaxRetries": 3,
              "SandboxMode": false
            }
          }
        },
        "Email.Azure": {
          "Instances": {
            "default": {
              "Name": "azure-email-connection",
              "MaxRetries": 3
            }
          }
        },
        "Sms.Azure": {
          "Instances": {
            "default": {
              "Name": "azure-sms-connection",
              "From": "+1234567890",
              "MaxRetries": 3
            }
          }
        },
        "Twilio": {
          "Instances": {
            "default": {
              "Name": "twilio-connection",
              "From": "+1234567890",
              "MaxRetries": 3
            }
          }
        }
      }
    }
  }
}
```

### Connection Strings

For development, add connection strings to your configuration:

```json
{
  "ConnectionStrings": {
    "sendgrid-api-key": "SG.actual-api-key-here",
    "azure-email-connection": "endpoint=https://your-resource.communication.azure.com/;accesskey=your-key",
    "azure-sms-connection": "endpoint=https://your-resource.communication.azure.com/;accesskey=your-key",
    "twilio-connection": "AccountSid=AC...;AuthToken=your-auth-token"
  }
}
```

### Azure Key Vault Integration

In production, the `Name` field in configuration maps to Key Vault secret names using the pattern `ConnectionStrings--{Name}`:

```json
{
  "Cirreum": {
    "Communications": {
      "Providers": {
        "SendGrid": {
          "Instances": {
            "default": {
              "Name": "sendgrid-api-key"  // Maps to Key Vault secret: ConnectionStrings--sendgrid-api-key
            }
          }
        }
      }
    }
  }
}
```

Key Vault secret values (stored as JSON strings):

```json
// Key Vault secret name: ConnectionStrings--sendgrid-api-key
{
  "ConnectionString": "SG.actual-api-key-here"
}

// Key Vault secret name: ConnectionStrings--azure-email-connection
{
  "ConnectionString": "endpoint=https://your-resource.communication.azure.com/;accesskey=your-key"
}

// Key Vault secret name: ConnectionStrings--twilio-connection
{
  "ConnectionString": "AccountSid=AC...;AuthToken=your-auth-token",
  "From": "+1234567890",
  "ServiceId": "IS..."  // Optional
}

// Key Vault secret name: ConnectionStrings--azure-sms-connection
{
  "ConnectionString": "endpoint=https://your-resource.communication.azure.com/;accesskey=your-key",
  "From": "+1234567890"
}
```

## Advanced Usage

### Bulk Operations

Send multiple messages efficiently:

```csharp
var messages = GetEmailMessages(); // Your list of messages

await emailService.SendBulkAsync(messages, new BulkOptions 
{
    MaxBatchSize = 100,      // Process in batches of 100
    MaxConcurrency = 4       // Up to 4 parallel operations
});
```

### Health Checks

Health checks are automatically registered and can be configured:

```json
{
  "HealthOptions": {
    "IncludeInReadinessCheck": true,
    "TestEmailAddress": "health@example.com",
    "TestApiConnectivity": true,
    "CachedResultTimeout": "00:01:00"  // Cache results for 1 minute
  }
}
```

### Multiple Named Instances

Configure multiple instances for different use cases:

```json
{
  "SendGrid": {
    "Instances": {
      "transactional": {
        "Name": "sendgrid-transactional-key",
        "MaxRetries": 5
      },
      "marketing": {
        "Name": "sendgrid-marketing-key",
        "SandboxMode": false,
        "GlobalCategories": ["marketing", "campaigns"]
      }
    }
  }
}
```

## Documentation

- [Configuration Guide](src/Cirreum.Runtime.Communications/CONFIGURATION_GUIDE.md) - Comprehensive configuration options and examples

## Contribution Guidelines

1. **Be conservative with new abstractions**  
   The API surface must remain stable and meaningful.

2. **Limit dependency expansion**  
   Only add foundational, version-stable dependencies.

3. **Favor additive, non-breaking changes**  
   Breaking changes ripple through the entire ecosystem.

4. **Include thorough unit tests**  
   All primitives and patterns should be independently testable.

5. **Document architectural decisions**  
   Context and reasoning should be clear for future maintainers.

6. **Follow .NET conventions**  
   Use established patterns from Microsoft.Extensions.* libraries.

## Versioning

Cirreum.Runtime.Communications follows [Semantic Versioning](https://semver.org/):

- **Major** - Breaking API changes
- **Minor** - New features, backward compatible
- **Patch** - Bug fixes, backward compatible

Given its foundational role, major version bumps are rare and carefully considered.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

**Cirreum Foundation Framework**  
*Layered simplicity for modern .NET*