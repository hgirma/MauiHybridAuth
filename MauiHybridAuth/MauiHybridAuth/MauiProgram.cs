using System.Reflection;
using MauiHybridAuth.Services;
using MauiHybridAuth.Shared.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MauiHybridAuth
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // Add device-specific services used by the MauiHybridAuth.Shared project
            builder.Services.AddSingleton<IFormFactor, FormFactor>();

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            //Register needed elements for authentication:
            // This is the core functionality
            builder.Services.AddAuthorizationCore();
            // This is our custom provider
            builder.Services.AddScoped<ICustomAuthenticationStateProvider, MauiAuthenticationStateProvider>();
            // Use our custom provider when the app needs an AuthenticationStateProvider
            builder.Services.AddScoped<AuthenticationStateProvider>(s
                => (MauiAuthenticationStateProvider)s.GetRequiredService<ICustomAuthenticationStateProvider>());

            var environment = string.Empty;
#if DEBUG
            environment = "Development";
#else
            environment = "Production";
#endif
            var stream =
                Assembly
                .GetExecutingAssembly()
                .GetManifestResourceStream($"MauiHybridAuth.appsettings.{environment}.json");

            IConfiguration config = new ConfigurationBuilder().AddJsonStream(stream!).Build();

            builder.Configuration.AddConfiguration(config);

            builder.Services.AddSingleton(_ =>
            {
                HttpClient httpClient;

#if WINDOWS || MACCATALYST
                httpClient = new HttpClient();
#else
                httpClient = new HttpClient(new HttpsClientHandlerService().GetPlatformMessageHandler());
#endif

                if (DeviceInfo.Platform == DevicePlatform.Android)
                {
                    httpClient.BaseAddress = new Uri(config.GetValue<string>("AndroidApiUrl")!);
                }
                else if (DeviceInfo.Platform == DevicePlatform.iOS)
                {
                    httpClient.BaseAddress = new Uri(config.GetValue<string>("IosApiUrl")!);
                }
                else
                {
                    httpClient.BaseAddress = new Uri(config.GetValue<string>("DefaultApiUrl")!);
                }

                return httpClient;
            });

            builder.Services.AddScoped<IRegisterService, RegisterService>();

            return builder.Build();
        }
    }
}
