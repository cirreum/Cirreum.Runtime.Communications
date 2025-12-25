namespace Microsoft.AspNetCore.Hosting;

using Cirreum.Communications.Email;
using Cirreum.Communications.Email.Configuration;
using Cirreum.Communications.Email.Health;
using Cirreum.Communications.Sms;
using Cirreum.Communications.Sms.Configuration;
using Cirreum.Communications.Sms.Health;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public static class HostingExtensions {

	private class ConfigureEmailStorageMarker { }
	private class ConfigureSmsStorageMarker { }

	/// <summary>
	/// Add support for Email by registering any configured providers
	/// and associated instances.
	/// </summary>
	public static IHostApplicationBuilder AddEmailServices(this IHostApplicationBuilder builder) {

		// Check if already registered using a marker service		
		if (builder.Services.IsMarkerTypeRegistered<ConfigureEmailStorageMarker>()) {
			return builder;
		}

		// Mark as registered
		builder.Services.MarkTypeAsRegistered<ConfigureEmailStorageMarker>();

		// Service Providers...
		return builder
			.RegisterServiceProvider<
				SendGridEmailRegistrar,
				SendGridEmailSettings,
				SendGridEmailInstanceSettings,
				SendGridEmailHealthCheckOptions>()
			.RegisterServiceProvider<
				AzureEmailRegistrar,
				AzureEmailSettings,
				AzureEmailInstanceSettings,
				AzureEmailHealthCheckOptions>();

		// .RegisterServiceProvider<AbcEmailRegistrar>();

	}

	/// <summary>
	/// Add support for Sms by registering any configured providers
	/// and associated instances.
	/// </summary>
	public static IHostApplicationBuilder AddSmsServices(this IHostApplicationBuilder builder) {

		// Check if already registered using a marker service		
		if (builder.Services.IsMarkerTypeRegistered<ConfigureSmsStorageMarker>()) {
			return builder;
		}

		// Mark as registered
		builder.Services.MarkTypeAsRegistered<ConfigureSmsStorageMarker>();

		// Service Providers...
		return builder
			.RegisterServiceProvider<
				TwilioSmsRegistrar,
				TwilioSmsSettings,
				TwilioSmsInstanceSettings,
				TwilioSmsHealthCheckOptions>();

		// .RegisterServiceProvider<AbcSmsRegistrar>();

	}

}