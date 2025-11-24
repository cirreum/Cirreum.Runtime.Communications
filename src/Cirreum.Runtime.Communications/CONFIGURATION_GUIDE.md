# Communications Configuration Guide

## Complete Configuration Example

```json
{
  "Cirreum": {
    "Communications": {
      "Providers": {
        "SendGrid": {
          "Tracing": true,
          "Instances": {
            "default": {
              "Name": "sendgrid-email-dev-001",
              "MaxRetries": 3,
              "SandboxMode": false,
              "GlobalCategories": ["Cirreum", "Notifications"],
              "GlobalHeaders": {
                "X-Cirreum-Env": "Development",
                "X-Application": "MyApp"
              },
              "BulkOptions": {
                "MaxBatchSize": 500,
                "MaxConcurrency": 4
              },
              "HealthChecks": true,
              "HealthOptions": {
                "IncludeInReadinessCheck": true,
                "TestEmailAddress": "test@example.com",
                "TestApiConnectivity": true,
                "CachedResultTimeout": "00:01:00"
              }
            }
          }
        },
        "Twilio": {
          "Tracing": true,
          "Instances": {
            "default": {
              "Name": "twilio-01",
              "ServiceId": "",
              "From": "+1234567890",
              "MaxRetries": 3,
              "BulkOptions": {
                "MaxConcurrency": 10
              },
              "HealthChecks": true,
              "HealthOptions": {
                "IncludeInReadinessCheck": true,
                "PhoneNumber": "+14807932935",
                "TestSending": true,
                "CachedResultTimeout": "06:00:00"
              }
            }
          }
        }
      }
    }
  }
}
```

## SendGrid Configuration

### Basic Settings

| Property | Type | Required | Description |
|----------|------|----------|-------------|
| `Name` | string | ✅ | Key Vault secret name or connection string key |
| `MaxRetries` | int | ✅ | Maximum retry attempts (default: 3) |
| `SandboxMode` | bool | ✅ | Enable sandbox mode for testing |

### Global Settings

```json
{
  "GlobalCategories": ["Cirreum", "Notifications"],
  "GlobalHeaders": {
    "X-Cirreum-Env": "Production",
    "X-Application": "MyApp",
    "X-Version": "1.0.0"
  }
}
```

- **GlobalCategories**: Applied to all emails for tracking and analytics
- **GlobalHeaders**: Custom headers added to all outgoing emails

### Bulk Operations

```json
{
  "BulkOptions": {
    "MaxBatchSize": 500,
    "MaxConcurrency": 4
  }
}
```

- **MaxBatchSize**: Maximum emails per batch (SendGrid limit: 1000)
- **MaxConcurrency**: Parallel batch processing (recommended: 2-6)

### Health Check Configuration

```json
{
  "HealthChecks": true,
  "HealthOptions": {
    "IncludeInReadinessCheck": true,
    "TestEmailAddress": "test@example.com",
    "TestApiConnectivity": true,
    "CachedResultTimeout": "00:01:00"
  }
}
```

| Property | Description |
|----------|-------------|
| `TestEmailAddress` | Email used for validation tests |
| `TestApiConnectivity` | Test API connection during health checks |
| `CachedResultTimeout` | Cache duration for health check results |

## Twilio Configuration

### Basic Settings

| Property | Type | Required | Description |
|----------|------|----------|-------------|
| `Name` | string | ✅ | Key Vault secret name or connection string key |
| `ServiceId` | string | ❌ | Twilio Service SID (leave empty to use Key Vault) |
| `From` | string | ✅ | Default sender phone number |
| `MaxRetries` | int | ✅ | Maximum retry attempts (default: 3) |

### Special `From` Values

- **Phone number**: `"+1234567890"` - Use specific number
- **`"none"`**: Ignore Key Vault value, use only what's specified in code
- **Empty/null**: Use value from Key Vault

### Bulk Operations

```json
{
  "BulkOptions": {
    "MaxConcurrency": 10
  }
}
```

- **MaxConcurrency**: Maximum parallel SMS operations (adjust based on Twilio limits)

### Health Check Configuration

```json
{
  "HealthChecks": true,
  "HealthOptions": {
    "IncludeInReadinessCheck": true,
    "PhoneNumber": "+14807932935",
    "TestSending": true,
    "CachedResultTimeout": "06:00:00"
  }
}
```

| Property | Description |
|----------|-------------|
| `PhoneNumber` | Phone number used for test messages |
| `TestSending` | Actually send test SMS during health checks |
| `CachedResultTimeout` | Cache duration (default: 6 hours) |

## Multiple Instances

You can configure multiple instances of the same provider:

```json
{
  "SendGrid": {
    "Instances": {
      "marketing": {
        "Name": "sendgrid-marketing",
        "GlobalCategories": ["Marketing", "Campaigns"]
      },
      "transactional": {
        "Name": "sendgrid-transactional", 
        "GlobalCategories": ["Transactional", "System"]
      }
    }
  }
}
```

Access via dependency injection:
```csharp
// Get specific instance
var marketingEmail = serviceProvider.GetRequiredKeyedService<IEmailService>("marketing");

// Get default instance  
var defaultEmail = serviceProvider.GetRequiredService<IEmailService>();
```

## Environment-Specific Configurations

### Development
```json
{
  "SendGrid": {
    "Instances": {
      "default": {
        "SandboxMode": true,
        "TestApiConnectivity": true,
        "CachedResultTimeout": "00:01:00"
      }
    }
  },
  "Twilio": {
    "Instances": {
      "default": {
        "TestSending": false,
        "CachedResultTimeout": "00:05:00"
      }
    }
  }
}
```

### Production
```json
{
  "SendGrid": {
    "Instances": {
      "default": {
        "SandboxMode": false,
        "TestApiConnectivity": false,
        "CachedResultTimeout": "00:15:00",
        "BulkOptions": {
          "MaxBatchSize": 1000,
          "MaxConcurrency": 6
        }
      }
    }
  },
  "Twilio": {
    "Instances": {
      "default": {
        "TestSending": false,
        "CachedResultTimeout": "06:00:00",
        "BulkOptions": {
          "MaxConcurrency": 20
        }
      }
    }
  }
}
```

## Security Best Practices

### Azure Key Vault Integration

Store sensitive values in Key Vault:

```json
{
  "SendGrid": {
    "Instances": {
      "default": {
        "Name": "sendgrid-api-key-prod"
      }
    }
  }
}
```

Key Vault secret format:
- **SendGrid**: Store the API key directly
- **Twilio**: Store as connection string format: `AccountSid=AC123;AuthToken=abc123;From=+1234567890`

### Connection Strings (Development)

For local development, use connection strings:

```json
{
  "ConnectionStrings": {
    "sendgrid-api-key-dev": "SG.abc123...",
    "twilio-dev": "AccountSid=AC123;AuthToken=abc123;From=+1234567890"
  }
}
```

## Troubleshooting

### Common Issues

1. **Health checks failing**
   - Verify Key Vault permissions
   - Check API key validity
   - Ensure test phone/email addresses are valid

2. **Bulk operations timing out**
   - Reduce `MaxConcurrency`
   - Decrease `MaxBatchSize`
   - Check provider rate limits

3. **Messages not sending**
   - Verify `SandboxMode` setting
   - Check provider account status
   - Review retry configuration

### Logging

Enable tracing for detailed logs:
```json
{
  "SendGrid": {
    "Tracing": true
  },
  "Twilio": {
    "Tracing": true
  }
}
```

Logs include:
- API call details
- Retry attempts
- Health check results
- Bulk operation metrics