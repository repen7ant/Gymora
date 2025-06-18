using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Newtonsoft.Json;
using System.Diagnostics;
using Gymora.Models;
using System.Collections.Generic;

namespace Gymora
{
    public partial class Gyms : ContentPage
    {
        private List<Gym> _allGyms = new List<Gym>();
        private Dictionary<string, (double? min, double? max)> _currentPriceFilters;
        private List<string> _currentDistrictFilters;
        private bool _currentHighRatingFilter;

        public Gyms()
        {
            InitializeComponent();
            var defaultPosition = new Location(56.838011, 60.597465);
            GymMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                defaultPosition,
                Distance.FromKilometers(50)));
            LoadGymsAsync();
        }

        private async void OnFilterButtonClicked(object sender, EventArgs e)
        {
            var filtersPage = new FiltersPage();
            await Navigation.PushModalAsync(filtersPage);

            filtersPage.Disappearing += async (s, e) =>
            {
                _currentPriceFilters = filtersPage.PriceFilters;
                _currentDistrictFilters = filtersPage.SelectedDistricts;
                _currentHighRatingFilter = filtersPage.HighRatingOnly;
                await ApplyFilters();
            };
        }

        private async Task ApplyFilters()
        {
            if (_allGyms == null || _allGyms.Count == 0) return;

            var filteredGyms = _allGyms.Where(g =>
                g.MatchesFilters(_currentDistrictFilters, _currentPriceFilters, _currentHighRatingFilter)).ToList();

            UpdateMapPins(filteredGyms);
        }

        private void UpdateMapPins(List<Gym> gyms)
        {
            GymMap.Pins.Clear();

            foreach (var gym in gyms)
            {
                var pin = new Pin
                {
                    Label = gym.Name ?? "����������� ���",
                    Address = gym.Location ?? "����� �� ������",
                    Location = new Location(gym.Latitude, gym.Longitude),
                    Type = PinType.Place
                };

                pin.MarkerClicked += async (s, e) =>
                {
                    e.HideInfoWindow = true;
                    await Navigation.PushModalAsync(new GymDetailPage(gym));
                };

                GymMap.Pins.Add(pin);
            }

            if (gyms.Count > 0 && GymMap.Pins.Count > 0)
            {
                var firstPin = GymMap.Pins.First();
                GymMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                    firstPin.Location,
                    Distance.FromKilometers(3)));
            }
        }

        private async void LoadGymsAsync()
        {
            try
            {
                _allGyms = await LoadGymsFromJson();
                UpdateMapPins(_allGyms);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"������: {ex}");
                await DisplayAlert("������", "�� ������� ��������� ������ � �����", "OK");
            }
        }

        private async Task<List<Gym>> LoadGymsFromJson()
        {
            try
            {
                using var resourceStream = await FileSystem.OpenAppPackageFileAsync("gyms.json");
                if (resourceStream == null)
                {
                    Debug.WriteLine("���� gyms.json �� ������ � �������� ����������");
                    return new List<Gym>();
                }

                using var reader = new StreamReader(resourceStream);
                var jsonContent = await reader.ReadToEndAsync();
                var gyms = JsonConvert.DeserializeObject<List<Gym>>(jsonContent);

                Debug.WriteLine($"������� ��������� {gyms?.Count ?? 0} �����");
                return gyms ?? new List<Gym>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"����������� ������ ��������: {ex}");
                return new List<Gym>();
            }
        }
    }
}