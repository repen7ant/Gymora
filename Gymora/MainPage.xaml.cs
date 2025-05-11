namespace Gymora
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            calendar.SelectedDate = DateTime.Now;
        }

        private void calendar_OnDateSelected(object sender, DateTime e)
        {

        }
    }

}
