using Gymora.Models;
using System;
using System.Collections.ObjectModel;

namespace Gymora;

public partial class GymDetailPage : ContentPage
{
    private readonly Gym _gym;

    public GymDetailPage(Gym gym)
    {
        InitializeComponent();
        _gym = gym;
        LoadGymData();
    }

    private void LoadGymData()
    {
        NameLabel.Text = _gym.Name;
        AddressLabel.Text = _gym.Location;
        RatingLabel.Text = _gym.Rating.ToString("0.0");
        PhoneLabel.Text = _gym.Number;
        WebsiteLabel.Text = _gym.Website;

        ImagesCarousel.ItemsSource = _gym.Images ?? new List<string>();

        PricesContainer.Children.Clear();

        if (_gym.Prices != null && _gym.Prices.Count > 0)
        {
            for (int i = 0; i < _gym.Prices.Count; i++)
            {
                var option = _gym.Prices[i];

                PricesContainer.Children.Add(new Label
                {
                    Text = option.Name,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Colors.White,
                    HorizontalOptions = LayoutOptions.Start
                });

                foreach (var price in option.Prices)
                {
                    var formatted = new FormattedString();
                    formatted.Spans.Add(new Span { Text = $"{price.Key} — ", TextColor = Colors.White });
                    formatted.Spans.Add(new Span { Text = $"{price.Value}", TextColor = Color.FromArgb("#D5FF40") });
                    formatted.Spans.Add(new Span { Text = " руб", TextColor = Colors.White });

                    PricesContainer.Children.Add(new Label
                    {
                        FormattedText = formatted,
                        HorizontalOptions = LayoutOptions.Center
                    });
                }

                if (i < _gym.Prices.Count - 1)
                {
                    PricesContainer.Children.Add(new BoxView
                    {
                        HeightRequest = 1,
                        Color = Colors.Gray,
                        Margin = new Thickness(0, 5)
                    });
                }
            }
        }

        if (_gym.WorkingHours != null)
        {
            foreach (var hours in _gym.WorkingHours)
            {
                HoursContainer.Children.Add(new Label
                {
                    Text = hours,
                    TextColor = Colors.Black,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center
                });
            }
        }
    }

    private async void OnRatingClicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(_gym._2GISLink))
        {
            try
            {
                await Launcher.Default.OpenAsync(_gym._2GISLink);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", "Не удалось открыть сайт.", "OK");
            }
        }
    }

    private async void OnWebsiteTapped(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(_gym.Website))
        {
            var url = _gym.Website.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                ? _gym.Website
                : $"https://{_gym.Website}";

            try
            {
                await Launcher.Default.OpenAsync(url);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", "Не удалось открыть сайт.", "OK");
            }
        }
    }

    private async void OnCloseButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}