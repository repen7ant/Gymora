using Gymora.Models;
using System.Collections.ObjectModel;

namespace Gymora
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<WorkoutProgram> Programs { get; } = new ObservableCollection<WorkoutProgram>();

        public MainPage()
        {
            InitializeComponent();
            calendar.SelectedDate = DateTime.Now;
            ProgramsCollectionView.ItemsSource = Programs;

            if (Programs.Count == 0)
                InitializeDefaultPrograms();
        }

        private void InitializeDefaultPrograms()
        {
            var chestTricepsShoulders = new WorkoutProgram
            {
                Name = "Грудь + Трицепс + Плечи",
                RestBetweenSets = TimeSpan.FromSeconds(90),
                RestBetweenExercises = TimeSpan.FromMinutes(3),
                Exercises = new List<Exercise>
                {
                    CreateExercise("Жим штанги лежа", MuscleGroup.Chest, 4, "8-10", 120),
                    CreateExercise("Жим гантелей на наклонной", MuscleGroup.Chest, 3, "10-12", 90),
                    CreateExercise("Отжимания на брусьях (с весом)", MuscleGroup.Triceps, 3, "8-10", 90),
                    CreateExercise("Разводка гантелей лежа", MuscleGroup.Chest, 3, "12-15", 60),
                    CreateExercise("Французский жим", MuscleGroup.Triceps, 3, "10-12", 60),
                    CreateExercise("Махи гантелями в стороны", MuscleGroup.Shoulders, 3, "12-15", 60)
                }
            };

            var backBiceps = new WorkoutProgram
            {
                Name = "Спина + Бицепс",
                RestBetweenSets = TimeSpan.FromSeconds(90),
                RestBetweenExercises = TimeSpan.FromMinutes(3),
                Exercises = new List<Exercise>
                {
                    CreateExercise("Подтягивания широким хватом", MuscleGroup.Back, 4, "6-8", 120),
                    CreateExercise("Тяга штанги в наклоне", MuscleGroup.Back, 4, "8-10", 90),
                    CreateExercise("Тяга вертикального блока", MuscleGroup.Back, 3, "10-12", 60),
                    CreateExercise("Тяга гантели одной рукой", MuscleGroup.Back, 3, "10-12", 60),
                    CreateExercise("Подъем штанги на бицепс", MuscleGroup.Biceps, 3, "10-12", 60),
                    CreateExercise("Молотки с гантелями", MuscleGroup.Biceps, 3, "12-15", 60)
                }
            };

            var legsAbs = new WorkoutProgram
            {
                Name = "Ноги + Пресс",
                RestBetweenSets = TimeSpan.FromSeconds(120),
                RestBetweenExercises = TimeSpan.FromMinutes(3),
                Exercises = new List<Exercise>
                {
                    CreateExercise("Приседания со штангой", MuscleGroup.Legs, 4, "6-8", 180),
                    CreateExercise("Румынская тяга", MuscleGroup.Legs, 4, "8-10", 120),
                    CreateExercise("Выпады с гантелями", MuscleGroup.Legs, 3, "10-12", 90),
                    CreateExercise("Подъем на носки стоя", MuscleGroup.Legs, 4, "15-20", 60),
                    CreateExercise("Скручивания на пресс", MuscleGroup.Abs, 3, "15-20", 45)
                }
            };

            Programs.Add(chestTricepsShoulders);
            Programs.Add(backBiceps);
            Programs.Add(legsAbs);
        }

        private Exercise CreateExercise(string name, MuscleGroup muscleGroup, int setsCount, string reps, int restSeconds)
        {
            var exercise = new Exercise
            {
                Name = name,
                MuscleGroup = muscleGroup,
                Sets = new List<ExerciseSet>()
            };

            for (int i = 0; i < setsCount; i++)
            {
                exercise.Sets.Add(new ExerciseSet
                {
                    SetNumber = i + 1,
                    Weight = 0, // Вес по умолчанию 0
                    Reps = int.Parse(reps.Split('-')[0]) 
                });
            }

            return exercise;
        }

        private void calendar_OnDateSelected(object sender, DateTime e)
        {
            // Логика выбора даты
        }

        private async void OnAddProgramClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new CreateProgramPage(this));
        }

        public void AddProgram(WorkoutProgram program)
        {
            Programs.Add(program);
        }

        private async void OnEditProgramClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.BindingContext is WorkoutProgram program)
            {
                var editPage = new CreateProgramPage(this, program, isEditMode: true);
                await Navigation.PushModalAsync(editPage);
            }
        }

        private async void OnStartProgramClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.BindingContext is WorkoutProgram program)
            {
                ResetProgramState(program);

                bool answer = await DisplayAlert("Подтверждение", "Хотите начать?", "✔", "❌");
                if (answer)
                {
                    await Navigation.PushAsync(new WorkoutExecutionPage(program));
                }
            }
        }

        private void ResetProgramState(WorkoutProgram program)
        {
            foreach (var exercise in program.Exercises)
            {
                exercise.Reset();

                foreach (var set in exercise.Sets)
                {
                    set.IsCompleted = false;
                }
            }

            if (program.Exercises.Count > 0)
            {
                program.Exercises[0].ShowExerciseRestTimer = false;
            }
        }

        private async void OnDeleteProgramClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.BindingContext is WorkoutProgram program)
            {
                bool answer = await DisplayAlert("Подтверждение", "Удалить эту программу?", "✔", "❌");
                if (answer)
                {
                    Programs.Remove(program);
                }
            }
        }
    }
}
