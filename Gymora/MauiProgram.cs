using Firebase.Auth;
using Firebase.Auth.Providers;
using Microsoft.Extensions.Logging;
using Gymora.ViewModels;
using Firebase.Auth.Repository;


namespace Gymora
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiMaps()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton(new FirebaseAuthClient(new FirebaseAuthConfig()
            {
                ApiKey = "AIzaSyBg4JAjqqM3TzFVj0SiBmRB - CLeZGQJS6w",
                AuthDomain = "gymora-d5b11.firebaseapp.com",
                Providers = new FirebaseAuthProvider[]
    {
                    new EmailProvider()
    },
                UserRepository = new FileUserRepository("GymoraUser")

            }));

            builder.Services.AddSingleton<SignInView>();
            builder.Services.AddSingleton<SignInViewModel>();
            builder.Services.AddSingleton<SignUpView>();
            builder.Services.AddSingleton<SignUpViewModel>();
            builder.Services.AddTransient<SignInSignUpViewModel>();
            builder.Services.AddTransient<SignInSignUpView>();

            return builder.Build();
        }
    }
}
