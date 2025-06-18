using Firebase.Auth;
using Gymora.Models;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using System.Diagnostics;
using System.Text.Json;

namespace Gymora;

public partial class ProfilePage : ContentPage
{
    private FirebaseAuthClient _authClient;

    public ProfilePage(FirebaseAuthClient authClient)
    {
        InitializeComponent();
        _authClient = authClient;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _authClient = Application.Current.Handler.MauiContext.Services.GetRequiredService<FirebaseAuthClient>();
        LoadUserProfileFromFirebaseAsync();
        UpdateWorkoutCount();
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        bool answer = await Application.Current.MainPage.DisplayAlert(
            "Редактирование профиля",
            "Вы уверены, что хотите сбросить профиль?",
            "Да", "Отмена");

        if (answer)
        {
            await Shell.Current.GoToAsync("//SignUp2");
        }
    }
    private async Task LoadUserProfileFromFirebaseAsync()
    {
        try
        {
            if (_authClient == null)
            {
                await DisplayAlert("Ошибка", "Firebase клиент не инициализирован", "OK");
                return;
            }

            if (_authClient?.User?.Info == null)
            {
                await DisplayAlert("Ошибка", "Пользователь не авторизован", "OK");
                return;
            }

            var userInfo = _authClient.User.Info;
            UsernameLabel.Text = $"@{userInfo.DisplayName}";

            string json = Preferences.Get("user_profile", null);
            Debug.WriteLine("JSON из Preferences: " + json);
            if (string.IsNullOrEmpty(json))
            {
                await DisplayAlert("Ошибка", "Локальные данные профиля не найдены", "OK");
                return;
            }

            var data = JsonSerializer.Deserialize<Models.User>(json);

            if (data == null)
            {
                await DisplayAlert("Ошибка", "Не удалось прочитать данные", "OK");
                return;
            }

            if (DateOnly.TryParse(data.BirthdateString, out var birthdate))
            {
                BirthdateLabel.Text = $"{birthdate.Day} {GetMonthName(birthdate.Month)} {birthdate.Year}";
            }
            else
            {
                BirthdateLabel.Text = "Не указана";
            }
            GymLabel.Text = data.Gym ?? "Не указан";
            GoalLabel.Text = data.Goal ?? "Цель не указана";

            UpdateTrainingDays(data.SelectedDays);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ошибка Firebase: {ex.Message}");
            await DisplayAlert("Ошибка", $"Не удалось загрузить профиль: {ex.Message}", "OK");
        }
    }

    private string GetMonthName(int month)
    {
        string[] months = { "января", "февраля", "марта", "апреля", "мая", "июня",
                              "июля", "августа", "сентября", "октября", "ноября", "декабря" };
        return month >= 1 && month <= 12 ? months[month - 1] : "месяца";
    }

    private void UpdateTrainingDays(List<string> selectedDays)
    {
        TrainingDaysLayout.Children.Clear();

        var daysInOrder = new List<string> { "пн", "вт", "ср", "чт", "пт", "сб", "вс" };

        foreach (var day in daysInOrder)
        {
            var isSelected = selectedDays?.Contains(day) ?? false;

            var border = new Border
            {
                StrokeShape = new RoundRectangle { CornerRadius = 30 },
                Padding = new Thickness(10),
                BackgroundColor = isSelected ? Color.FromArgb("#CCFF00") : Colors.Black,
                Stroke = isSelected ? null : new SolidColorBrush(Colors.White)
            };

            var label = new Label
            {
                Text = day.ToUpper(),
                FontSize = 12,
                TextColor = isSelected ? Colors.Black : Colors.White
            };

            border.Content = label;
            TrainingDaysLayout.Children.Add(border);
        }
    }

    private void UpdateWorkoutCount()
    {
        try
        {
            var json = Preferences.Get("checked_dates", null);
            if (!string.IsNullOrEmpty(json))
            {
                var checkedDates = JsonSerializer.Deserialize<List<DateTime>>(json);
                if (checkedDates != null)
                {
                    var pastWorkouts = checkedDates.Where(d => d.Date <= DateTime.Today).Count();
                    WorkoutsCountLabel.Text = pastWorkouts.ToString();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ошибка при загрузке количества тренировок: {ex.Message}");
        }
    }

}