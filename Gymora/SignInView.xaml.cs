namespace Gymora;

using Gymora.ViewModels;

public partial class SignInView : ContentPage
{
	public SignInView(SignInViewModel viewModel)
	{
		InitializeComponent();	

		BindingContext = viewModel;
	}
}	