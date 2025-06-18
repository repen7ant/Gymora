using Gymora.Models;
using System.Collections.ObjectModel;

namespace Gymora;

public partial class CreateProgramPage : ContentPage
{
    private readonly MainPage _mainPage;
    private readonly WorkoutProgram _editProgram;
    private readonly bool _isEditMode;
    public ObservableCollection<Exercise> Exercises { get; } = new ObservableCollection<Exercise>();

    public CreateProgramPage(MainPage mainPage, WorkoutProgram programToEdit = null, bool isEditMode = false)
    {
        InitializeComponent();
        _mainPage = mainPage;
        _editProgram = programToEdit;
        _isEditMode = isEditMode;

        if (isEditMode && _editProgram != null)
        {
            ProgramNameEntry.Text = _editProgram.Name;
            RestBetweenSetsEntry.Text = _editProgram.RestBetweenSets.TotalSeconds.ToString();
            RestBetweenExercisesEntry.Text = _editProgram.RestBetweenExercises.TotalSeconds.ToString();
            foreach (var ex in _editProgram.Exercises)
                Exercises.Add(CloneExercise(ex));
        }

        ExercisesCollectionView.ItemsSource = Exercises;
    }

    private Exercise CloneExercise(Exercise exercise)
    {
        return new Exercise
        {
            Name = exercise.Name,
            MuscleGroup = exercise.MuscleGroup,
            Sets = exercise.Sets.Select(s => new ExerciseSet
            {
                SetNumber = s.SetNumber,
                Weight = s.Weight,
                Reps = s.Reps,
                IsCompleted = s.IsCompleted
            }).ToList()
        };
    }

    private async void OnAddExerciseClicked(object sender, EventArgs e)
    {
        var exercisePage = new CreateExercisePage();
        exercisePage.ExerciseCreated += (s, exercise) =>
        {
            Exercises.Add(exercise);
        };

        await Navigation.PushModalAsync(exercisePage);
    }

    private void OnDeleteExerciseClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is Exercise exercise)
        {
            Exercises.Remove(exercise);
        }
    }

    private async void OnEditExerciseClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is Exercise exercise)
        {
            var index = Exercises.IndexOf(exercise);
            var exercisePage = new CreateExercisePage(exercise);

            exercisePage.ExerciseCreated += (s, updatedExercise) =>
            {
                Exercises[index] = updatedExercise;
            };

            await Navigation.PushModalAsync(exercisePage);
        }
    }

    private void OnSaveProgramClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ProgramNameEntry.Text))
        {
            DisplayAlert("Ошибка", "Введите название программы", "OK");
            return;
        }

        if (Exercises.Count == 0)
        {
            DisplayAlert("Ошибка", "Добавьте хотя бы одно упражнение", "OK");
            return;
        }

        if (!int.TryParse(RestBetweenSetsEntry.Text, out var restBetweenSets))
        {
            restBetweenSets = 60; // значение по умолчанию
        }

        if (!int.TryParse(RestBetweenExercisesEntry.Text, out var restBetweenExercises))
        {
            restBetweenExercises = 120; // значение по умолчанию
        }

        var program = new WorkoutProgram
        {
            Name = ProgramNameEntry.Text,
            RestBetweenSets = TimeSpan.FromSeconds(restBetweenSets),
            RestBetweenExercises = TimeSpan.FromSeconds(restBetweenExercises),
            Exercises = new List<Exercise>(Exercises)
        };

        if (_isEditMode && _editProgram != null)
        {
            _editProgram.Name = program.Name;
            _editProgram.RestBetweenSets = program.RestBetweenSets;
            _editProgram.RestBetweenExercises = program.RestBetweenExercises;
            _editProgram.Exercises.Clear();
            foreach (var ex in program.Exercises) _editProgram.Exercises.Add(CloneExercise(ex));
        }
        else
        {
            _mainPage.AddProgram(program);
        }

        Navigation.PopModalAsync();
    }
}