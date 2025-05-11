namespace Gymora;

using Gymora.ViewModels;

public partial class SignInSignUpView : ContentPage
{
    public SignInSignUpView(SignInSignUpViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }

}