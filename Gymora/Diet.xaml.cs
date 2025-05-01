namespace Gymora;
public partial class Diet : ContentPage
{
    public Diet()
    {
        InitializeComponent();
    }

    private void OnCalculateClicked(object sender, EventArgs e)
    {
        try
        {
            ResultGrid.IsVisible = false;
            ResultGrid.Children.Clear();
            ResultGrid.RowDefinitions.Clear();

            if (!int.TryParse(AgeEntry.Text, out int age) || age < 0 || age > 120 ||
                !double.TryParse(HeightEntry.Text, out double height) || height < 120 || height > 250 ||
                !double.TryParse(WeightEntry.Text, out double weight) || weight < 20 || weight > 350 ||
                GenderPicker.SelectedIndex == -1 ||
                ActivityPicker.SelectedIndex == -1)
            {
                ResultGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                var errorLabel = new Label
                {
                    Text = "Заполните все поля корректно.",
                    TextColor = Colors.Red,
                    FontSize = 16
                };

                ResultGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                ResultGrid.Children.Add(errorLabel);
                Grid.SetRow(errorLabel, 0);
                Grid.SetColumn(errorLabel, 0);
                Grid.SetColumnSpan(errorLabel, 2);
                ResultGrid.IsVisible = true;
                return;
            }

            string gender = GenderPicker.SelectedItem.ToString();
            double bmr = gender == "Male"
                ? 10 * weight + 6.25 * height - 5 * age + 5
                : 10 * weight + 6.25 * height - 5 * age - 161;

            double activityMultiplier = ActivityPicker.SelectedIndex switch
            {
                0 => 1.2,
                1 => 1.375,
                2 => 1.55,
                3 => 1.725,
                4 => 1.9,
                _ => 1.2
            };

            double tdee = bmr * activityMultiplier;

            var results = new (string Label, double Calories)[]
            {
            ("Поддержание веса", tdee),
            ("Умеренное похудение \n(0.25 кг/нед)", tdee - 250),
            ("Похудение \n(0.5 кг/нед)", tdee - 500),
            ("Экстремальное похудение \n(1 кг/нед)", tdee - 1000),
            ("Умеренный набор веса \n(0.25 кг/нед)", tdee + 250),
            ("Набор веса \n(0.5 кг/нед)", tdee + 500),
            ("Экстремальный набор веса \n(1 кг/нед)", tdee + 1000)
            };

            for (int i = 0; i < results.Length; i++)
            {
                ResultGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                var goalLabel = new Label
                {
                    Text = results[i].Label,
                    FontSize = 16,
                    TextColor = Colors.Black
                };

                var kcalLabel = new Label
                {
                    Text = $"{results[i].Calories:F0} ккал/день",
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 16,
                    TextColor = Colors.DarkGreen,
                    HorizontalTextAlignment = TextAlignment.End
                };

                ResultGrid.Children.Add(goalLabel);
                Grid.SetRow(goalLabel, i);
                Grid.SetColumn(goalLabel, 0);

                ResultGrid.Children.Add(kcalLabel);
                Grid.SetRow(kcalLabel, i);
                Grid.SetColumn(kcalLabel, 1);
            }

            ResultGrid.IsVisible = true;
        }
        catch (Exception ex)
        {
            ResultGrid.IsVisible = true;
            ResultGrid.Children.Clear();
            ResultGrid.RowDefinitions.Clear(); // <-- Очистим старые строки

            ResultGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var errorLabel = new Label
            {
                Text = $"Ошибка: {ex.Message}",
                TextColor = Colors.Red,
                FontSize = 16
            };

            ResultGrid.Children.Add(errorLabel);
            Grid.SetRow(errorLabel, 0);
            Grid.SetColumn(errorLabel, 0);
            Grid.SetColumnSpan(errorLabel, 2);
        }
    }
}