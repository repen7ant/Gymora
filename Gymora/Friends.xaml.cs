using System.Reflection;
using System.Text.Json;

namespace Gymora;

public partial class Friends : ContentPage
{
    public List<Friend> FriendsList { get; set; }

    public Friends()
    {
        InitializeComponent();
        LoadFriendsAsync();
        BindingContext = this;
    }

    private async void LoadFriendsAsync()
    {
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("friends.json");
            using var reader = new StreamReader(stream);
            string json = await reader.ReadToEndAsync();
            FriendsList = JsonSerializer.Deserialize<List<Friend>>(json);
            FriendsCollectionView.ItemsSource = FriendsList;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Не удалось загрузить друзей: {ex.Message}", "OK");
        }
    }

    private void OnFriendSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Friend selectedFriend)
        {
            ShowFriendProfile(selectedFriend);
            FriendsCollectionView.SelectedItem = null;
        }
    }

    private void ShowFriendProfile(Friend friend)
    {
        ModalProfileImage.Source = friend.Picture ?? "profile.png";
        ModalUsernameLabel.Text = friend.Username;
        ModalWorkoutsCountLabel.Text = friend.CompletedWorkouts.ToString();
        ModalFriendsCountLabel.Text = friend.Friends.ToString();
        ModalGymLabel.Text = friend.Gym;
        ModalGoalLabel.Text = friend.Goal;

        if (DateTime.TryParse(friend.BirthDate, out DateTime birthDate))
        {
            int age = DateTime.Now.Year - birthDate.Year;
            if (DateTime.Now.DayOfYear < birthDate.DayOfYear) age--;
            ModalBirthdateLabel.Text = $"{birthDate:dd MMMM yyyy} ({age} лет)";
        }
        else
            ModalBirthdateLabel.Text = friend.BirthDate;

        ProfileModal.IsVisible = true;
    }

    private void OnCloseModalClicked(object sender, EventArgs e) => ProfileModal.IsVisible = false;
    private void OnModalBackgroundTapped(object sender, EventArgs e) => ProfileModal.IsVisible = false;
}

public class Friend
{
    public string Username { get; set; }
    public string Picture { get; set; }
    public int CompletedWorkouts { get; set; }
    public int Friends { get; set; }
    public string BirthDate { get; set; }
    public string Gym { get; set; }
    public string Goal { get; set; }
}