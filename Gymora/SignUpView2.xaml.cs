using Gymora.ViewModels;

namespace Gymora;

public partial class SignUpView2 : ContentPage
{
    private readonly SignUpViewModel2 _viewModel;

    public SignUpView2() : this(new SignUpViewModel2())
    {
    }

    public SignUpView2(SignUpViewModel2 viewModel) : base()
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;

        // Подписываемся на событие загрузки фото
        _viewModel.PhotoLoaded += ViewModel_PhotoLoaded;
    }

    private void ViewModel_PhotoLoaded(object sender, EventArgs e)
    {
        string base64 = Preferences.Get("user_photo", null);
        if (!string.IsNullOrEmpty(base64))
        {
            byte[] imageBytes = Convert.FromBase64String(base64);
            ProfilePhoto.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
        }
    }
}