namespace Gymora;

using Gymora.ViewModels;

public partial class SignUpView : ContentPage
{
	public SignUpView(SignUpViewModel viewModel)
	{
		InitializeComponent();

        BindingContext = viewModel;
    }
}