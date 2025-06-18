
using Gymora.Models;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;
using YourNamespace.Models;

namespace Gymora.ViewModels
{
    public partial class SignUpViewModel2 : INotifyPropertyChanged
    {
        public ObservableCollection<SelectableItem> Days { get; set; }
        public ICommand LoadPhotoCommand { get; }
        public ICommand NextCommand { get; }

        public SignUpViewModel2()
        {
            Days = new ObservableCollection<SelectableItem>
             {
                new SelectableItem { Name = "пн", IsSelected = false },
                new SelectableItem { Name = "вт", IsSelected = false },
                new SelectableItem { Name = "ср", IsSelected = false },
                new SelectableItem { Name = "чт", IsSelected = false },
                new SelectableItem { Name = "пт", IsSelected = false },
                new SelectableItem { Name = "сб", IsSelected = false },
                new SelectableItem { Name = "вс", IsSelected = false }
            };

            LoadPhotoCommand = new Command(OnLoadPhoto);
            NextCommand = new Command(SaveAndNext);
        }
        private DateTime _birthdate = DateTime.Now.AddYears(-18); // Используем DateTime вместо DateOnly

        public DateTime Birthdate
        {
            get => _birthdate;
            set
            {
                if (_birthdate != value)
                {
                    _birthdate = value;
                    OnPropertyChanged();
                    Debug.WriteLine($"Дата изменена на: {value:yyyy-MM-dd}");
                }
            }
        }



        private string _gym;
        public string Gym
        {
            get => _gym;
            set
            {
                _gym = value;
                OnPropertyChanged();
            }
        }

        private string _goal;
        public string Goal
        {
            get => _goal;
            set
            {
                _goal = value;
                OnPropertyChanged();
            }
        }

        private async void OnLoadPhoto()
        {
            try
            {
                var result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Выберите фото профиля"
                });

                if (result == null) return;

                using var stream = await result.OpenReadAsync();
                byte[] imageBytes = new byte[stream.Length];
                await stream.ReadAsync(imageBytes);

                string base64 = Convert.ToBase64String(imageBytes);
                Preferences.Set("user_photo", base64);

                // Вызываем событие обновления изображения на странице
                PhotoLoaded?.Invoke(this, new EventArgs());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка загрузки фото: {ex.Message}");
            }
        }
        
       
        private async void SaveAndNext()
        {
            try
            {
                var age = DateTime.Now.Year - Birthdate.Year;
                if (age < 10 || age > 100)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка",
                        "Пожалуйста, укажите корректную дату рождения", "OK");
                    return;
                }
                var user = new User
                {
                    BirthdateString = this.Birthdate.ToString("yyyy-MM-dd"),
                    Gym = Gym,
                    Goal = Goal,
                    SelectedDays = Days.Where(d => d.IsSelected).Select(d => d.Name.ToLower()).ToList(),
                    PhotoBase64 = Preferences.Get("user_photo", null)
                };

                string json = JsonSerializer.Serialize(user);
                Preferences.Set("user_profile", json);

                Debug.WriteLine("Сохраняемый JSON: " + json);

                await Shell.Current.GoToAsync("//MainPage");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", ex.Message, "OK");
            }
        }

        public event EventHandler PhotoLoaded;
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}