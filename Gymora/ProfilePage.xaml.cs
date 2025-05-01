namespace Gymora;

public partial class ProfilePage : ContentPage
{
    public ProfilePage()
    {
        InitializeComponent();
    }

    private void OnBackClicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private void OnEditClicked(object sender, EventArgs e)
    {
        // Переход на редактирование профиля (можно реализовать позже)
    }
}