using System.Collections.ObjectModel;
using System.Timers;
using System.Windows.Input;
using Gymora.Converters;
using Gymora.Models;
using Microsoft.Maui.Controls;

namespace Gymora;

public partial class WorkoutExecutionPage : ContentPage
{
    public WorkoutProgram Program { get; }

    public ICommand ToggleExerciseCommand { get; }
    public ICommand CompleteSetCommand { get; }
    public ICommand FinishWorkoutCommand { get; }
    public ObservableCollection<Exercise> Exercises { get; set; }

    private System.Timers.Timer _restTimer;
    private int _remainingSeconds;
    private Exercise _currentExercise;
    private ExerciseSet _currentSet;
    private bool _isExerciseRest;
    private Exercise _previousExercise;

    public WorkoutExecutionPage(WorkoutProgram program)
    {
        InitializeComponent();
        Program = program;
        Exercises = new ObservableCollection<Exercise>(Program.Exercises);

        ToggleExerciseCommand = new Command<Exercise>(ToggleExercise);
        CompleteSetCommand = new Command<ExerciseSet>(CompleteSet);
        FinishWorkoutCommand = new Command(async () => await FinishWorkout());

        BindingContext = this;

        Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = false });

        _restTimer = new System.Timers.Timer(1000);
        _restTimer.Elapsed += OnRestTimerElapsed;
        _restTimer.AutoReset = true;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        for (int i = 0; i < Exercises.Count; i++)
        {
            Exercises[i].ShowExerciseRestTimer = i > 0;
            Exercises[i].IsExpanded = false;
            Exercises[i].IsResting = false;
        }
    }

    private void ToggleExercise(Exercise exercise)
    {
        exercise.IsExpanded = !exercise.IsExpanded;
    }

    private void CompleteSet(ExerciseSet set)
    {
        if (set == null) return;

        set.IsCompleted = true;
        _currentSet = set;

        _currentExercise = Exercises.FirstOrDefault(e => e.Sets.Contains(set));
        if (_currentExercise == null) return;

        var isLastSet = _currentExercise.Sets.IndexOf(set) == _currentExercise.Sets.Count - 1;

        if (isLastSet)
        {
            _currentExercise.IsResting = false;
            StartExerciseRestTimer(Program.RestBetweenExercises.TotalSeconds);
        }
        else
        {
            StartRestTimer(Program.RestBetweenSets.TotalSeconds);
        }
    }

    private void StartRestTimer(double seconds)
    {
        _remainingSeconds = (int)seconds;
        _isExerciseRest = false;

        _currentExercise.IsResting = true;
        _currentExercise.RestProgress = 1.0;
        _currentExercise.RestTimeText = $"{_remainingSeconds} сек";

        _restTimer.Start();
    }

    private void StartExerciseRestTimer(double seconds)
    {
        _remainingSeconds = (int)seconds;
        _isExerciseRest = true;

        var nextExerciseIndex = Exercises.IndexOf(_currentExercise) + 1;
        if (nextExerciseIndex < Exercises.Count)
        {
            var nextExercise = Exercises[nextExerciseIndex];
            nextExercise.ExerciseRestProgress = 1.0;
            nextExercise.ExerciseRestTimeText = $"{_remainingSeconds} сек";
        }

        _restTimer.Start();
    }

    private void OnRestTimerElapsed(object sender, ElapsedEventArgs e)
    {
        _remainingSeconds--;

        Dispatcher.Dispatch(() =>
        {
            if (_isExerciseRest)
            {
                var nextExerciseIndex = Exercises.IndexOf(_currentExercise) + 1;
                if (nextExerciseIndex < Exercises.Count)
                {
                    var nextExercise = Exercises[nextExerciseIndex];
                    nextExercise.ExerciseRestProgress = (double)_remainingSeconds / Program.RestBetweenExercises.TotalSeconds;
                    nextExercise.ExerciseRestTimeText = $"{_remainingSeconds} сек";
                }
            }
            else
            {
                _currentExercise.RestProgress = (double)_remainingSeconds / Program.RestBetweenSets.TotalSeconds;
                _currentExercise.RestTimeText = $"{_remainingSeconds} сек";
            }

            if (_remainingSeconds <= 0)
            {
                _restTimer.Stop();
                if (_isExerciseRest)
                {
                    var nextExerciseIndex = Exercises.IndexOf(_currentExercise) + 1;
                    if (nextExerciseIndex < Exercises.Count)
                    {
                        Exercises[nextExerciseIndex].ShowExerciseRestTimer = false;
                    }
                }
                else
                    _currentExercise.IsResting = false;
            }
        });
    }

    private async Task FinishWorkout()
    {
        bool answer = await DisplayAlert("Подтверждение", "Хотите закончить тренировку?", "Да", "Нет");
        if (answer)
        {
            ResetWorkoutState();
            await Navigation.PopAsync();
        }
    }

    private void ResetWorkoutState()
    {
        if (_restTimer != null && _restTimer.Enabled)
            _restTimer.Stop();

        foreach (var exercise in Program.Exercises)
            exercise.Reset();

        if (Program.Exercises.Count > 0)
            Program.Exercises[0].ShowExerciseRestTimer = false;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        ResetWorkoutState(); 
    }
}