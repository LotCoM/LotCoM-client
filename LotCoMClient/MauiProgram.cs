using Microsoft.Extensions.Logging;

namespace LotComClient;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("Aptos.ttf", "AptosRegular");
				fonts.AddFont("Aptos-SemiBold.ttf", "AptosSemiBold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
