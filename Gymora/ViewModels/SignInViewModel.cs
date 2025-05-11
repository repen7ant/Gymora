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
            private async Task NavigateSignUp()
            {
                await Shell.Current.GoToAsync("//SignUp");
            }
        }
    }
