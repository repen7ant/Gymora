using Gymora.Models;
using System.Collections.ObjectModel;

namespace Gymora;

public partial class CreateExercisePage : ContentPage
{
    public event EventHandler<Exercise> ExerciseCreated;
    private Exercise _exercise;

    public CreateExercisePage(Exercise exercise = null)
    {
        InitializeComponent();
        Title = "Упражнение";

        _exercise = exercise ?? new Exercise { Sets = new List<ExerciseSet> { new ExerciseSet { SetNumber = 1 } } };

        if (exercise != null)
        {
            ExerciseNameEntry.Text = exercise.Name;
            MuscleGroupPicker.SelectedIndex = (int)exercise.MuscleGroup;
            SetsCollectionView.ItemsSource = new ObservableCollection<ExerciseSet>(exercise.Sets);
        }
        else
        {
            SetsCollectionView.ItemsSource = new ObservableCollection<ExerciseSet>(_exercise.Sets);
        }
    }

    private async void OnDeleteSetClicked(object sender, EventArgs e)
    {
        var sets = (ObservableCollection<ExerciseSet>)SetsCollectionView.ItemsSource;

        if (sets.Count <= 1)
        {
            await DisplayAlert("Ошибка", "Должен остаться хотя бы один подход", "OK");
            return;
        }

        if (sender is Button button && button.BindingContext is ExerciseSet set)
        {
            bool answer = await DisplayAlert("Подтверждение",
                                         "Удалить этот подход?",
                                         "✔", "❌");
            if (answer)
            {
                sets.Remove(set);

                for (int i = 0; i < sets.Count; i++)
                {
                    sets[i].SetNumber = i + 1;
                }
            }
        }
    }

    private void OnAddSetClicked(object sender, EventArgs e)
    {
        var sets = (ObservableCollection<ExerciseSet>)SetsCollectionView.ItemsSource;
        sets.Add(new ExerciseSet { SetNumber = sets.Count + 1 });
    }

    private async void OnSaveExerciseClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ExerciseNameEntry.Text))
        {
            await DisplayAlert("Ошибка", "Введите название упражнения", "OK");
            return;
        }

        if (MuscleGroupPicker.SelectedIndex == -1)
        {
            await DisplayAlert("Ошибка", "Выберите группу мышц", "OK");
            return;
        }

        var sets = (ObservableCollection<ExerciseSet>)SetsCollectionView.ItemsSource;

        if (sets.Count == 0)
        {
            await DisplayAlert("Ошибка", "Добавьте хотя бы один подход", "OK");
            return;
        }

        foreach (var set in sets)
        {
            if (set.Weight == null || set.Reps == null)
            {
                await DisplayAlert("Ошибка", "Заполните все поля для подходов", "OK");
                return;
            }
        }

        _exercise.Name = ExerciseNameEntry.Text;
        _exercise.MuscleGroup = (MuscleGroup)MuscleGroupPicker.SelectedIndex;
        _exercise.Sets = new List<ExerciseSet>(sets);

        ExerciseCreated?.Invoke(this, _exercise);
        await Navigation.PopModalAsync();
    }
}
