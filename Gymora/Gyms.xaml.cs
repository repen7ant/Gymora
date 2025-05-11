using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Newtonsoft.Json;
using System.Diagnostics;
using Gymora.Models;

namespace Gymora
{
    public partial class Gyms : ContentPage
    {
        public Gyms()
        {
            InitializeComponent();
            var defaultPosition = new Location(56.838011, 60.597465); // ���������� �������������
            GymMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                defaultPosition,
                Distance.FromKilometers(50)));
            LoadGymsAsync();
        }

        private async void LoadGymsAsync()
        {
            try
            {
                var gyms = await LoadGymsFromJson();

                if (gyms == null || gyms.Count == 0)
                {
                    Debug.WriteLine("��� ������ � �����");
                    return;
                }

                // ��������� �������
                foreach (var gym in gyms)
                {
                    if (double.IsNaN(gym.Latitude) || double.IsNaN(gym.Longitude))
                    {
                        Debug.WriteLine($"�������� ��� {gym.Name}: �������� ����������");
                        continue;
                    }

                    // ���������, ��� Label �� ������ (����� ����� ������)
                    if (string.IsNullOrWhiteSpace(gym.Name))
                    {
                        Debug.WriteLine($"�������� ���: �� ������� ��������");
                        continue;
                    }

                    var pin = new Pin
                    {
                        Label = gym.Name ?? "����������� ���", // ���� Name == null, ������ ��������� ��������
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
                    Debug.WriteLine($"�������� ������: {pin.Label} ({gym.Latitude}, {gym.Longitude})");
                }

                // ���������� ����� � ������� ����
                if (gyms.Count > 0 && GymMap.Pins.Count > 0)
                {
                    var firstPin = GymMap.Pins.First();
                    GymMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                        firstPin.Location,
                        Distance.FromKilometers(3)));
                }
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
                // 1. ��������� JSON �� �������� � ������
                using var resourceStream = await FileSystem.OpenAppPackageFileAsync("gyms.json");
                if (resourceStream == null)
                {
                    Debug.WriteLine("���� gyms.json �� ������ � �������� ����������");
                    return new List<Gym>();
                }

                // 2. ������ ���������� � ������
                using var reader = new StreamReader(resourceStream);
                var jsonContent = await reader.ReadToEndAsync();

                // 3. ������������� ����� �� ������ (��� ������ � ����)
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
