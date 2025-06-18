using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    namespace Gymora.ViewModels
    {
        public partial class SignInViewModel : ObservableObject
        {
            private readonly FirebaseAuthClient _authClient;

            [ObservableProperty]
            private string _email;

            [ObservableProperty]
            private string _userName;

            [ObservableProperty]
            private string _password;
        
            public SignInViewModel(FirebaseAuthClient authClient)
            {
                _authClient = authClient;
            }
        
            [RelayCommand]
            private async Task SignIn()
            {
                try
                {
                    await _authClient.SignInWithEmailAndPasswordAsync(Email, Password);
                    UserName = _authClient.User?.Info?.DisplayName;
                    await Shell.Current.GoToAsync("///MainPage");
                }
                catch (FirebaseAuthException ex)
                {
                    string errorMessage = null;
                    if (ex.Message.Contains("INVALID_LOGIN_CREDENTIALS") || ex.Message.Contains("INVALID_EMAIL"))
                    {
                        errorMessage = "Неверный email или пароль";
                    }
   
                    await Shell.Current.DisplayAlert("Ошибка", errorMessage, "OK");
                }
            }

        [RelayCommand]
        private async Task NewPassword()
        {
            try
            {
                // Проверяем, авторизован ли пользователь
                if (_authClient.User == null)
                {
                    await Shell.Current.DisplayAlert("Ошибка", "Вы не авторизованы", "OK");
                    return;
                }

                // Запрашиваем email пользователя для отправки ссылки сброса
                string email = await Shell.Current.DisplayPromptAsync(
                    "Восстановление пароля",
                    "Введите ваш email для отправки инструкций:",
                    "Отправить",
                    "Отмена",
                    "example@mail.com",
                    -1,
                    Keyboard.Email);

                if (string.IsNullOrWhiteSpace(email))
                    return;

                // Отправляем письмо для сброса пароля
                await _authClient.ResetEmailPasswordAsync(email);

                await Shell.Current.DisplayAlert(
                    "Успешно",
                    "Письмо с инструкциями по смене пароля отправлено на вашу почту",
                    "OK");
            }
            catch (FirebaseAuthException ex)
            {
                string errorMessage = ex.Reason switch
                {
                    AuthErrorReason.InvalidEmailAddress => "Неверный формат email",
                    AuthErrorReason.UserNotFound => "Пользователь с таким email не найден",
                    AuthErrorReason.TooManyAttemptsTryLater => "Слишком много попыток. Попробуйте позже",
                    _ => $"Ошибка: {ex.Message}"
                };

                await Shell.Current.DisplayAlert("Ошибка", errorMessage, "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка", $"Не удалось отправить запрос: {ex.Message}", "OK");
            }
        }
    }
}
