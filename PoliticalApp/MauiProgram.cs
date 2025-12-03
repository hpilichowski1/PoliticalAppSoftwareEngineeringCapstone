using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using PoliticalApp.Services;
using PoliticalApp.ViewModels;
using PoliticalApp.Views;
using Microsoft.Extensions.DependencyInjection;

namespace PoliticalApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Register Services
		builder.Services.AddSingleton<RepresentativesViewModel>();
		builder.Services.AddSingleton<IPolicyService, MockPolicyService>();

		builder.Services.AddSingleton<IBillService, ApiBillService>();
		builder.Services.AddTransient<BillsViewModel>();
		builder.Services.AddTransient<BillsPage>();

		// Register ViewModels
		builder.Services.AddTransient<HomeViewModel>();
		builder.Services.AddTransient<RepresentativesViewModel>();

		builder.Services.AddHttpClient<ApiClient>();
		builder.Services.AddTransient<LoginViewModel>();
		builder.Services.AddTransient<LoginPage>();

		builder.Services.AddTransient<RegisterViewModel>();
		builder.Services.AddTransient<RegisterPage>();	

		// Register Pages
		builder.Services.AddTransient<MainPage>();
		builder.Services.AddTransient<HomePage>();
		builder.Services.AddTransient<RepresentativesPage>();

		builder.Logging.AddDebug();

		builder.Services.AddHttpClient<ApiClient>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5154/"); // matches your API
        });

		builder.Services.AddHttpClient<IRepresentativeService, ApiRepresentativeService>(client =>
        {
            // NOTE: set to your real API URL
            client.BaseAddress = new Uri("http://localhost:5154/");
        });

		builder.Services.AddHttpClient<IBillService, ApiBillService>(client =>
		{
			client.BaseAddress = new Uri("http://localhost:5154/"); // adjust if different
		});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
