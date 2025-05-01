namespace Gymora
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            calendar.SelectedDate = DateTime.Now;
        }

        private async void OnProfileIconClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProfilePage());
        }

        private void calendar_OnDateSelected(object sender, DateTime e)
        {

        }
    }

}
