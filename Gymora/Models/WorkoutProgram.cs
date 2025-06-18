using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Gymora.Models
{
    public class WorkoutProgram
    {
        public string Name { get; set; }
        public TimeSpan RestBetweenSets { get; set; }
        public TimeSpan RestBetweenExercises { get; set; }
        public List<Exercise> Exercises { get; set; } = new List<Exercise>();
    }

    public class Exercise : INotifyPropertyChanged
    {
        private bool _isExpanded;
        private bool _isResting;
        private double _restProgress;
        private string _restTimeText;
        private bool _showExerciseRestTimer;
        private double _exerciseRestProgress;
        private string _exerciseRestTimeText;

        public string Name { get; set; }
        public MuscleGroup MuscleGroup { get; set; }
        public List<ExerciseSet> Sets { get; set; } = new List<ExerciseSet>();

        public void Reset()
        {
            IsExpanded = false;
            IsResting = false;
            RestProgress = 0;
            RestTimeText = string.Empty;
            ShowExerciseRestTimer = true;
            ExerciseRestProgress = 0;
            ExerciseRestTimeText = string.Empty;

            foreach (var set in Sets)
                set.IsCompleted = false;
        }

        public bool ShowExerciseRestTimer
        {
            get => _showExerciseRestTimer;
            set
            {
                _showExerciseRestTimer = value;
                OnPropertyChanged();
            }
        }

        public double ExerciseRestProgress
        {
            get => _exerciseRestProgress;
            set
            {
                _exerciseRestProgress = value;
                OnPropertyChanged();
            }
        }

        public string ExerciseRestTimeText
        {
            get => _exerciseRestTimeText;
            set
            {
                _exerciseRestTimeText = value;
                OnPropertyChanged();
            }
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                _isExpanded = value;
                OnPropertyChanged();
            }
        }

        public bool IsResting
        {
            get => _isResting;
            set
            {
                _isResting = value;
                OnPropertyChanged();
            }
        }

        public double RestProgress
        {
            get => _restProgress;
            set
            {
                _restProgress = value;
                OnPropertyChanged();
            }
        }

        public string RestTimeText
        {
            get => _restTimeText;
            set
            {
                _restTimeText = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string MuscleGroupDisplay =>
        MuscleGroup switch
        {
            MuscleGroup.Biceps => "Бицепс",
            MuscleGroup.Triceps => "Трицепс",
            MuscleGroup.Shoulders => "Плечи",
            MuscleGroup.Back => "Спина",
            MuscleGroup.Chest => "Грудь",
            MuscleGroup.Abs => "Пресс",
            MuscleGroup.Glutes => "Ягодицы",
            MuscleGroup.Legs => "Ноги",
            _ => MuscleGroup.ToString()
        };
    }

    public class ExerciseSet : INotifyPropertyChanged
    {
        private bool _isCompleted;

        public int SetNumber { get; set; }
        public double? Weight { get; set; }
        public int? Reps { get; set; }

        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                if (_isCompleted != value)
                {
                    _isCompleted = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum MuscleGroup
    {
        Biceps,
        Triceps,
        Shoulders,
        Back,
        Chest,
        Abs,
        Glutes,
        Legs
    }
}
