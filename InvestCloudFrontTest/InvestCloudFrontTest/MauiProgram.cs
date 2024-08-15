using Microsoft.Extensions.Logging;
using InvestCloudFrontTest.Services;
using InvestCloudFrontTest.ViewModels;
using InvestCloudFrontTest.Views;
namespace InvestCloudFrontTest
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
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "users.db3");
            builder.Services.AddSingleton<DatabaseHelper>(s => new DatabaseHelper(dbPath));
            builder.Services.AddSingleton<IEncryptionService, EncryptionService>();
            builder.Services.AddSingleton<IUserService, UserService>();
            builder.Services.AddTransient<SignInViewModel>();
            builder.Services.AddTransient<SignInPage>();
            builder.Services.AddTransient<CreateAccountViewModel>();
            builder.Services.AddTransient<CreateAccountPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
