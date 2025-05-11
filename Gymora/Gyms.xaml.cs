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
            var defaultPosition = new Location(56.838011, 60.597465); // Координаты Екатеринбурга
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
                    Debug.WriteLine("Нет данных о залах");
                    return;
                }

                // Добавляем маркеры
                foreach (var gym in gyms)
                {
                    if (double.IsNaN(gym.Latitude) || double.IsNaN(gym.Longitude))
                    {
                        Debug.WriteLine($"Пропущен зал {gym.Name}: неверные координаты");
                        continue;
                    }

                    // Проверяем, что Label не пустой (иначе будет ошибка)
                    if (string.IsNullOrWhiteSpace(gym.Name))
                    {
                        Debug.WriteLine($"Пропущен зал: не указано название");
                        continue;
                    }

                    var pin = new Pin
                    {
                        Label = gym.Name ?? "Неизвестный зал", // Если Name == null, задаем дефолтное значение
                        Address = gym.Location ?? "Адрес не указан",
                        Location = new Location(gym.Latitude, gym.Longitude),
                        Type = PinType.Place
                    };

                    pin.MarkerClicked += async (s, e) =>
                    {
                        e.HideInfoWindow = true;
                        await Navigation.PushModalAsync(new GymDetailPage(gym));
                    };

                    GymMap.Pins.Add(pin);
                    Debug.WriteLine($"Добавлен маркер: {pin.Label} ({gym.Latitude}, {gym.Longitude})");
                }

                // Перемещаем карту к первому залу
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
                Debug.WriteLine($"Ошибка: {ex}");
                await DisplayAlert("Ошибка", "Не удалось загрузить данные о залах", "OK");
            }
        }

        private async Task<List<Gym>> LoadGymsFromJson()
        {
            try
            {
                // 1. Загружаем JSON из ресурсов в память
                using var resourceStream = await FileSystem.OpenAppPackageFileAsync("gyms.json");
                if (resourceStream == null)
                {
                    Debug.WriteLine("Файл gyms.json не найден в ресурсах приложения");
                    return new List<Gym>();
                }

                // 2. Читаем содержимое в строку
                using var reader = new StreamReader(resourceStream);
                var jsonContent = await reader.ReadToEndAsync();

                // 3. Десериализуем прямо из памяти (без записи в файл)
                var gyms = JsonConvert.DeserializeObject<List<Gym>>(jsonContent);

                Debug.WriteLine($"Успешно загружено {gyms?.Count ?? 0} залов");
                return gyms ?? new List<Gym>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Критическая ошибка загрузки: {ex}");
                return new List<Gym>();
            }
        }
    }
}
