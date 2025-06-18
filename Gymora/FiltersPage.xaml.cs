namespace Gymora
{
    public partial class FiltersPage : ContentPage
    {
        public Dictionary<string, (double? min, double? max)> PriceFilters { get; private set; }
        public List<string> SelectedDistricts { get; private set; }
        public bool HighRatingOnly { get; private set; }
        private string _selectedPeriod = "�������";

        public FiltersPage()
        {
            InitializeComponent();
        }

        private void OnPeriodButtonClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;

            SingleBtn.BackgroundColor = Colors.White;
            MonthBtn.BackgroundColor = Colors.White;
            HalfYearBtn.BackgroundColor = Colors.White;
            YearBtn.BackgroundColor = Colors.White;

            button.BackgroundColor = Color.FromArgb("#D5FF40");

            _selectedPeriod = button.Text;
        }

        private void OnApplyFiltersClicked(object sender, EventArgs e)
        {
            PriceFilters = new Dictionary<string, (double? min, double? max)>();

            string periodKey = _selectedPeriod switch
            {
                "�������" => "1 �����",
                "1 �����" => "1 �����",
                "�������" => "6 �������",
                "���" => "12 �������",
                _ => _selectedPeriod
            };

            AddPriceFilter(periodKey, MinPriceEntry.Text, MaxPriceEntry.Text);

            SelectedDistricts = new List<string>();
            if (AcademicDistrict.IsChecked) SelectedDistricts.Add("�������������");
            if (UpperIsetskyDistrict.IsChecked) SelectedDistricts.Add("����-��������");
            if (RailwayDistrict.IsChecked) SelectedDistricts.Add("���������������");
            if (KirovskyDistrict.IsChecked) SelectedDistricts.Add("���������");
            if (LeninskyDistrict.IsChecked) SelectedDistricts.Add("���������");
            if (OktyabrskyDistrict.IsChecked) SelectedDistricts.Add("�����������");
            if (OrdzhonikidzevskyDistrict.IsChecked) SelectedDistricts.Add("�����������������");
            if (ChkalovskyDistrict.IsChecked) SelectedDistricts.Add("����������");

            HighRatingOnly = HighRatingSwitch.IsToggled;

            Navigation.PopModalAsync();
        }

        private void AddPriceFilter(string period, string minText, string maxText)
        {
            double? min = string.IsNullOrEmpty(minText) ? null : double.Parse(minText);
            double? max = string.IsNullOrEmpty(maxText) ? null : double.Parse(maxText);

            if (min.HasValue || max.HasValue)
            {
                PriceFilters[period] = (min, max);
            }
        }
    }
}