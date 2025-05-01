using Gymora.ViewModels;

namespace Gymora;

public partial class IntroScreenView : ContentPage
{
	public IntroScreenView()
	{
        InitializeComponent();
        this.BindingContext = new IntroScreenViewModel();
    }
}