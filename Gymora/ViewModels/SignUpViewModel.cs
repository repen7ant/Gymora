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
    public partial class SignUpViewModel: ObservableObject
    {
        private readonly FirebaseAuthClient _authClient;

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _username;

        [ObservableProperty]
        private string _password;

        public SignUpViewModel(FirebaseAuthClient authClient)
        {
            _authClient = authClient;
        }

        [RelayCommand]  
        private async Task SignUp()
        {
            try
            {
                await _authClient.CreateUserWithEmailAndPasswordAsync(Email, Password, Username);
                await Shell.Current.GoToAsync("//SignIn");
            }

            catch (FirebaseAuthException ex)
            {
                string errorMessage = null;
                if (ex.Message.Contains("INVALID_EMAIL"))
                {
                    errorMessage = "Неверный email";
                }
                else if (ex.Message.Contains("WEAK_PASSWORD"))
                {
                    errorMessage = "Пароль должен содержать минимум 6 символов";
                }

                await Shell.Current.DisplayAlert("Ошибка", errorMessage, "OK");
            }
        }
        [RelayCommand]
        private async Task NavigateSignIn()
        {
            await Shell.Current.GoToAsync("//SignIn");
        }
    }
}
